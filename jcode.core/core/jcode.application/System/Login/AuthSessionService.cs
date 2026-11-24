using jcode.iapplication;
using jcode.orm;
using jcode.util;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 登录会话数据访问：CacheType 非 Redis 时落库（表 lr_base_auth_session）
    /// </summary>
    public class AuthSessionService : ServiceBase
    {
        /// <summary>
        /// 会话表名
        /// </summary>
        public const string TableName = "lr_base_auth_session";

        // 按「租户 + 连接串」记录表是否已初始化，避免每次请求都探测表结构
        private static readonly ConcurrentDictionary<string, Lazy<bool>> InitFlags =
            new ConcurrentDictionary<string, Lazy<bool>>();

        // 实体声明为可空的列，用于发现旧版自动建表遗留的 NOT NULL 列
        private static readonly string[] NullableColumns = typeof(AuthSessionEntity).GetProperties()
            .Where(t => t.GetCustomAttributes(typeof(Column), true)
                .Any(a => ((Column)a).IsNullable))
            .Select(t => t.Name)
            .ToArray();

        private IRepository Repository()
        {
            var repo = BaseRepository();
            EnsureTable(repo);
            return repo;
        }

        /// <summary>
        /// 确保会话表存在（首次使用时按实体结构自动建表）
        /// </summary>
        private void EnsureTable(IRepository repo)
        {
            var key = $"{GetTenantNo()}|{repo.ConnString}";
            var lazy = InitFlags.GetOrAdd(key, _ => new Lazy<bool>(() =>
            {
                if (!repo.IsAnyTable(TableName))
                {
                    repo.InitTables<AuthSessionEntity>();
                    return true;
                }

                // 旧实体未声明可空，自动建表时字符串列都是 NOT NULL；管理租户没有租户号，
                // 登录写会话会报 Column 'F_TenantNo' cannot be null，这里让 CodeFirst 修正列可空性
                var columns = repo.GetColumnInfosByTableName(TableName);
                if (columns.Any(t => !t.IsNullable &&
                    NullableColumns.Contains(t.DbColumnName, StringComparer.OrdinalIgnoreCase)))
                {
                    repo.InitTables<AuthSessionEntity>();
                }
                return true;
            }, LazyThreadSafetyMode.ExecutionAndPublication));

            try
            {
                _ = lazy.Value;
            }
            catch
            {
                // 建表失败不缓存，下一次请求重试
                InitFlags.TryRemove(key, out _);
                throw;
            }
        }

        /// <summary>
        /// 获取会话
        /// </summary>
        public Task<AuthSessionEntity> Get(string id)
        {
            return Repository().FindEntityByKey<AuthSessionEntity>(id);
        }

        /// <summary>
        /// 保存会话（存在则更新）
        /// </summary>
        public async Task Save(AuthSessionEntity entity)
        {
            var repo = Repository();
            var exist = await repo.FindEntityByKey<AuthSessionEntity>(entity.F_Id);
            if (exist == null)
            {
                await repo.Insert(entity);
            }
            else
            {
                await repo.Update(entity);
            }
        }

        /// <summary>
        /// 删除会话
        /// </summary>
        public Task Delete(string id)
        {
            return Repository().Delete<AuthSessionEntity>(id);
        }

        /// <summary>
        /// 吊销指定账号的全部会话
        /// </summary>
        public Task RevokeAll(string account)
        {
            var now = DateTime.Now;
            return Repository().Update<AuthSessionEntity>(
                new { F_Revoked = 1, F_RevokedTime = now },
                t => t.F_Account == account && t.F_Revoked != 1);
        }

        /// <summary>
        /// 清理已过期/已吊销的会话
        /// </summary>
        public Task CleanExpired(DateTime now)
        {
            return Repository().Delete<AuthSessionEntity>(t => t.F_ExpireTime < now || t.F_Revoked == 1);
        }
    }
}
