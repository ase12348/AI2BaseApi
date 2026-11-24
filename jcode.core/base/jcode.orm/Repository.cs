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
    /// 日 期：2019.09.10
    /// 描 述：数据库接口类
    /// </summary>
    public class Repository : IRepository
    {
        #region 构造
        /// <summary>
        /// 数据库操作接口
        /// </summary>
        public IDataBase db;
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="idatabase"></param>
        public Repository(IDataBase idatabase)
        {
            this.db = idatabase;
        }
        #endregion

        /// <summary>
        /// 当前数据库连接
        /// </summary>
        public string ConnString { get { return db.ConnString; } }
        /// <summary>
        /// 当前数据库类型
        /// </summary>
        public SqlSugar.DbType DataType { get { return db.DataType; } }

        /// <summary>
        /// orm
        /// </summary>
        public SqlSugarClient ORM
        { 
            get {
                return db.Orm;
            }
        }

        #region 事务
        /// <summary>
        /// 开始事务
        /// </summary>
        /// <returns></returns>
        public IRepository BeginTrans() {
            db.BeginTrans();
            return this;
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="isBeginTransId"></param>
        public void Commit(bool isBeginTransId = false) {
            db.Commit(isBeginTransId);
        }
        /// <summary>
        /// 回滚
        /// </summary>
        /// <param name="isBeginTransId"></param>
        public void Rollback(bool isBeginTransId = false) {
            db.Rollback(isBeginTransId);
        }
        #endregion

        #region 执行sql语句
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql, List<SugarParameter> dbParameters = null) {
            return this.db.ExecuteSql(strSql, dbParameters);
        }
        
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql,int commandTimeOut, List<SugarParameter> dbParameters = null) {
            return this.db.ExecuteSql(strSql, commandTimeOut,dbParameters);
        }
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql, object param = null) {
            return this.db.ExecuteSql(strSql, param);
        }
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql, int commandTimeOut,object param = null) {
            return this.db.ExecuteSql(strSql,commandTimeOut, param);
        }
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteProc(string procName, List<SugarParameter> cmdParms = null)
        {
            return this.db.ExecuteProc(procName, cmdParms);
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteProc(string procName, int commandTimeOut,List<SugarParameter> cmdParms = null)
        {
            return this.db.ExecuteProc(procName,commandTimeOut, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<T> QueryProcOne<T>(string procName, List<SugarParameter> cmdParms = null) where T : class
        {
            return this.db.QueryProcOne<T>(procName, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<T> QueryProcOne<T>(string procName,int commandTimeOut, List<SugarParameter> cmdParms = null) where T : class
        {
            return this.db.QueryProcOne<T>(procName,commandTimeOut, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<List<T>> QueryProc<T>(string procName, List<SugarParameter> cmdParms = null) where T : class
        {
            return this.db.QueryProc<T>(procName, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<List<T>> QueryProc<T>(string procName, int commandTimeOut,List<SugarParameter> cmdParms = null) where T : class
        {
            return this.db.QueryProc<T>(procName,commandTimeOut, cmdParms);
        }


        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<DataTable> QueryProc(string procName, List<SugarParameter> cmdParms = null)
        {
            return this.db.QueryProc(procName, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<DataTable> QueryProc(string procName, int commandTimeOut,List<SugarParameter> cmdParms = null)
        {
            return this.db.QueryProc(procName, commandTimeOut,cmdParms);
        }
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<DataSet> QueryProcDataSet(string procName, List<SugarParameter> cmdParms = null) {
            return this.db.QueryProcDataSet(procName, cmdParms);
        }
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="cmdParms">参数</param>
        /// <returns></returns>
        public Task<DataSet> QueryProcDataSet(string procName,int commandTimeOut, List<SugarParameter> cmdParms = null) {
            return this.db.QueryProcDataSet(procName,commandTimeOut, cmdParms);
        }
        #endregion

        #region 对像实体 新增/修改/删除      
        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public Task<int> Insert<T>(T entity) where T : class, new()
        {
            return this.db.Insert(entity);
        }
        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public Task<int> InsertNotDiffLog<T>(T entity) where T : class, new()
        {
            return this.db.InsertNotDiffLog(entity);
        }
        
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <param name="useParameter">使用参数</param>
        /// <returns></returns>
        public Task Inserts<T>(List<T> entitys,bool useParameter = true) where T : class, new()
        {
            return this.db.Inserts(entitys, useParameter);
        }
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <returns></returns>
        public Task FastInserts<T>(List<T> entitys) where T : class, new() {
            return this.db.FastInserts(entitys);
        }
        /// <summary>
        /// 插入实体数据【自增主键返回主键值】--ADD BY SSY 20230821
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public Task<bool> InsertIdentity<T>(T entity) where T : class, new()
        {
            return this.db.InsertIdentity(entity);
        }
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entity">实体数据</param>
        /// <param name="isIgnoreNull">是否忽略Null值</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Update<T>(T entity, bool isIgnoreNull = false,bool isNotEnableDiffLog = false) where T : class, new()
        {
            return this.db.Update(entity, isIgnoreNull, isNotEnableDiffLog);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dto">数据</param>
        /// <param name="exp">条件</param>
        /// <returns></returns>
        public Task<int> Update<T>(object dto, Expression<Func<T, bool>> exp) where T : class, new()
        {
            return this.db.Update(dto, exp);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Updates<T>(List<T> entitys, bool isNotEnableDiffLog = false) where T : class, new() {
            return this.db.Updates(entitys, isNotEnableDiffLog);
        }

        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> FastUpdates<T>(List<T> entitys) where T : class, new() {
            return this.db.FastUpdates(entitys);
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObj">虚拟条件</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Delete<T>(T deleteObj) where T : class, new()
        {
            return this.db.Delete<T>(deleteObj);
        }
        /// <summary>
        /// 删除实体数据（通过主键）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValue">主键</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Delete<T>(dynamic primaryKeyValue) where T : class, new()
        {
            return this.db.Delete<T>(primaryKeyValue);
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObjs">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Deletes<T>(List<T> deleteObjs) where T : class, new() {
            return this.db.Deletes<T>(deleteObjs);
        }
        /// <summary>
        /// 删除实体数据【批量删除】（通过主键组）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValues">主键组</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Delete<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            return this.db.Delete<T>(primaryKeyValues);
        }
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="exp">条件</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public Task<int> Delete<T>(Expression<Func<T, bool>> exp,bool isNotEnableDiffLog = false) where T : class, new()
        {
            return this.db.Delete<T>(exp,isNotEnableDiffLog);
        }
        #endregion

        #region 对象实体查询
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="keyValue">主键</param>
        /// <typeparam name="T">类</typeparam>
        public Task<T> FindEntityByKey<T>(object keyValue,bool noLock = false) where T : class {
            return this.db.FindEntityByKey<T>(keyValue,noLock);
        }
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">类</typeparam>
        public Task<T> FindEntity<T>(string sql, object param = null) where T : class {
            return this.db.FindEntity<T>(sql, param);
        }

        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="exp">参数</param>
        /// <param name="orderSql">排序sql语句</param>
        /// <typeparam name="T">类</typeparam>
        public Task<T> FindEntity<T>(Expression<Func<T, bool>> exp, string orderSql = null,bool noLock = false) where T : class
        {
            orderSql = DBCommonHelper.SqlFilters(orderSql);
            return this.db.FindEntity<T>(exp, orderSql,noLock);
        }

        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="sql">sql语句.</param>
        /// <param name="param">参数.</param>
        /// <typeparam name="T">类型.</typeparam>
        public Task<IEnumerable<T>> FindList<T>(string sql, object param = null) where T : class {
            return this.db.FindList<T>(sql, param);
        }


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
        public Task<IEnumerable<T>> FindList<T>(string sql, object param, Pagination pagination, string whereSql = null, object whereParam = null) where T : class, new()
        {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            whereSql = DBCommonHelper.SqlFilters2(whereSql);
            return this.db.FindList<T>(sql,param, pagination, whereSql, whereParam);
        }


        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="exp">条件.</param>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <typeparam name="T">类型.</typeparam>
        // 改进建议 2.2：多条件动态查询请改用 FindList(sql, param, pagination) 原生重载（sql + param 直接参数化）。
        // 本重载的 whereSql 只过 SqlFilters2，whereParam 会按属性名内联；若 whereSql 里的 @占位符与 whereParam 对不上，
        // 底层会把未匹配参数内联成字面量 NULL，产生 MySQL 语法错 / 条件永远不生效的静默错误。
        // 故在过滤前先做占位符对齐校验（DBCommonHelper.ValidateWhereParameters），缺参即抛，避免静默错。
        public Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, string orderSql = null, string whereSql = null, object whereParam = null,bool noLock = false) where T : class
        {
            DBCommonHelper.ValidateWhereParameters(whereSql, whereParam);
            orderSql = DBCommonHelper.SqlFilters(orderSql);
            whereSql = DBCommonHelper.SqlFilters2(whereSql);
            return this.db.FindList(exp, orderSql,whereSql,whereParam,noLock);
        }

        /// <summary>
        /// 获取数据列表（分页）---数据权限传参
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="exp">条件</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="isQuery">是否开启高级查询</param>
        /// <returns></returns>
        // 改进建议 2.2：多条件动态查询请改用 FindList(sql, param, pagination) 原生重载（sql + param 直接参数化）。
        // 本重载与上面的 orderSql 重载共用 DBCommonHelper.ValidateWhereParameters 做占位符对齐校验：
        // whereSql 有 @占位符却在 whereParam 中找不到同名参数时直接抛 ArgumentException，
        // 避免底层把未匹配参数内联成字面量 NULL（静默错 / MySQL 语法错）；无占位符或全参数化调用不受影响。
        public Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, Pagination pagination, string whereSql = null, object whereParam = null,bool noLock = false) where T : class, new()
        {
            DBCommonHelper.ValidateWhereParameters(whereSql, whereParam);
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            whereSql = DBCommonHelper.SqlFilters2(whereSql);
            return this.db.FindList<T>(exp, pagination, whereSql, whereParam,noLock);
        }
        
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
        /// <returns></returns>
        public Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination,DateTime beginTime, DateTime endTime, string whereSql = null, object whereParam = null,bool noLock = false) where T : class, new()
        {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            whereSql = DBCommonHelper.SqlFilters2(whereSql);
            return this.db.FindListBySplitTable<T>(exp, pagination,beginTime,endTime, whereSql, whereParam,noLock);
        }
        
        /// <summary>
        /// 获取数据列表（分页）---数据权限传参
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="exp">条件</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination, string whereSql = null, object whereParam = null,bool noLock = false) where T : class, new()
        {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            whereSql = DBCommonHelper.SqlFilters2(whereSql);
            return this.db.FindListBySplitTable<T>(exp, pagination, whereSql, whereParam,noLock);
        }
        
        /// <summary>
        /// 获取数据列表（分页）
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="pagination">分页参数</param>
        /// <param name="filter">过滤条件</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> FindList<T>(Pagination pagination, string filter,bool noLock = false) where T : class
        {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            return this.db.FindList<T>(pagination, filter,noLock);
        }
        /// <summary>
        /// 获取表的所有数据
        /// </summary>
        /// <returns>列表数据</returns>
        /// <typeparam name="T">类</typeparam>
        /// <param name="orderSql">排序sql语句</param>
        public Task<IEnumerable<T>> FindAll<T>(string orderSql = null,bool noLock = false) where T : class {
            orderSql = DBCommonHelper.SqlFilters(orderSql);
            orderSql = DBCommonHelper.SqlFilters(orderSql);
            return this.db.FindAll<T>(orderSql,noLock);
        }

        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <param name="pagination">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func, Pagination pagination) where T : class, new()
        {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            return this.db.FindListByQueryable(func, pagination);
        }

        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <returns></returns>
        public Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func) where T : class, new() {
            return this.db.FindListByQueryable(func);
        }
        #endregion

            #region 数据源查询
            /// <summary>
            /// 查询方法，返回datatable
            /// </summary>
            /// <returns>datatable数据</returns>
            /// <param name="sql">sql语句</param>
            /// <param name="param">参数</param>
        public Task<DataTable> FindTable(string sql, object param = null) {
            return this.db.FindTable(sql,param);
        }
        /// <summary>
        /// 分页查询方法，返回datatable
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="whereParam">查询参数</param>
        /// <returns></returns>
        public Task<DataTable> FindTable(string sqlStr, object sqlparam, Pagination pagination, string whereStr = null, object whereParam=null) {
            pagination.sidx = DBCommonHelper.SqlFiltersToSidx(pagination.sidx);
            pagination.sord = DBCommonHelper.SqlFiltersToSidx(pagination.sord);
            return this.db.FindTable(sqlStr, sqlparam, pagination, whereStr, whereParam);
        }
        /// <summary>
        /// 查询方法，返回datatable----CHANGE BY SSY 20231008
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="param">sql参数</param>
        /// <param name="orderSql">排序字段</param>
        /// <returns></returns>
        public Task<DataTable> FindTable(string sqlStr, string whereStr, string orderSql, object param = null)
        {
            return this.db.FindTable(sqlStr, whereStr, orderSql, param);
        }
        #endregion

        #region 获取数据库表信息
        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <returns>数据库表信息</returns>
        /// <param name="isCache">是否读取存储区</param>
        public List<DbTableInfo> GetTableInfoList(bool isCache = false) {
            return this.db.GetTableInfoList(isCache);
        }

        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public bool IsAnyTable(string tableName, bool isCache = false)
        {
            return this.db.IsAnyTable(tableName, isCache);
        }

        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = false) {
            return this.db.GetColumnInfosByTableName(tableName, isCache);
        }
        /// <summary>
        /// 获取数据库表主键信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<string> GetPrimaries(string tableName)
        {
            return this.db.GetPrimaries(tableName);
        }
        /// <summary>
        /// 获取数据库地址信息
        /// </summary>
        /// <returns>数据库地址信息</returns>
        public string GetDataSource() {
            return this.db.GetDataSource();
        }
        /// <summary>
        /// 测试数据库是否能链接成功
        /// </summary>
        /// <returns>The connection.</returns>
        public bool TestConnection() {
            return this.db.TestConnection();
        }

        /// <summary>
        /// 获取sql语句的字段
        /// </summary>
        /// <param name="sqlStr">查询语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetSqlColName(string sqlStr, string whereStr = null) {
            return this.db.GetSqlColName(sqlStr, whereStr);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteInsert(string tableName, List<SugarParameter> dbParameters) {
            return this.db.ExecuteInsert(tableName, dbParameters);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="pkey">主键名</param>
        /// <returns></returns>
        public Task<int> ExecuteUpdate(string tableName, List<SugarParameter> dbParameters, string pkey){
            return this.db.ExecuteUpdate(tableName, dbParameters, pkey);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="whereSql">条件</param>
        /// <returns></returns>
        public Task<int> ExecuteUpdateByWhere(string tableName, List<SugarParameter> dbParameters,
            string whereSql)
        {
            return this.db.ExecuteUpdateByWhere(tableName, dbParameters, whereSql);
        }

        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteDelete(string tableName, List<SugarParameter> dbParameters)
        {
            return this.db.ExecuteDelete(tableName, dbParameters);
        }
        #endregion

        #region 数据库表操作
        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        public bool CreateDatabase(string databaseDirectory = null) {
            return this.db.CreateDatabase(databaseDirectory);
        }

        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        public bool CreateDatabase(string databaseName, string databaseDirectory = null) {
            return this.db.CreateDatabase(databaseName,databaseDirectory);
        }
        /// <summary>
        /// 创建表
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        public void InitTables<T>() {
            this.db.InitTables<T>();
        }
        
        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        /// <returns></returns>
        public TypeModel GetTypeBilder(EntitiesGen genentity, bool isDeleteOldField = false){
            return this.db.GetTypeBilder(genentity,isDeleteOldField);
        }
        
        /// <summary>
        /// 创建或更新数据库表
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        public void CreateOrUpdateTable(EntitiesGen genentity, bool isDeleteOldField = false){
            this.db.CreateOrUpdateTable(genentity,isDeleteOldField);
        }

        /// <summary>
        /// 回收列：只回收调用方逐条确认过的列，主键列与自增列一律拒绝
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnNames">确认过要回收的列名</param>
        public void DropColumns(string tableName, IEnumerable<string> columnNames){
            this.db.DropColumns(tableName, columnNames);
        }
        #endregion

        #region 多租户
        /// <summary>
        /// 开启租户模式
        /// </summary>
        public void OpenTenant()
        {
            this.db.OpenTenant();
        }
        /// <summary>
        /// 关闭租户模式
        /// </summary>
        public void ColseTenant()
        {
            this.db.ColseTenant();
        }
        #endregion
    }
}
