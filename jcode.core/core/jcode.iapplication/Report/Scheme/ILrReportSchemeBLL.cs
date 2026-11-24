using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期： 2022-05-05 10:40:00
    /// 描 述： 简洁报表
    /// </summary>
    public interface ILrReportSchemeBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取简洁报表的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrReportSchemeEntity>> GetList(LrReportSchemeEntity queryParams);

        /// <summary>
        /// 获取简洁报表的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrReportSchemeEntity>> GetPageList(Pagination pagination, LrReportSchemeEntity queryParams);

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrReportSchemeEntity> GetEntity(string keyValue);

        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <returns></returns>
        Task<ReportSchemeData> GetData(string keyValue);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task Delete(string keyValue);

        

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, LrReportSchemeEntity entity);

        

        #endregion
    }
}
