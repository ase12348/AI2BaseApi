using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-03-28 16:23:13
    /// 描 述： 日志接口配置
    /// </summary>
    public class LrOtherLogschemeBLL : BLLBase, ILrOtherLogschemeBLL,BLL
    {
        private readonly LrOtherLogschemeService lrOtherLogschemeService = new LrOtherLogschemeService();

        

        #region 获取数据
        /// <summary>
        /// 获取日志接口配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogschemeEntity>> GetList(LrOtherLogschemeEntity queryParams)
        {
            return lrOtherLogschemeService.GetList(queryParams);
        }

        /// <summary>
        /// 获取日志接口配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogschemeEntity>> GetPageList(Pagination pagination, LrOtherLogschemeEntity queryParams)
        {
            return lrOtherLogschemeService.GetPageList(pagination, queryParams);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrOtherLogschemeEntity> GetEntity(string keyValue)
        {
            return lrOtherLogschemeService.GetEntity(keyValue);
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
            await lrOtherLogschemeService.Delete(keyValue);
        }

        /// <summary>
        /// 删除@Des的实体根据外键
        /// <summary>
        /// <param name="key">关联键</param>
        /// <returns></returns>
        public Task DeleteRelateEntity(string key) {
            return lrOtherLogschemeService.DeleteRelate(key);
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrOtherLogschemeEntity entity)
        {
            
            await lrOtherLogschemeService.SaveEntity(keyValue, entity);
        }

        /// <summary>
        /// 保存子表数据
        /// <summary>
        /// <param name="key">外键值</param>
        /// <param name="list">数据集合</param>
        /// <returns></returns>
        public async Task SaveList(string key, IEnumerable<LrOtherLogschemeEntity> list) {
            await lrOtherLogschemeService.SaveList(key,list);
        }


        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            await lrOtherLogschemeService.Deletes(keyValues);
        }

        

        #endregion
    }
}
