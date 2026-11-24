using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.23
    /// 描 述：数据表草稿
    /// </summary>
    public class DbDraftService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbDraftEntity>> GetList(string queryJson)
        {
            var queryParam = queryJson.ToJObject();
            string keyword = "";
            if (!queryParam["keyword"].IsEmpty())
            {
                keyword = queryParam["keyword"].ToString();
            }


            var exp = Expressionable.Create<DbDraftEntity>()
                .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Name.Contains(keyword) || t.F_Remark.Contains(keyword));

            return this.BaseRepository().FindList<DbDraftEntity>(exp.ToExpression());
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DbDraftEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity<DbDraftEntity>(keyValue);
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
            await this.BaseRepository().Delete<DbDraftEntity>(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DbDraftEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
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
