using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.23
    /// 描 述：数据表草稿
    /// </summary>
    public class DbDraftBLL : BLLBase, DbDraftIBLL, BLL
    {
        private readonly DbDraftService dbDraftService = new DbDraftService();

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="queryJson">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbDraftEntity>> GetList(string queryJson)
        {
            return dbDraftService.GetList(queryJson);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DbDraftEntity> GetEntity(string keyValue)
        {
            return dbDraftService.GetEntity(keyValue);
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
            await dbDraftService.DeleteEntity(keyValue);
        }


        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DbDraftEntity entity)
        {
            await dbDraftService.SaveEntity(keyValue, entity);
        }

        #endregion
    }
}
