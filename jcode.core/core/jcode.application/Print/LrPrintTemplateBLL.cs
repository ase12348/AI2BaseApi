using jcode.autofac;
using jcode.application;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.bll
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板
    /// </summary>
    public class LrPrintTemplateBLL : BLLBase, ILrPrintTemplateBLL, BLL
    {
        private readonly LrPrintTemplateService lrPrintTemplateService = new LrPrintTemplateService();
        
        #region 获取数据
        /// <summary>
        /// 获取打印设计-打印模板的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintTemplateEntity>> GetPageList(Pagination pagination, LrPrintTemplateEntity queryParams)
        {
            return lrPrintTemplateService.GetPageList(pagination, queryParams);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintTemplateEntity> GetEntity(string keyValue)
        {
            return lrPrintTemplateService.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取最新的模版版本（排除草稿）
        /// </summary>
        /// <param name="templateInfoId"></param>
        /// <returns></returns>
        public Task<LrPrintTemplateEntity> GetLastEntity(string templateInfoId)
        {
            return lrPrintTemplateService.GetLastEntity(templateInfoId);
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
            await lrPrintTemplateService.Delete(keyValue);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="templateInfoId">模版信息主键</param>
        /// <returns></returns>
        public async Task DeleteByInfo(string templateInfoId)
        {
            await lrPrintTemplateService.DeleteByInfo(templateInfoId);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintTemplateEntity entity)
        {
            await lrPrintTemplateService.SaveEntity(keyValue, entity);
        }
        #endregion
    }
}
