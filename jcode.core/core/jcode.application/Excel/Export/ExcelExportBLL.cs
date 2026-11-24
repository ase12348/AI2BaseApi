using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导出设置
    /// </summary>
    public class ExcelExportBLL : BLLBase, ExcelExportIBLL, BLL
    {
        private readonly ExcelExportService excelExportService = new ExcelExportService();

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetPageList(Pagination pagination, string keyword, string moduleId)
        {
            return excelExportService.GetPageList(pagination, keyword, moduleId);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetList(string moduleId)
        {
            return excelExportService.GetList(moduleId);
        }

        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetList() {
            return excelExportService.GetList();
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<ExcelExportEntity> GetEntity(string keyValue)
        {
            return excelExportService.GetEntity(keyValue);
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
            await excelExportService.DeleteEntity(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, ExcelExportEntity entity)
        {
            await excelExportService.SaveEntity(keyValue, entity);
        }

        #endregion
    }
}
