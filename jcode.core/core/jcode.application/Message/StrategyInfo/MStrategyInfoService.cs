using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：消息策略
    /// </summary>
    public class MStrategyInfoService : ServiceBase
    {

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<MStrategyInfoEntity>> GetList()
        {
            return this.BaseRepository().FindAll<MStrategyInfoEntity>();
        }

        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<MStrategyInfoEntity>> GetPageList(Pagination pagination, string keyword)
        {
            var expression = LinqExtensions.True<MStrategyInfoEntity>();
            if (!string.IsNullOrEmpty(keyword)) {
                expression = expression.And(t => t.F_StrategyName.Contains(keyword) || t.F_StrategyCode.Contains(keyword));
            }
            return this.BaseRepository().FindList<MStrategyInfoEntity>(expression, pagination);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<MStrategyInfoEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<MStrategyInfoEntity>(keyValue);
        }
        /// <summary>
        /// 根据策略编码获取策略
        /// </summary>
        /// <param name="code">策略编码</param>
        /// <returns></returns>
        public Task<MStrategyInfoEntity> GetEntityByCode(string code)
        {
            return this.BaseRepository().FindEntity<MStrategyInfoEntity>(t=>t.F_StrategyCode == code);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await this.BaseRepository().Delete<MStrategyInfoEntity>(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, MStrategyInfoEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();

                await this.BaseRepository().Update(entity);
            }
            else
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();

                await this.BaseRepository().Insert(entity);
            }
        }

        #endregion
    }
}
