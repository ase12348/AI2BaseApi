using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jcode.orm
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.05
    /// 描 述：数据库接口类
    /// </summary>
    public interface IDataBase
    {
        /// <summary>
        /// 当前数据库连接
        /// </summary>
        string ConnString { get; }
        /// <summary>
        /// 当前数据库类型
        /// </summary>
        SqlSugar.DbType DataType { get; }
        /// <summary>
        /// orm操作对象
        /// </summary>
        SqlSugarClient Orm { get; }

        #region 事务
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>The trans.</returns>
        IDataBase BeginTrans();
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="isBeginTransId"></param>
        void Commit(bool isBeginTransId = false);

        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        /// <param name="isBeginTransId"></param>
        void Rollback(bool isBeginTransId = false);
        #endregion

        #region 执行sql语句
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteSql(string strSql, List<SugarParameter> dbParameters = null);
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteSql(string strSql, int commandTimeOut, List<SugarParameter> dbParameters = null);
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteSql(string strSql, object parameters);
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteSql(string strSql, int commandTimeOut, object parameters);
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        Task<int> ExecuteProc(string procName, List<SugarParameter> cmdParms = null);
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteProc(string procName, int commandTimeOut, List<SugarParameter> dbParameters = null);
        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        Task<T> QueryProcOne<T>(string procName, List<SugarParameter> cmdParms = null) where T : class;
        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<T> QueryProcOne<T>(string procName, int commandTimeOut, List<SugarParameter> dbParameters = null)
            where T : class;
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        Task<List<T>> QueryProc<T>(string procName, List<SugarParameter> cmdParms = null) where T : class;
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<List<T>> QueryProc<T>(string procName, int commandTimeOut, List<SugarParameter> dbParameters = null)
            where T : class;

        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        Task<DataTable> QueryProc(string procName, List<SugarParameter> cmdParms = null);
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<DataTable> QueryProc(string procName, int commandTimeOut, List<SugarParameter> dbParameters = null);
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<DataSet> QueryProcDataSet(string procName, List<SugarParameter> dbParameters = null);
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<DataSet> QueryProcDataSet(string procName, int commandTimeOut,
            List<SugarParameter> dbParameters = null);
        #endregion

        #region 对像实体 新增/修改/删除      
        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task<int> Insert<T>(T entity) where T : class, new();

        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task<int> InsertNotDiffLog<T>(T entity) where T : class, new();
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <param name="useParameter">使用参数</param>
        /// <returns></returns>
        Task Inserts<T>(List<T> entitys,bool useParameter = true) where T : class, new();
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <returns></returns>
        Task FastInserts<T>(List<T> entitys) where T : class, new();
        /// <summary>
        /// 插入实体数据【自增主键返回主键值】--ADD BY SSY 20230821
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task<bool> InsertIdentity<T>(T entity) where T : class, new();
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entity">实体数据</param>
        /// <param name="isIgnoreNull">是否忽略Null值</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Update<T>(T entity, bool isIgnoreNull,bool isNotEnableDiffLog = false) where T : class, new();
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dto">数据</param>
        /// <param name="exp">条件</param>
        /// <returns></returns>
        Task<int> Update<T>(object dto, Expression<Func<T, bool>> exp) where T : class, new();


        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Updates<T>(List<T> entitys, bool isNotEnableDiffLog) where T : class, new();
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> FastUpdates<T>(List<T> entitys) where T : class, new();
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObj">虚拟条件</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Delete<T>(T deleteObj) where T : class, new();
        /// <summary>
        /// 删除实体数据（通过主键）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValue">主键</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Delete<T>(dynamic primaryKeyValue) where T : class, new();

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObjs">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Deletes<T>(List<T> deleteObjs) where T : class, new();

        /// <summary>
        /// 删除实体数据【批量删除】（通过主键组）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValues">主键组</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Delete<T>(dynamic[] primaryKeyValues) where T : class, new();
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="exp">条件</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        Task<int> Delete<T>(Expression<Func<T, bool>> exp,bool isNotEnableDiffLog = false) where T : class, new();
        #endregion

        #region 对象实体查询
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="pkValue">主键</param>
        /// <param name="noLock">是否脏读</param>
        /// <typeparam name="T">类</typeparam>
        Task<T> FindEntityByKey<T>(object pkValue,bool noLock = false) where T : class;
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">类</typeparam>
        Task<T> FindEntity<T>(string sql, object param = null) where T : class;
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="exp">参数</param>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="noLock">是否脏读</param>
        /// <typeparam name="T">类</typeparam>
        Task<T> FindEntity<T>(Expression<Func<T, bool>> exp, string orderSql = null,bool noLock = false) where T : class;

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="sql">sql语句.</param>
        /// <param name="param">参数.</param>
        /// <typeparam name="T">类型.</typeparam>
        Task<IEnumerable<T>> FindList<T>(string sql, object param = null) where T : class;

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="exp">条件.</param>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="noLock">是否脏读</param>
        /// <typeparam name="T">类型.</typeparam>
        Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, string orderSql = null, string whereSql = null, object whereParam = null,bool noLock = false) where T : class;

        /// <summary>
        /// 获取数据列表（分页）
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">参数</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindList<T>(string sql, object param, Pagination pagination, string whereSql = null, object whereParam = null) where T : class, new();
        /// <summary>
        /// 获取数据列表（分页）
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="pagination">分页参数</param>
        /// <param name="filter">过滤条件</param>
        /// <param name="noLock">是否脏读</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindList<T>(Pagination pagination, string filter,bool noLock = false) where T : class;
        /// <summary>
        /// 获取数据列表（分页）---数据权限传参
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="exp">条件</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="noLock">是否脏读</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, Pagination pagination, string whereSql = null, object whereParam = null,bool noLock = false) where T : class, new();

        /// <summary>
        /// 获取数据列表（分页）---数据权限传参
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="exp">条件</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="beginTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="noLock">是否脏读</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination,
            DateTime beginTime, DateTime endTime, string whereSql = null, object whereParam = null,bool noLock = false)
            where T : class, new();
        


       /// <summary>
       /// 获取数据列表（分页）---数据权限传参
       /// </summary>
       /// <typeparam name="T">类</typeparam>
       /// <param name="exp">条件</param>
       /// <param name="pagination">分页参数</param>
       /// <param name="whereSql">查询语句</param>
       /// <param name="whereParam">查询参数</param>
       /// <param name="noLock">是否脏读</param>
       /// <returns></returns>
       Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination,
           string whereSql = null, object whereParam = null,bool noLock = false) where T : class, new();
        

        
        /// <summary>
        /// 获取表的所有数据
        /// </summary>
        /// <returns>列表数据</returns>
        /// <typeparam name="T">类</typeparam>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="noLock">是否脏读</param>
        Task<IEnumerable<T>> FindAll<T>(string orderSql = null,bool noLock = false) where T : class;




        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <param name="pagination">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func, Pagination pagination) where T : class, new();
        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <returns></returns>
        Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func) where T : class, new();
        #endregion

        #region 数据源查询
        /// <summary>
        /// 查询方法，返回datatable
        /// </summary>
        /// <returns>datatable数据</returns>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        Task<DataTable> FindTable(string sql, object param = null);
        /// <summary>
        /// 分页查询方法，返回datatable
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        Task<DataTable> FindTable(string sqlStr, object sqlparam,Pagination pagination,string whereStr = null, object whereParam = null);
        /// <summary>
        /// 查询方法，返回datatable----CHANGE BY SSY 20231008
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="orderSql">排序字段</param>
        /// <returns></returns>
        Task<DataTable> FindTable(string sqlStr, string whereStr, string orderSql, object sqlparam = null);
        #endregion

        #region 获取数据库表信息
        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <returns>数据库表信息</returns>
        /// <param name="isCache">是否读取存储区</param>
        List<DbTableInfo> GetTableInfoList(bool isCache = false);
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        bool IsAnyTable(string tableName, bool isCache = false);
        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = false);
        /// <summary>
        /// 获取数据库表主键信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        List<string> GetPrimaries(string tableName);
        /// <summary>
        /// 获取数据库地址信息
        /// </summary>
        /// <returns>数据库地址信息</returns>
        string GetDataSource();
        /// <summary>
        /// 测试数据库是否能链接成功
        /// </summary>
        /// <returns></returns>
        bool TestConnection();
        /// <summary>
        /// 获取sql语句的字段
        /// </summary>
        /// <param name="sqlStr">查询语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetSqlColName(string sqlStr, string whereStr = null);

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteInsert(string tableName, List<SugarParameter> dbParameters);

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="pkey">主键名</param>
        /// <returns></returns>
        Task<int> ExecuteUpdate(string tableName, List<SugarParameter> dbParameters, string pkey);
        
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="whereSql">主键名</param>
        /// <returns></returns>
        Task<int> ExecuteUpdateByWhere(string tableName, List<SugarParameter> dbParameters, string whereSql);

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        Task<int> ExecuteDelete(string tableName, List<SugarParameter> dbParameters);

        #endregion

        #region 数据库表操作
        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        bool CreateDatabase(string databaseDirectory = null);
        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        bool CreateDatabase(string databaseName, string databaseDirectory = null);
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        void InitTables<T>();

        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        /// <returns></returns>
        TypeModel GetTypeBilder(EntitiesGen genentity, bool isDeleteOldField = false);
        /// <summary>
        /// 创建或更新数据库表
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        void CreateOrUpdateTable(EntitiesGen genentity, bool isDeleteOldField = false);

        /// <summary>
        /// 回收列：只回收调用方逐条确认过的列，主键列与自增列一律拒绝
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnNames">确认过要回收的列名</param>
        void DropColumns(string tableName, IEnumerable<string> columnNames);
        #endregion

        #region 多租户
        /// <summary>
        /// 开启租户模式
        /// </summary>
        void OpenTenant();
        /// <summary>
        /// 关闭租户模式
        /// </summary>
        void ColseTenant();
        #endregion
    }
}
