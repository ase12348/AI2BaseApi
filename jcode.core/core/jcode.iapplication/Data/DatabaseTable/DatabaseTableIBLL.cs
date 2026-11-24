using jcode.autofac;
using jcode.util;
using SqlSugar;
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
    /// 描 述：数据库表管理
    /// </summary>
    public interface DatabaseTableIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 数据表列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        List<DbTableInfo> GetTableList(string code, bool isCache = false);
        /// <summary>
        /// 获取数据库表字段信息
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        List<DbColumnInfo> GetColumnInfosByTableName(string code, string tableName, bool isCache = false);

        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <param name="field">字段</param>
        /// <param name="logic">逻辑</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        Task<DataTable> GetTableDataList(string code, string tableName, string field, string logic, string keyword, Pagination pagination);
        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        Task<DataTable> GetTableDataList(string code, string tableName);
        /// <summary>
        /// 给定查询语句查询字段
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="strSql">表名</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetSqlColName(string code, string strSql);

        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取导入表数据（库里全部物理表，不做归属过滤）
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <returns></returns>
        List<DbTableInfo> GetImportTableList(string code);

        /// <summary>
        /// 可以被导入数据表模型的表：宿主自有表不在其中
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <returns></returns>
        List<DbTableInfo> GetImportableTableList(string code);

        /// <summary>
        /// 判断数据表字段重复
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <param name="filedsJson">数据字段</param>
        /// <param name="isNotSass">是否关闭多租户</param>
        /// <returns></returns>
        Task<bool> ExistFiled(string keyValue, string tableName, string keyName, string filedsJson,bool isNotSass = false);
        #endregion
    }
}
