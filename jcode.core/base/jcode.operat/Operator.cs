using JWT.Algorithms;
using JWT.Builder;
using JWT.Exceptions;
using jcode.util;
using System;
using System.Security.Cryptography;

namespace jcode.operat
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：当前连接用户信息处理类
    /// </summary>
    public class Operator: IOperator
    {
        //private readonly ICache _cache;
        //public Operator(ICache cache) {
        //    _cache = cache;
        //}
        ///// <summary>
        ///// 判断是否登录
        ///// </summary>
        ///// <param name="account">账号</param>
        ///// <param name="token">token</param>
        ///// <returns></returns>
        //public async Task<bool> IsOnLine(string account,string token)
        //{
        //    try
        //    {
        //        string accountTmp = await _cache.ReadAsync<string>(token);

        //        if (account == accountTmp)
        //        {
        //            return true;
        //        }
        //        else {
        //            return false;
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}
        ///// <summary>
        ///// 添加登录者(保存一天时间)
        ///// </summary>
        ///// <param name="account">账号</param>
        ///// <returns></returns>
        //public async Task<string> AddLoginUser(string account) {
        //    string token = Guid.NewGuid().ToString();
        //    TimeSpan ts = new TimeSpan(1, 0, 0, 0);
        //    var list = await _cache.ReadAsync<List<string>>(account);
        //    if (list == null) {
        //        list = new List<string>();
        //    }
        //    list.Add(token);
        //    await _cache.WriteAsync(account, list, ts);
        //    await _cache.WriteAsync(token, account, ts);
        //    return token;
        //}

        ///// <summary>
        ///// 清除登录者信息
        ///// </summary>
        ///// <param name="account">登录者用户账号</param>
        ///// <returns></returns>
        //public async Task ClearLoginUser(string account)
        //{
        //    var list = await _cache.ReadAsync<List<string>>(account);
        //    if (list != null)
        //    {
        //        foreach (var token in list)
        //        {
        //            await _cache.RemoveAsync(token);
        //        }
        //        await _cache.RemoveAsync(account);
        //    }
        //}


        // private static readonly Fixture _fixture = new Fixture();


        /// <summary>
        /// 生成jwt令牌（访问令牌/刷新令牌共用）
        /// </summary>
        /// <param name="descriptor">令牌声明信息</param>
        /// <returns></returns>
        public string EncodeToken(TokenDescriptor descriptor)
        {
            if (descriptor == null)
            {
                throw new ArgumentNullException(nameof(descriptor));
            }

            var secret = ConfigHelper.GetConfig().JwtSecret;
            if (string.IsNullOrEmpty(secret))
            {
                throw new Exception("#NotLog#JwtSecret 未配置");
            }

            var issuedAt = descriptor.IssuedAt == default ? DateTime.Now : descriptor.IssuedAt;
            var expiresAt = descriptor.ExpiresAt == default ? issuedAt.AddHours(12) : descriptor.ExpiresAt;

            var jwt = new JwtBuilder()
                .WithAlgorithm(new HMACSHA256Algorithm())
                .WithSecret(secret)
                .AddClaim("iat", new DateTimeOffset(issuedAt).ToUnixTimeSeconds())
                .AddClaim("exp", new DateTimeOffset(expiresAt).ToUnixTimeSeconds())
                .AddClaim("typ", descriptor.Typ)
                .AddClaim("sid", descriptor.Sid)
                .AddClaim("jti", descriptor.Jti)
                .AddClaim("Account", descriptor.Account)
                .AddClaim("LoginRandomCode", descriptor.LoginRandomCode)
                .AddClaim("Time", descriptor.LoginTime ?? issuedAt);

            if (!string.IsNullOrEmpty(descriptor.UserId))
            {
                jwt.AddClaim("UserId", descriptor.UserId);
            }
            if (!string.IsNullOrEmpty(descriptor.UserName))
            {
                jwt.AddClaim("UserName", descriptor.UserName);
            }

            if (ConfigHelper.GetConfig().IsSass)
            {
                // 添加租户编码：优先使用显式传入的租户（刷新令牌场景上下文可能还未就绪）
                var tenantNo = descriptor.TenantNo;
                if (string.IsNullOrEmpty(tenantNo))
                {
                    tenantNo = ContextHelper.GetItem("TenantNo") as string;
                }
                if (!string.IsNullOrEmpty(tenantNo))
                {
                    jwt.AddClaim("TenantNo", tenantNo);
                }
            }

            return jwt.Encode();
        }

        /// <summary>
        /// 解密jwt令牌（要求 typ=access）
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns>TokenExpiredException 时间过期, SignatureVerificationException 签证不正确, TokenTypeException 令牌类型错误</returns>
        public string DecodeToken(string token = "")
        {
            string res = ContextHelper.GetItem("Token") as string;
            try
            {
                if (!string.IsNullOrEmpty(res))
                {
                    return res;
                }

                if (string.IsNullOrEmpty(token))
                {
                    token = RequestPermission.GetRequestToken;
                }

                if (string.IsNullOrEmpty(token))
                {
                    return res;
                }

                var result = Decode(token, AuthTokenConstants.AccessType);
                if (result.Success)
                {
                    res = result.Json;
                }
                else if (result.Error == AuthTokenConstants.ErrorExpired)
                {
                    res = AuthTokenConstants.ErrorExpired;
                }
                else if (result.Error == AuthTokenConstants.ErrorSignature)
                {
                    res = AuthTokenConstants.ErrorSignature;
                }
                else if (result.Error == AuthTokenConstants.ErrorTypeMismatch)
                {
                    res = AuthTokenConstants.ErrorTypeMismatch;
                }
            }
            catch (TokenExpiredException)
            {
                res = AuthTokenConstants.ErrorExpired;
            }
            catch (SignatureVerificationException)
            {
                res = AuthTokenConstants.ErrorSignature;
            }

            ContextHelper.SetItem("Token", res);

            return res;
        }

        /// <summary>
        /// 解析jwt令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="expectedTyp">期望的令牌类型（access/refresh），为空不校验</param>
        /// <param name="ignoreExpiration">是否忽略过期时间（仅用于解析租户号等不授权场景）</param>
        /// <returns></returns>
        public TokenDecodeResult Decode(string token, string expectedTyp = null, bool ignoreExpiration = false)
        {
            var result = new TokenDecodeResult();
            if (string.IsNullOrEmpty(token))
            {
                result.Error = "TokenMissing";
                return result;
            }

            try
            {
                var builder = new JwtBuilder()
                    .WithAlgorithm(new HMACSHA256Algorithm())
                    .WithSecret(ConfigHelper.GetConfig().JwtSecret);

                if (ignoreExpiration)
                {
                    // 仅跳过过期校验，签名仍然必须通过
                    builder = builder
                        .WithVerifySignature(true)
                        .WithValidationParameters(p => { p.ValidateExpirationTime = false; });
                }
                else
                {
                    builder = builder.MustVerifySignature();
                }

                var json = builder.Decode(token);
                var payload = json.ToObject<Payload>();
                if (payload == null)
                {
                    result.Error = "TokenInvalid";
                    return result;
                }

                if (!string.IsNullOrEmpty(expectedTyp) && payload.Typ != expectedTyp)
                {
                    result.Error = AuthTokenConstants.ErrorTypeMismatch;
                    return result;
                }

                result.Success = true;
                result.Json = json;
                result.Payload = payload;
            }
            catch (TokenExpiredException)
            {
                result.Error = AuthTokenConstants.ErrorExpired;
            }
            catch (TokenNotYetValidException)
            {
                result.Error = "TokenNotYetValidException";
            }
            catch (SignatureVerificationException)
            {
                result.Error = AuthTokenConstants.ErrorSignature;
            }
            catch (InvalidTokenPartsException)
            {
                result.Error = "TokenInvalid";
            }
            catch (Exception)
            {
                result.Error = "TokenInvalid";
            }

            return result;
        }
    }
}
