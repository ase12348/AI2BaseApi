using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导出设置
    /// </summary>
    public interface ExcelExportIBLL : IBLL
    {

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<IEnumerable<ExcelExportEntity>> GetPageList(Pagination pagination, string keyword, string moduleId);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        Task<IEnumerable<ExcelExportEntity>> GetList(string moduleId);
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ExcelExportEntity>> GetList();
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<ExcelExportEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">主键</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, ExcelExportEntity entity);
        #endregion
    }
}
