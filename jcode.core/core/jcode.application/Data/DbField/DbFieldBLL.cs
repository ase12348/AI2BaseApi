using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期： 2022-05-05 11:14:02
    /// 描 述： 数据表模型常用字段维护
    /// </summary>
    public class DbFieldBLL : BLLBase, DbFieldIBLL, BLL
    {
        private readonly DbFieldService lrBaseDbfieldService = new DbFieldService();


        #region 获取数据
        /// <summary>
        /// 获取数据表模型常用字段维护的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbFieldEntity>> GetList(DbFieldEntity queryParams)
        {
            return lrBaseDbfieldService.GetList(queryParams);
        }

        /// <summary>
        /// 获取数据表模型常用字段维护的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbFieldEntity>> GetPageList(Pagination pagination, DbFieldEntity queryParams)
        {
            return lrBaseDbfieldService.GetPageList(pagination, queryParams);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DbFieldEntity> GetEntity(string keyValue)
        {
            return lrBaseDbfieldService.GetEntity(keyValue);
        }



        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await lrBaseDbfieldService.Delete(keyValue);
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DbFieldEntity entity)
        {

            await lrBaseDbfieldService.SaveEntity(keyValue, entity);
        }



        #endregion
    }
}
