using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using System;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：双令牌签发、刷新轮换、注销
    /// </summary>
    public class AuthTokenBLL : BLLBase, IAuthTokenBLL, BLL
    {
        private readonly IOperator _operator;
        private readonly ICache _cache;
        private readonly AuthSessionService _sessionService = new AuthSessionService();

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="ioperator">令牌签发/解析</param>
        /// <param name="cache">缓存（Redis 存会话；非 Redis 只用于轮换宽限期临时记录）</param>
        public AuthTokenBLL(IOperator ioperator, ICache cache)
        {
            _operator = ioperator ?? throw new ArgumentNullException(nameof(ioperator));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        /// <summary>
        /// 会话是否存 Redis（否则落库）
        /// </summary>
        private bool UseCache
        {
            get
            {
                return string.Equals(ConfigHelper.GetConfig().CacheType, "Redis",
                    StringComparison.OrdinalIgnoreCase);
            }
        }

        private static string SessionKey(string sid)
        {
            return $"auth_session_{sid}";
        }

        private static string RotationKey(string jti)
        {
            return $"auth_refresh_rot_{jti}";
        }

        private static string AccountRevokeKey(string account)
        {
            return $"auth_account_revoke_{account}";
        }

        /// <summary>
        /// 登录成功后签发访问令牌与刷新令牌，并创建服务端会话
        /// </summary>
        public async Task<AuthTokenResult> Issue(UserEntity user, int sessionHours)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var config = ConfigHelper.GetConfig();
            if (sessionHours <= 0)
            {
                sessionHours = config.JwtExp > 0 ? config.JwtExp : 12;
            }

            var now = DateTime.Now;
            var session = new AuthSessionEntity
            {
                F_Id = Guid.NewGuid().ToString("N"),
                F_Account = user.F_Account,
                F_UserId = user.F_UserId,
                F_UserName = user.F_RealName,
                F_TenantNo = GetTenantNo(),
                F_LoginRandomCode = user.F_LoginRandomCode,
                F_RefreshJti = Guid.NewGuid().ToString("N"),
                F_CreateTime = now,
                F_LastRefreshTime = now,
                F_ExpireTime = now.AddHours(sessionHours),
                F_Revoked = 0,
                F_CreateIp = ContextHelper.GetItem("ip") as string,
                F_Device = ContextHelper.GetItem("device") as string,
            };

            await SaveSession(session);
            return BuildResult(session);
        }

        /// <summary>
        /// 滑动会话：每次成功刷新把绝对到期时间往后推「一整个原会话跨度」。
        /// 跨度用 F_ExpireTime - F_LastRefreshTime 取（签发时 LastRefreshTime = CreateTime），
        /// 刷新后把 LastRefreshTime 也移到 now，跨度因此恒定，不会越滚越大。
        /// 桌面客户端常驻时宿主前端到期前自动续签，会话就不会走到绝对到期、被踢回登录页。
        /// 注意这是明确的安全取舍：refresh token 被盗后可一直续期，靠 logout/吊销标记收回。
        /// </summary>
        private static void SlideSession(AuthSessionEntity session, DateTime now)
        {
            var anchor = session.F_LastRefreshTime ?? session.F_CreateTime ?? now;
            var span = session.F_ExpireTime.HasValue ? session.F_ExpireTime.Value - anchor : TimeSpan.Zero;
            if (span > TimeSpan.Zero)
            {
                session.F_ExpireTime = now.Add(span);
            }
            session.F_LastRefreshTime = now;
        }

        /// <summary>
        /// 给滑动续期加一个从会话创建时间起算的绝对上限（共享模式专用）。
        /// 否则同一份 refresh token 被长期共享时，只要还有人在刷新，会话就永不过期。
        /// </summary>
        private static void ApplyMaxLifetime(AuthSessionEntity session, ServerOp config)
        {
            var maxHours = config.JwtRefreshMaxHours;
            if (maxHours <= 0 || !session.F_CreateTime.HasValue || !session.F_ExpireTime.HasValue)
            {
                return;
            }

            var ceiling = session.F_CreateTime.Value.AddHours(maxHours);
            if (session.F_ExpireTime.Value > ceiling)
            {
                session.F_ExpireTime = ceiling;
            }
        }

        /// <summary>
        /// 用刷新令牌换取新的令牌对。
        /// JwtRefreshSharedMode=true 时刷新幂等（不轮换 jti），供多人共用同一账号/登录态；
        /// =false 时保持严格轮换 + 60 秒宽限期的重放检测。
        /// </summary>
        public async Task<AuthTokenResult> Refresh(string refreshToken)
        {
            var config = ConfigHelper.GetConfig();
            var sharedMode = config.JwtRefreshSharedMode;

            // 共享模式下忽略 JWT 自带的 exp：同一 jti 的不同代令牌分散在多个客户端手里，
            // 某一方滑动续期不会更新其他人的本地副本，令牌是否可用一律以会话表为准。
            // 签名与 typ 仍然严格校验，伪造/过期令牌不会因此被放行。
            var decoded = _operator.Decode(refreshToken, AuthTokenConstants.RefreshType, sharedMode);
            if (!decoded.Success || decoded.Payload == null)
            {
                return null;
            }

            var payload = decoded.Payload;
            if (string.IsNullOrEmpty(payload.Sid) || string.IsNullOrEmpty(payload.Jti))
            {
                return null;
            }

            AuthTokenResult result = null;
            await _cache.Lock(payload.Sid, async _ =>
            {
                var session = await GetSession(payload.Sid);
                if (session == null || session.F_Revoked == 1 ||
                    !session.F_ExpireTime.HasValue || session.F_ExpireTime.Value <= DateTime.Now)
                {
                    return;
                }

                if (!string.Equals(session.F_Account, payload.Account, StringComparison.Ordinal))
                {
                    return;
                }

                // 账号级吊销标记（Redis 模式没有按账号遍历会话的能力，用标记兜底）
                if (UseCache)
                {
                    var marker = await _cache.ReadAsync<AuthRevokeMarker>(AccountRevokeKey(payload.Account));
                    if (marker != null && session.F_CreateTime.HasValue &&
                        session.F_CreateTime.Value <= marker.Time)
                    {
                        await RevokeSession(session);
                        return;
                    }
                }

                var now = DateTime.Now;

                if (sharedMode)
                {
                    // 共享模式：jti 签发后恒定，刷新是幂等的——只换访问令牌 + 滑动会话。
                    // 不轮换、不判重放、不吊销，多个持有同一登录态的客户端互不干扰。
                    if (!string.Equals(payload.Jti, session.F_RefreshJti, StringComparison.Ordinal))
                    {
                        // 不是本会话签发的刷新令牌：只拒绝本次，绝不吊销会话。
                        // 旧实现会 RevokeSession，共享账号时一个客户端的旧令牌会让所有人一起掉线。
                        return;
                    }

                    SlideSession(session, now);
                    ApplyMaxLifetime(session, config);
                    await SaveSession(session);

                    result = BuildResult(session);
                    return;
                }

                var graceSeconds = config.JwtRefreshGraceSeconds;
                if (graceSeconds < 0)
                {
                    graceSeconds = 0;
                }

                if (string.Equals(payload.Jti, session.F_RefreshJti, StringComparison.Ordinal))
                {
                    // 正常轮换：旧 jti 保留一个宽限期，供并发请求/响应丢失时重放
                    var oldJti = session.F_RefreshJti;
                    session.F_PrevRefreshJti = oldJti;
                    session.F_PrevExpireTime = now.AddSeconds(graceSeconds);
                    session.F_RefreshJti = Guid.NewGuid().ToString("N");
                    SlideSession(session, now);
                    await SaveSession(session);

                    result = BuildResult(session);
                    if (graceSeconds > 0)
                    {
                        await SaveRotation(oldJti, result, graceSeconds);
                    }
                    return;
                }

                if (graceSeconds > 0 &&
                    string.Equals(payload.Jti, session.F_PrevRefreshJti, StringComparison.Ordinal) &&
                    session.F_PrevExpireTime.HasValue && session.F_PrevExpireTime.Value >= now)
                {
                    // 宽限期内重放：优先返回首次轮换产生的那一对令牌，保证多标签页拿到同一份
                    result = await _cache.ReadAsync<AuthTokenResult>(RotationKey(payload.Jti));
                    if (result != null)
                    {
                        return;
                    }

                    // 宽限期缓存丢失（例如多节点部署且使用内存缓存）：再轮换一次，保证本次请求可用
                    var oldJti = session.F_RefreshJti;
                    session.F_PrevRefreshJti = oldJti;
                    session.F_PrevExpireTime = now.AddSeconds(graceSeconds);
                    session.F_RefreshJti = Guid.NewGuid().ToString("N");
                    SlideSession(session, now);
                    await SaveSession(session);

                    result = BuildResult(session);
                    var ttl = (int)Math.Max(0, (session.F_PrevExpireTime.Value - now).TotalSeconds);
                    await SaveRotation(oldJti, result, ttl);
                    await SaveRotation(payload.Jti, result, ttl);
                    return;
                }

                // 既不匹配当前 jti，也不在宽限期内：判定为令牌重放。
                // 只拒绝本次刷新，不吊销整条会话——同一账号多端/多人共用登录态时，
                // 吊销会连坐把其他仍持有有效令牌的客户端一起踢下线。
                // 需要主动收回时走 RevokeAll(account) 或 Logout(refreshToken)。
            }, "auth_token_refresh");

            return result;
        }

        /// <summary>
        /// 注销会话。
        /// 注意：共享模式下多个客户端持有同一 sid，注销会把这一整条会话删掉（所有人一起下线）。
        /// 若需要各端独立登录/登出，应让每个客户端各自走 login/pc 拿独立 sid，而不是共用一份登录态。
        /// </summary>
        public async Task Logout(string refreshToken)
        {
            var decoded = _operator.Decode(refreshToken, AuthTokenConstants.RefreshType, true);
            if (!decoded.Success || decoded.Payload == null || string.IsNullOrEmpty(decoded.Payload.Sid))
            {
                return;
            }

            var sid = decoded.Payload.Sid;
            if (UseCache)
            {
                await _cache.RemoveAsync(SessionKey(sid));
            }
            else
            {
                await _sessionService.Delete(sid);
            }
        }

        /// <summary>
        /// 吊销指定账号的全部会话
        /// </summary>
        public async Task RevokeAll(string account)
        {
            if (string.IsNullOrEmpty(account))
            {
                return;
            }

            if (UseCache)
            {
                var hours = ConfigHelper.GetConfig().JwtExp > 0 ? ConfigHelper.GetConfig().JwtExp : 12;
                await _cache.WriteAsync(AccountRevokeKey(account),
                    new AuthRevokeMarker { Time = DateTime.Now },
                    TimeSpan.FromHours(hours + 1));
            }
            else
            {
                await _sessionService.RevokeAll(account);
            }
        }

        #region 会话存取

        private async Task<AuthSessionEntity> GetSession(string sid)
        {
            if (UseCache)
            {
                var entity = await _cache.ReadAsync<AuthSessionEntity>(SessionKey(sid));
                if (entity == null)
                {
                    return null;
                }
                if (!entity.F_ExpireTime.HasValue || entity.F_ExpireTime.Value <= DateTime.Now)
                {
                    await _cache.RemoveAsync(SessionKey(sid));
                    return null;
                }
                return entity;
            }

            var dbEntity = await _sessionService.Get(sid);
            if (dbEntity != null &&
                (!dbEntity.F_ExpireTime.HasValue || dbEntity.F_ExpireTime.Value <= DateTime.Now))
            {
                await _sessionService.Delete(sid);
                return null;
            }
            return dbEntity;
        }

        private async Task SaveSession(AuthSessionEntity entity)
        {
            if (UseCache)
            {
                var seconds = (entity.F_ExpireTime ?? DateTime.Now).Subtract(DateTime.Now).TotalSeconds;
                if (seconds <= 0)
                {
                    await _cache.RemoveAsync(SessionKey(entity.F_Id));
                    return;
                }
                await _cache.WriteAsync(SessionKey(entity.F_Id), entity, TimeSpan.FromSeconds(seconds));
                return;
            }

            await _sessionService.Save(entity);
        }

        private async Task RevokeSession(AuthSessionEntity session)
        {
            session.F_Revoked = 1;
            session.F_RevokedTime = DateTime.Now;
            await SaveSession(session);
        }

        private async Task SaveRotation(string jti, AuthTokenResult result, int ttlSeconds)
        {
            if (string.IsNullOrEmpty(jti) || result == null || ttlSeconds <= 0)
            {
                return;
            }
            await _cache.WriteAsync(RotationKey(jti), result, TimeSpan.FromSeconds(ttlSeconds));
        }

        #endregion

        #region 令牌生成

        private AuthTokenResult BuildResult(AuthSessionEntity session)
        {
            var config = ConfigHelper.GetConfig();
            var accessMinutes = config.JwtAccessExpMinutes;
            if (accessMinutes <= 0)
            {
                accessMinutes = 60;
            }

            var now = DateTime.Now;
            var accessExpire = now.AddMinutes(accessMinutes);
            var sessionExpire = session.F_ExpireTime ?? now.AddHours(config.JwtExp > 0 ? config.JwtExp : 12);

            var descriptor = new TokenDescriptor
            {
                Account = session.F_Account,
                UserId = session.F_UserId,
                UserName = session.F_UserName,
                TenantNo = session.F_TenantNo,
                LoginRandomCode = session.F_LoginRandomCode,
                Sid = session.F_Id,
                Jti = Guid.NewGuid().ToString("N"),
                Typ = AuthTokenConstants.AccessType,
                IssuedAt = now,
                ExpiresAt = accessExpire,
                LoginTime = session.F_CreateTime ?? now,
            };
            var accessToken = _operator.EncodeToken(descriptor);

            descriptor.Jti = session.F_RefreshJti;
            descriptor.Typ = AuthTokenConstants.RefreshType;
            descriptor.IssuedAt = now;
            descriptor.ExpiresAt = sessionExpire;
            var refreshToken = _operator.EncodeToken(descriptor);

            return new AuthTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = accessMinutes * 60,
                RefreshExpiresIn = (int)Math.Max(0, (sessionExpire - now).TotalSeconds),
            };
        }

        #endregion
    }
}
