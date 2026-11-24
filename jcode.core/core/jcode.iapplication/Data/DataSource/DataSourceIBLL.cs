using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据源
    /// </summary>
    public interface DataSourceIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">关键字</param>
        /// <param name="category">分类</param>
        /// <param name="appId">所属应用主键（为空表示不过滤）</param>
        /// <returns></returns>
        Task<IEnumerable<DataSourceEntity>> GetPageList(Pagination pagination, string keyword, string category, string appId = null);
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DataSourceEntity>> GetList();
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="code">编号</param>
        /// <returns></returns>
        Task<DataSourceEntity> GetEntityByCode(string code);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataSourceEntity">数据源实体</param>
        /// <returns></returns>
        Task<bool> SaveEntity(string keyValue, DataSourceEntity dataSourceEntity);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取数据源的数据
        /// </summary>
        /// <param name="code">数据源编码</param>
        /// <param name="sidx">排序字段</param>
        /// <param name="paramsJson">查询条件集合</param>
        /// <returns></returns>
        Task<DataTable> GetDataTable(string code,string sidx, string paramsJson = "");
        /// <summary>
        /// 获取数据源的数据(分页)
        /// </summary>
        /// <param name="code">数据源编码</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="paramsJson">查询条件</param>
        /// <param name="field">字段</param>
        /// <param name="values">值</param>
        /// <param name="whereSql">条件语句</param>
        /// <returns></returns>
        Task<DataTable> GetDataTable(string code, Pagination pagination, string paramsJson, string field, string values,
            string whereSql = null);
        /// <summary>
        /// 获取数据源的数据(分页)
        /// </summary>
        /// <param name="entity">数据源信息</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="paramsJson">查询条件</param>
        /// <param name="whereSql">条件语句</param>
        /// <returns></returns>
        Task<DataTable> GetDataTable(DataSourceEntity entity, Pagination pagination, string paramsJson, string whereSql = null);
        
        /// <summary>
        /// 获取数据源的数据（根据指定字段和值）
        /// </summary>
        /// <param name="code">数据源编码</param>
        /// <param name="field">字段</param>
        /// <param name="values">值</param>
        /// <param name="paramsJson">值</param>
        /// <returns></returns>
        Task<DataTable> GetDataTableByValues(string code, string field, string values,string paramsJson = "");
        /// <summary>
        /// 获取sql的列
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDataColName(string code);
        /// <summary>
        /// 获取sql的列
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDataColName(string dbCode, string sql);

        /// <summary>
        /// 获取视图列信息
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<List<DataSourceColumns>> GetViewColumns(string code);

        /// <summary>
        /// 获取列信息
        /// </summary>
        /// <param name="dt">数据集合</param>
        /// <returns></returns>
        List<DataSourceColumns> GetDataColumnsInfo(DataTable dt);
        #endregion
        
        #region 扩展方法
        /// <summary>
        /// 将datatable里的数据转换成显示值通过数据源
        /// </summary>
        /// <param name="dt">数据表</param>
        /// <param name="cols">需要转化的列</param>
        /// <returns></returns>
        Task<DataTable> ConvertDataTable(DataTable dt, List<DataSourceCol> cols);
        #endregion
        
        
    }
}
