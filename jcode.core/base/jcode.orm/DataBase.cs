using jcode.util;
using jcode.util.NLog;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using ClickHouse.Client.ADO;
using Microsoft.Extensions.Primitives;
using Npgsql;
using SqlSugar.ClickHouse;
using SqlSugar.TDengine;
using SqlSugar.TDengineAdo;
using SqlSugar.GBase;
using SqlSugar.SplitTableExtensions;
using DbType = SqlSugar.DbType;

namespace jcode.orm
{
    /// <summary>
    /// 数据库底层实现
    /// 实体如果用了pgsql的地图字段请 [Newtonsoft.Json.JsonConverter(typeof(NetTopologySuite.IO.Converters.GeometryConverter))]
    /// </summary>
    public class DataBase: IDataBase
    {
        /// <summary>
        /// 判断事务是否启动
        /// </summary>
        private bool isOpenTrans = false;
        /// <summary>
        /// 当前数据库连接
        /// </summary>
        public string ConnString { get { return _connString; } }
        /// <summary>
        /// 当前数据库类型
        /// </summary>
        public DbType DataType { get { return _dataType; } }

        /// <summary>
        /// 当前数据库操作对象
        /// </summary>
        public SqlSugarClient Orm { get { return _orm; } }
        

        /// <summary>
        /// 当前数据库连接
        /// </summary>
        private string _connString { get; set; }
        /// <summary>
        /// 当前数据库类型
        /// </summary>
        private DbType _dataType { get; set; }
        /// <summary>
        /// 当前数据库操作对象
        /// </summary>
        private SqlSugarClient _orm { get; set; }
        /// <summary>
        /// 是否启用租户查询
        /// </summary>
        private bool _isTenant { get; set; }
        /// <summary>
        /// 开启ORM文件日志记录
        /// </summary>
        private bool _isEnableDiffLog { get; set; }

        private bool _isReplaceTenant { get; set; }
        /// <summary>
        /// PostgreSQL数据库是否强制转小写，默认true
        /// </summary>
        private bool _pgSqlIsAutoToLower { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataType"></param>
        /// <param name="connString"></param>
        /// <param name="isSlave"></param>
        /// <param name="slaveConnectionConfigs"></param>
        /// <param name="isTenant"></param>
        public DataBase(DbType dataType, string connString,bool isSlave,List<SlaveConnectionConfig> slaveConnectionConfigs, bool isTenant = true)
        {
            _connString = connString;
            _dataType = dataType;

            _isReplaceTenant = true;
            if (ConfigHelper.GetConfig().IsEnableDiffLog)
            {
                _isEnableDiffLog = true;
            }
            else
            {
                _isEnableDiffLog = false;
            }

            if (ConfigHelper.GetConfig().IsSass && ConfigHelper.GetConfig().IsSassTable)
            { 
                _isTenant = isTenant; 
            }
            else{
                _isTenant = false;
            }


   
            
            //处理PgSql的大小写问题
            _pgSqlIsAutoToLower = true;
            if (dataType== DbType.PostgreSQL)
            {
                var res = DBCommonHelper.GetPgSqlIsAutoToLower(connString);
                _connString = res._connString;
                if (!res.PgSqlIsAutoToLower)
                {
                    _pgSqlIsAutoToLower = false;
                }
                else
                {
                    _pgSqlIsAutoToLower = true;
                }
            }

            var dbconnectionConfig = new ConnectionConfig()
            {
                ConnectionString = _connString, //连接符字串-主库
                DbType = _dataType,
                IsAutoCloseConnection = true, //不设成true要手动close
                InitKeyType = InitKeyType.Attribute,
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) =>
                    {
                        var attributes = property.GetCustomAttributes(true); //get all attributes 

                        if (attributes.Any(it =>
                                it is Column && (it as Column).IsPrimary)) // by attribute set primarykey
                        {
                            column.IsPrimarykey = true;
                        }

                        if (attributes.Any(it => it is Column && (it as Column).IsJson)) //是否为Json格式
                        {
                            column.IsJson = true;
                        }

                        if (attributes.Any(it => it is Column && (it as Column).IsIgnore)) //是否忽略
                        {
                            column.IsIgnore = true;
                        }

                        if (attributes.Any(it => it is Column && (it as Column).IsIdentity)) //是否自增
                        {
                            column.IsIdentity = true;
                        }

                        if (attributes.Any(it => it is Column && (it as Column).IsNullable)) //是否必填
                        {
                            column.IsNullable = true;
                        }

                        var item = (Column)attributes.FirstOrDefault(it =>
                            it is Column && !string.IsNullOrEmpty((it as Column).ColumnName));

                        if (item != null) //是否必填
                        {
                            column.DbColumnName = item.ColumnName;
                        }
                    },
                    EntityNameService = (type, entity) =>
                    {
                        var attributes = type.GetCustomAttributes(true);
                        if (attributes.Any(it => it is MyTableAttribute))
                        {
                            entity.DbTableName = (attributes.First(it => it is MyTableAttribute) as MyTableAttribute)
                                .Name;
                            entity.TableDescription =
                                (attributes.First(it => it is MyTableAttribute) as MyTableAttribute).TableDescription;
                        }
                    }
                },
                MoreSettings = new ConnMoreSettings()
                {
                    // IsWithNoLockQuery = _dataType == DbType.SqlServer? true:false,
                    PgSqlIsAutoToLower = _pgSqlIsAutoToLower, //增删查改支持驼峰表
                    PgSqlIsAutoToLowerCodeFirst = _pgSqlIsAutoToLower, // 建表建驼峰表。5.1.3.30 
                }

            };
            
            
            if (dataType== DbType.PostgreSQL)
            {
                _orm = new SqlSugarClient(dbconnectionConfig,db=>{
                    //配置使用这个类型
                    //连接由 SqlSugar 内部创建，无法挂到 NpgsqlDataSource 上，只能沿用已标记过时的全局映射
                    //全局映射必须在任何连接创建前注册，此处位于 SqlSugarClient 构造阶段，满足要求
#pragma warning disable CS0618
                    NpgsqlConnection.GlobalTypeMapper.UseNetTopologySuite();
#pragma warning restore CS0618
                });
            }
            else
            {
                _orm = new SqlSugarClient(dbconnectionConfig);
            }

            // 设置读写分离
            if (isSlave && slaveConnectionConfigs != null && slaveConnectionConfigs.Count > 0)
            {
                _orm.CurrentConnectionConfig.SlaveConnectionConfigs = slaveConnectionConfigs;
            }
            //_orm.Ado.IsDisableMasterSlaveSeparation = !isSlave;//是否全局禁止读写分离
            _orm.Ado.CommandTimeOut = 120;//设置超时时间，单位秒
            
            
            //添加Sql打印事件
            _orm.Aop.OnLogExecuting = (sql, pars) =>
            {
                Trace.WriteLine(sql);
                // Trace.WriteLine(pars);
            };
            // 添加sql改变事件
            _orm.Aop.OnExecutingChangeSql = (sql, pars) => //可以修改SQL和参数的值
            {
                if (this._isTenant)
                {
                    pars = TenantHelper.SetParams(pars);
                    sql = TenantHelper.SetSqlTenantId(sql);
                }
                else if(this._isReplaceTenant)
                {
                    sql = TenantHelper.SetSql(sql, pars);
                }

                if (_dataType == DbType.Oracle && pars != null)
                {
                    foreach (var item in pars)
                    {
                        if (item.ParameterName.IndexOf(":") == -1)
                        {
                            item.ParameterName = $":{item.ParameterName}";
                        }
                    }
                }
                
                return new KeyValuePair<string, SugarParameter[]>(sql, pars);
            };

            #region 差异日志记录
            //新增、修改、删除
            _orm.Aop.OnDiffLogEvent = it =>
            {
                string diffLogsStr = ContextHelper.GetItem("dbDiffLogs")?.ToString();
                var diffLogs = new List<DbDiffLog>();
                if (!string.IsNullOrEmpty(diffLogsStr))
                {
                    diffLogs = diffLogsStr.ToObject<List<DbDiffLog>>();
                }

                diffLogs.Add(new DbDiffLog()
                {
                    BeforeData = it.BeforeData,
                    AfterData = it.AfterData,
                    Time = it.Time,
                    DiffType = it.DiffType,
                });
                ContextHelper.SetItem("dbDiffLogs",diffLogs.ToJson());
            };
            #endregion
        }

        #region 事务
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>The trans.</returns>
        public IDataBase BeginTrans()
        {
            if (!isOpenTrans)
            {
                _orm.BeginTran();
                isOpenTrans = true;
            }
            return this;
        }
        /// <summary>
        /// 提交
        /// </summary>
        /// <param name="isBeginTransId"></param>
        public void Commit(bool isBeginTransId = false)
        {
            if (!isBeginTransId)
            {
                string _BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
                if (!string.IsNullOrEmpty(_BeginTransId))
                {
                    return;
                }
            }
            try
            {
                _orm.CommitTran();
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        /// <param name="isBeginTransId"></param>
        public void Rollback(bool isBeginTransId = false)
        {
            if (!isBeginTransId)
            {
                string _BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
                if (!string.IsNullOrEmpty(_BeginTransId))
                {
                    return;
                }
            }

            try
            {
                _orm.RollbackTran();
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region 执行sql语句
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql, List<SugarParameter> dbParameters = null)
        {
            try
            {
                strSql = TenantHelper.CheckTenantId(strSql, this._isTenant);
                strSql = DBCommonHelper.GetTranslationColumnNameBySql(strSql, _dataType,_pgSqlIsAutoToLower);
                return _orm.Ado.ExecuteCommandAsync(strSql, dbParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql,int commandTimeOut, List<SugarParameter> dbParameters = null)
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return ExecuteSql(strSql,dbParameters);
        }


        
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public async Task<int> ExecuteSql(string strSql, object parameters)
        {
            try
            {
                strSql =  TenantHelper.CheckTenantId(strSql, this._isTenant);
                strSql = DBCommonHelper.GetTranslationColumnNameBySql(strSql, _dataType, _pgSqlIsAutoToLower);
                return await _orm.Ado.ExecuteCommandAsync(strSql, parameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行sql语句(带参数)
        /// </summary>
        /// <param name="strSql">sql语句</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="parameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteSql(string strSql, int commandTimeOut,object parameters)
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return ExecuteSql(strSql,parameters);
        }
        #endregion

        #region 执行存储过程
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<int> ExecuteProc(string procName, List<SugarParameter> dbParameters = null)
        {
            try
            {
                TenantHelper.AddParam(dbParameters, this._isTenant);
                return await _orm.Ado.UseStoredProcedure().ExecuteCommandAsync(procName, dbParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<int> ExecuteProc(string procName,int commandTimeOut, List<SugarParameter> dbParameters = null)
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return ExecuteProc(procName,dbParameters);
        }

        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<T> QueryProcOne<T>(string procName, List<SugarParameter> dbParameters = null) where T : class
        {
            try
            {
                TenantHelper.AddParam(dbParameters, this._isTenant);
                return await _orm.Ado.UseStoredProcedure().SqlQuerySingleAsync<T>(procName, dbParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行存储过程(查询实体数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<T> QueryProcOne<T>(string procName,int commandTimeOut,  List<SugarParameter> dbParameters = null) where T : class
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return QueryProcOne<T>(procName,dbParameters);
        }


        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<List<T>> QueryProc<T>(string procName, List<SugarParameter> dbParameters = null) where T : class
        {
            try
            {
                TenantHelper.AddParam(dbParameters, this._isTenant);
                return await _orm.Ado.UseStoredProcedure().SqlQueryAsync<T>(procName, dbParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<List<T>> QueryProc<T>(string procName,int commandTimeOut,  List<SugarParameter> dbParameters = null) where T : class
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return QueryProc<T>(procName,dbParameters);
        }

        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<DataTable> QueryProc(string procName, List<SugarParameter> dbParameters = null)
        {
            try
            {
                TenantHelper.AddParam(dbParameters, this._isTenant);
                return DBCommonHelper.DtColToLow(await _orm.Ado.UseStoredProcedure().GetDataTableAsync(procName, dbParameters));
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<DataTable> QueryProc(string procName,int commandTimeOut,  List<SugarParameter> dbParameters = null)
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return QueryProc(procName,dbParameters);
        }


        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<DataSet> QueryProcDataSet(string procName, List<SugarParameter> dbParameters = null)
        {
            try
            {
                TenantHelper.AddParam(dbParameters, this._isTenant);
                return await _orm.Ado.UseStoredProcedure().GetDataSetAllAsync(procName, dbParameters);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 执行存储过程(获取列表数据)
        /// </summary>
        /// <param name="procName">存储过程名称</param>
        /// <param name="commandTimeOut">超时设置</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public Task<DataSet> QueryProcDataSet(string procName,int commandTimeOut,  List<SugarParameter> dbParameters = null)
        {
            _orm.Ado.CommandTimeOut = commandTimeOut;
            return QueryProcDataSet(procName,dbParameters);
        }
        #endregion

        #region 对像实体 新增/修改/删除      
        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<int> Insert<T>(T entity) where T : class, new()
        {
            try
            {
                var res = await _orm.Insertable(entity).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 插入实体数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<int> InsertNotDiffLog<T>(T entity) where T : class, new()
        {
            try
            {
                var res = await _orm.Insertable(entity).EnableDiffLogEventIF(false).ExecuteCommandAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <param name="useParameter">使用参数</param>
        /// <returns></returns>
        public async Task Inserts<T>(List<T> entitys,bool useParameter = true) where T : class, new()
        {
            try
            {
                await _orm.Utilities.PageEachAsync(entitys, 500, async (pageList) => {
                    if (useParameter)
                    {
                        await _orm.Insertable(pageList).UseParameter().ExecuteCommandAsync();
                    }
                    else {
                        this._isReplaceTenant = false;
                        await _orm.Insertable(pageList).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                    }
                   
                });
            }
            catch (Exception)
            {
                throw;
            }
            finally{
                this._isReplaceTenant = true;
            }
        }
        /// <summary>
        /// 插入实体数据集合
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entitys">实体数据集合</param>
        /// <returns></returns>
        public async Task FastInserts<T>(List<T> entitys) where T : class, new()
        {
            try
            {
                if (entitys.Count > 0)
                {
                    this._isReplaceTenant = false;
                    await _orm.Fastest<T>().BulkCopyAsync(entitys);
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this._isReplaceTenant = true;
            }
        }
        /// <summary>
        /// 插入实体数据【自增主键返回主键值】--ADD BY SSY 20230821
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task<bool> InsertIdentity<T>(T entity) where T : class, new()
        {
            try
            {
                var res = await _orm.Insertable(entity).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandIdentityIntoEntityAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entity">实体数据</param>
        /// <param name="isIgnoreNull">是否忽略Null值</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Update<T>(T entity, bool isIgnoreNull,bool isNotEnableDiffLog = false) where T : class, new()
        {
            try
            {
                var flag = _isEnableDiffLog;
                if (isNotEnableDiffLog)
                {
                    flag = false;
                }

                if (isIgnoreNull)
                {
                    var res = await _orm.Updateable(entity).IgnoreColumns(ignoreAllNullColumns: true).EnableDiffLogEventIF(flag).ExecuteCommandAsync();
                    return res;
                }
                else
                {
                    var res = await _orm.Updateable(entity).EnableDiffLogEventIF(flag).ExecuteCommandAsync();
                    return res;
                }

            }
            catch (Exception)
            {
                throw;
            }
        }




        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="dto">数据</param>
        /// <param name="exp">条件</param>
        /// <returns></returns>
        public async Task<int> Update<T>(object dto, Expression<Func<T, bool>> exp) where T : class, new()
        {
            this._isReplaceTenant = false;
            try
            {
                var dbUpdate = _orm.Updateable<T>(dto).IgnoreColumns(ignoreAllNullColumns: true).Where(exp);
                if (this._isTenant) {
                    dbUpdate = dbUpdate.WhereColumns("F_TenantId");
                }
                var res = await dbUpdate.EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();// 添加条件
                return res;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this._isReplaceTenant = true;
            }
        }


        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Updates<T>(List<T> entitys, bool isNotEnableDiffLog) where T : class, new()
        {
            this._isReplaceTenant = false;
            try
            {
                if (entitys.Count > 0)
                {
                    var flag = _isEnableDiffLog;
                    if (isNotEnableDiffLog)
                    {
                        flag = false;
                    }
                    
                    await _orm.Utilities.PageEachAsync(entitys, 500, async (pageList) => {
                        await _orm.Updateable(pageList).EnableDiffLogEventIF(flag).ExecuteCommandAsync();
                    });




                    /*if (isIgnoreNull)
                    {
                        var res = await _orm.Updateable(entitys).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync();
                        return res;
                    }
                    else
                    {
                        var res = await _orm.Updateable(entitys).ExecuteCommandAsync();
                        return res;
                    }*/
                }
                return 0;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this._isReplaceTenant = true;
            }
        }


        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="entitys">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> FastUpdates<T>(List<T> entitys) where T : class, new()
        {
            this._isReplaceTenant = false;
            try
            {
                if (entitys.Count > 0)
                {
                    await _orm.Fastest<T>().BulkUpdateAsync(entitys);
                }
                return 0;

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this._isReplaceTenant = true;
            }

        }
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObj">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Delete<T>(T deleteObj) where T : class, new()
        {
            try
            {
                var res = await _orm.Deleteable<T>(deleteObj).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 删除实体数据（通过主键）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValue">主键</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Delete<T>(dynamic primaryKeyValue) where T : class, new()
        {
            try
            {
                var res = await _orm.Deleteable<T>(primaryKeyValue).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除实体数据【批量删除】（通过主键组）
        /// </summary>
        /// <returns></returns>
        /// <param name="primaryKeyValues">主键组</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Delete<T>(dynamic[] primaryKeyValues) where T : class, new()
        {
            try
            {
                var res = await _orm.Deleteable<T>(primaryKeyValues).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="deleteObjs">实体数据</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Deletes<T>(List<T> deleteObjs) where T : class, new()
        {
            try
            {
                if (deleteObjs.Count > 0)
                {
                    var res = await _orm.Deleteable<T>(deleteObjs).EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                    return res;
                }

                return 0;
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <returns></returns>
        /// <param name="exp">条件</param>
        /// <param name="isNotEnableDiffLog">不记录变更</param>
        /// <typeparam name="T">类型</typeparam>
        public async Task<int> Delete<T>(Expression<Func<T, bool>> exp,bool isNotEnableDiffLog = false) where T : class, new()
        {
            try
            {
                var dbDelete = _orm.Deleteable<T>().Where(exp);
                if (this._isTenant)
                {
                    dbDelete = dbDelete.Where("F_TenantId = @F_TenantId",new SugarParameter("@F_TenantId", null));
                }

                if (isNotEnableDiffLog)
                {
                    var res = await dbDelete.ExecuteCommandAsync();
                    return res;
                }
                else
                {
                    var res = await dbDelete.EnableDiffLogEventIF(_isEnableDiffLog).ExecuteCommandAsync();
                    return res;
                }


            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region 对象实体查询
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="pkValue">主键</param>
        /// <param name="noLock">是否脏读</param>
        /// <typeparam name="T">类</typeparam>
        public Task<T> FindEntityByKey<T>(object pkValue,bool noLock = false) where T : class
        {
            try
            {
                if (noLock)
                {
                    return _orm.Queryable<T>().With(SqlWith.NoLock).InSingleAsync(pkValue ?? "");
                }

                return _orm.Queryable<T>().InSingleAsync(pkValue ?? "");
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="sql">sql语句</param>
        /// <param name="param">参数</param>
        /// <typeparam name="T">类</typeparam>
        public Task<T> FindEntity<T>(string sql, object param = null) where T : class
        {
            try
            {
                sql = TenantHelper.CheckTenantId(sql, this._isTenant);
                sql = DBCommonHelper.GetTranslationColumnNameBySql(sql, _dataType, _pgSqlIsAutoToLower);
                return _orm.Ado.SqlQuerySingleAsync<T>(sql, param);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取单个实体数据
        /// </summary>
        /// <returns>单个实体数据</returns>
        /// <param name="exp">参数</param>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="noLock">是否脏读</param>
        /// <typeparam name="T">类</typeparam>
        public async Task<T> FindEntity<T>(Expression<Func<T, bool>> exp, string orderSql = null,bool noLock = false ) where T : class
        {
            try
            {
                var dbSelect = _orm.Queryable<T>().Where(exp);
                if (noLock)
                {
                    dbSelect = dbSelect.With(SqlWith.NoLock);
                }

                if (this._isTenant) {
                    dbSelect = dbSelect.Where(" F_TenantId = @F_TenantId ");
                }
                if (!string.IsNullOrEmpty(orderSql))
                {
                    dbSelect = dbSelect.OrderBy(orderSql);
                }
                return await dbSelect.FirstAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// 获取数据列表
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="sql">sql语句.</param>
        /// <param name="param">参数.</param>
        /// <typeparam name="T">类型.</typeparam>
        public async Task<IEnumerable<T>> FindList<T>(string sql, object param = null) where T : class
        {
            try
            {
                sql = TenantHelper.CheckTenantId(sql, this._isTenant);
                sql = DBCommonHelper.GetTranslationColumnNameBySql(sql, _dataType, _pgSqlIsAutoToLower);
                var list = (await _orm.Ado.SqlQueryAsync<T>(sql, param)).ToObject<IEnumerable<T>>();
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

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
        public async Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, string orderSql = null, string whereSql = null, object whereParam = null,bool noLock = false) where T : class
        {
            try
            {
              
                var queryable = _orm.Queryable<T>().Where(exp);
                if (noLock)
                {
                    queryable = queryable.With(SqlWith.NoLock);
                }
                if (!string.IsNullOrEmpty(orderSql)) {
                    queryable = queryable.OrderBy(orderSql);
                }

                if (!string.IsNullOrEmpty(whereSql)) {
                    queryable = queryable.Where(whereSql, whereParam);
                }


                if (this._isTenant)
                {
                    queryable = queryable.Where(" F_TenantId = @F_TenantId ");
                }

                return await queryable.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 获取数据列表（分页）
        /// </summary>
        /// <returns>列表数据</returns>
        /// <param name="sql">sql查询语句</param>
        /// <param name="param">参数</param>
        /// <param name="orderSql">排序字段</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageIndex">页码</param>
        /// <param name="whereSql">查询语句</param>
        /// <param name="whereParam">查询参数</param>
        /// <typeparam name="T">类</typeparam>
        private async Task<(IEnumerable<T> list, long total)> FindList<T>(string sql, object param, string orderSql, int pageSize, int pageIndex,string whereSql, object whereParam) where T : class,new()
        {
            try
            {
                sql = TenantHelper.CheckTenantId(sql, this._isTenant);
                sql = DBCommonHelper.GetTranslationColumnNameBySql(sql, _dataType, _pgSqlIsAutoToLower);

                RefAsync<int> total = 0;
                var sqlQueryable = _orm.SqlQueryable<T>(sql);

                if (param != null) {
                    sqlQueryable = sqlQueryable.AddParameters(DBCommonHelper.ConvertToDic(param));
                }  
                if (!string.IsNullOrEmpty(orderSql))
                {
                    sqlQueryable = sqlQueryable.OrderBy(orderSql);
                }
                if (!string.IsNullOrEmpty(whereSql)) {
                    sqlQueryable = sqlQueryable.Where(whereSql, whereParam);
                }

                var list = await sqlQueryable.ToPageListAsync(pageIndex, pageSize, total);
                return (list, total);

            }
            catch (Exception)
            {
                throw;
            }
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
        public async Task<IEnumerable<T>> FindList<T>(string sql, object param, Pagination pagination, string whereSql = null, object whereParam = null) where T : class, new()
        {

            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord)) {
                sidx += $" {pagination.sord}";
            }



            var (list, total) = await FindList<T>(sql, param, sidx, pagination.rows, pagination.page, whereSql, whereParam);
            pagination.records = total;
            return list;
        }

         
        
        /// <summary>
        /// 获取数据列表（分页）
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="pagination">分页参数</param>
        /// <param name="filter">过滤器名称</param>
        /// <param name="noLock">是否脏读</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindList<T>(Pagination pagination, string filter,bool noLock = false) where T : class
        {
            RefAsync<int> total = 0;
            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }

            var queryable = _orm.Queryable<T>().OrderBy(sidx).Filter(filter, false);
            if (noLock)
            {
                queryable = queryable.With(SqlWith.NoLock);
            }
            if (this._isTenant)
            {
                queryable = queryable.Where(" F_TenantId = @F_TenantId ");
            }

            var list = await queryable.ToPageListAsync(pagination.page, pagination.rows, total);
            pagination.records = total;
            return list;
        }

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
        public async Task<IEnumerable<T>> FindList<T>(Expression<Func<T, bool>> exp, Pagination pagination, string whereSql=null, object whereParam = null, bool noLock = false) where T : class, new()
        {
            RefAsync<int> total = 0;
            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }
            var queryable = _orm.Queryable<T>().Where(exp);
            if (noLock)
            {
                queryable = queryable.With(SqlWith.NoLock);
            }
            if (!string.IsNullOrEmpty(whereSql))
            {
                queryable = queryable.Where(whereSql, whereParam);
            }

            if (this._isTenant)
            {
                queryable = queryable.Where(" F_TenantId = @F_TenantId ");
            }

            if (!string.IsNullOrEmpty(sidx)) {
                queryable = queryable.OrderBy(sidx);
            }

            var list =await queryable.ToPageListAsync(pagination.page, pagination.rows, total);
            pagination.records = total;
            return list;
        }
        
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
        public async Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination,DateTime beginTime, DateTime endTime, string whereSql=null, object whereParam = null,bool noLock = false) where T : class, new()
        {
            RefAsync<int> total = 0;
            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }
            var queryable = _orm.Queryable<T>().Where(exp);
            if (noLock)
            {
                queryable = queryable.With(SqlWith.NoLock);
            }
            if (!string.IsNullOrEmpty(whereSql))
            {
                queryable = queryable.Where(whereSql, whereParam);
            }

            if (this._isTenant)
            {
                queryable = queryable.Where(" F_TenantId = @F_TenantId ");
            }

            queryable = queryable.SplitTable(beginTime, endTime);

            if (!string.IsNullOrEmpty(sidx)) {
                queryable = queryable.OrderBy(sidx);
            }
            

            var list =await queryable.ToPageListAsync(pagination.page, pagination.rows, total);
            pagination.records = total;
            return list;


           
        }


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
        public async Task<IEnumerable<T>> FindListBySplitTable<T>(Expression<Func<T, bool>> exp, Pagination pagination, string whereSql=null, object whereParam = null,bool noLock = false) where T : class, new()
        {
            RefAsync<int> total = 0;
            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }
            var queryable = _orm.Queryable<T>().Where(exp);
            if (noLock)
            {
                queryable = queryable.With(SqlWith.NoLock);
            }
            if (!string.IsNullOrEmpty(whereSql))
            {
                queryable = queryable.Where(whereSql, whereParam);
            }

            if (this._isTenant)
            {
                queryable = queryable.Where(" F_TenantId = @F_TenantId ");
            }
            
            
            queryable = queryable.SplitTable(tabs => tabs.Take(1));

            if (!string.IsNullOrEmpty(sidx)) {
                queryable = queryable.OrderBy(sidx);
            }

            var list =await queryable.ToPageListAsync(pagination.page, pagination.rows, total);
            pagination.records = total;
            return list;


           
        }



        /// <summary>
        /// 获取表的所有数据
        /// </summary>
        /// <returns>列表数据</returns>
        /// <typeparam name="T">类</typeparam>
        /// <param name="orderSql">排序sql语句</param>
        /// <param name="noLock">是否脏读</param>
        public async Task<IEnumerable<T>> FindAll<T>(string orderSql = null,bool noLock = false) where T : class
        {
            try
            {
                var queryable = _orm.Queryable<T>();
                if (noLock)
                {
                    queryable = queryable.With(SqlWith.NoLock);
                }
                if (this._isTenant)
                {
                    queryable = queryable.Where(" F_TenantId = @F_TenantId ");
                }
                if (!string.IsNullOrEmpty(orderSql)) {
                    queryable = queryable.OrderBy(orderSql);
                }

                return await queryable.ToListAsync();
            }
            catch (Exception)
            {
                throw;
            }
        }


        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <param name="pagination">查询参数</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func, Pagination pagination) where T : class, new()
        {
            RefAsync<int> total = 0;
            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }
            ISugarQueryable<T> queryable = func(this._orm.Queryable<T>());
            if (this._isTenant)
            {
                queryable = queryable.Where("t.F_TenantId = @F_TenantId ");
            }
            if (!string.IsNullOrEmpty(sidx))
            {
                queryable = queryable.OrderBy(sidx);
            }

            var list = await queryable.ToPageListAsync(pagination.page, pagination.rows, total);
            pagination.records = total;
            return list;
        }

        /// <summary>
        /// 适用于多表联表查询
        /// </summary>
        /// <typeparam name="T">类</typeparam>
        /// <param name="func">委托对象</param>
        /// <returns></returns>
        public async Task<IEnumerable<T>> FindListByQueryable<T>(Func<ISugarQueryable<T>, ISugarQueryable<T>> func) where T : class, new()
        {
            ISugarQueryable<T> queryable = func(this._orm.Queryable<T>());
            if (this._isTenant)
            {
                queryable = queryable.Where("t.F_TenantId = @F_TenantId ");
            }
            var list = await queryable.ToListAsync();
            return list;
        }



        #endregion

        #region 数据源查询
        /// <summary>
        /// 查询方法，返回datatable
        /// </summary>
        /// <returns>datatable数据</returns>
        /// <param name="sql">sql语句</param>
        /// <param name="sqlparam">参数</param>
        public async Task<DataTable> FindTable(string sql, object sqlparam = null)
        {
            try
            {
                sql = TenantHelper.CheckTenantId(sql, this._isTenant);
                sql = DBCommonHelper.GetTranslationColumnNameBySql(sql, _dataType, _pgSqlIsAutoToLower);

                var sqlQueryable = _orm.SqlQueryable<object>(sql);
                if (sqlparam != null)
                {
                    sqlQueryable = sqlQueryable.AddParameters(sqlparam);
                }
                var dt = await sqlQueryable.ToDataTableAsync();
                return DBCommonHelper.DtColToLow(dt);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 分页查询方法，返回datatable
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public async Task<DataTable> FindTable(string sqlStr, object sqlparam,Pagination pagination,string whereStr = null, object whereParam = null)
        {
            sqlStr = TenantHelper.CheckTenantId(sqlStr, this._isTenant);
            sqlStr = DBCommonHelper.GetTranslationColumnNameBySql(sqlStr, _dataType, _pgSqlIsAutoToLower);

            string sidx = pagination.sidx;
            if (!string.IsNullOrEmpty(pagination.sidx) && !string.IsNullOrEmpty(pagination.sord))
            {
                sidx += $" {pagination.sord}";
            }
            var (list, total) = await FindTable(sqlStr, sqlparam, whereStr, whereParam, sidx, pagination.rows, pagination.page);
            pagination.records = total;
            return DBCommonHelper.DtColToLow(list);
        }

        /// <summary>
        /// 分页查询方法，返回datatable
        /// </summary>
        /// <returns>datatable数据</returns>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="whereParam">查询参数</param>
        /// <param name="orderSql">排序字段</param>
        /// <param name="pageSize">每页数据条数</param>
        /// <param name="pageIndex">页码</param>
        /// <returns>list数据,total 总共条数</returns>
        private async Task<(DataTable list, long total)> FindTable(string sqlStr, object sqlparam, string whereStr, object whereParam, string orderSql, int pageSize, int pageIndex)
        {

            try
            {
                sqlStr = TenantHelper.CheckTenantId(sqlStr, this._isTenant);
                sqlStr = DBCommonHelper.GetTranslationColumnNameBySql(sqlStr, _dataType, _pgSqlIsAutoToLower);

                RefAsync<int> total = 0;
                var sqlQueryable = _orm.SqlQueryable<object>(sqlStr);
                if (sqlparam != null)
                {
                    sqlQueryable = sqlQueryable.AddParameters(sqlparam);
                }

                if (!string.IsNullOrEmpty(whereStr))
                {
                    sqlQueryable = sqlQueryable.Where(whereStr, whereParam);
                }
                if (!string.IsNullOrEmpty(orderSql))
                {
                    sqlQueryable = sqlQueryable.OrderBy(orderSql);
                }
                var dt = await sqlQueryable.ToDataTablePageAsync(pageIndex, pageSize, total);
                return (dt, total);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 查询方法，返回datatable----CHANGE BY SSY 20231008
        /// </summary>
        /// <param name="sqlStr">sql语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <param name="sqlparam">sql参数</param>
        /// <param name="orderSql">排序字段</param>
        /// <returns></returns>
        public async Task<DataTable> FindTable(string sqlStr, string whereStr, string orderSql, object sqlparam = null)
        {

            try
            {
                sqlStr = TenantHelper.CheckTenantId(sqlStr, this._isTenant);
                sqlStr = DBCommonHelper.GetTranslationColumnNameBySql(sqlStr, _dataType, _pgSqlIsAutoToLower);

                var sqlQueryable = _orm.SqlQueryable<object>(sqlStr);
                if (sqlparam != null)
                {
                    sqlQueryable = sqlQueryable.AddParameters(sqlparam);
                }
                if (!string.IsNullOrEmpty(whereStr))
                {
                    sqlQueryable = sqlQueryable.Where(whereStr);
                }
                if (!string.IsNullOrEmpty(orderSql))
                {
                    sqlQueryable = sqlQueryable.OrderBy(orderSql);
                }
                var dt = await sqlQueryable.ToDataTableAsync();
                return DBCommonHelper.DtColToLow(dt);
            }
            catch (Exception)
            {
                throw;
            }
        }



        #endregion

        #region 获取数据库表信息
        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <returns>数据库表信息</returns>
        /// <param name="isCache">是否读取存储区</param>
        public List<DbTableInfo> GetTableInfoList(bool isCache = false)
        {
            try
            {
                /*if (_dataType == DbType.SqlServer) {
                    return _orm.Ado.SqlQueryAsync<DbTableInfo>(@"SELECT s.Name,Convert(nvarchar(max),tbp.value) as Description
                        FROM sysobjects s
                        LEFT JOIN sys.extended_properties as tbp ON s.id = tbp.major_id and tbp.minor_id = 0 AND(tbp.Name = 'MS_Description' OR tbp.Name is null)  WHERE s.xtype IN('U')
                    ").GetAwaiter().GetResult();
                }*/

                return _orm.DbMaintenance.GetTableInfoList(isCache);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 判断表是否存在
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public bool IsAnyTable(string tableName, bool isCache = false)
        {
            try
            {
                return _orm.DbMaintenance.IsAnyTable(tableName,isCache);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public List<DbColumnInfo> GetColumnInfosByTableName(string tableName, bool isCache = false)
        {
            try
            {
                var list = _orm.DbMaintenance.GetColumnInfosByTableName(tableName, isCache);

                foreach (var item in list) {
                    item.PropertyName = DBCommonHelper.GetCsType(DataType, item.DataType);
                }
                
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 获取数据库表主键信息
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public List<string> GetPrimaries(string tableName)
        {
            try
            {
                return _orm.DbMaintenance.GetPrimaries(tableName);
            }
            catch (Exception)
            {
                throw;
            }
        }

        /// <summary>
        /// 获取数据库地址信息(等待补充完全)
        /// </summary>
        /// <returns>数据库地址信息</returns>
        public string GetDataSource()
        {
            try
            {
                string DataSource = string.Empty;
                var Connection = _orm.Ado.Connection;
                switch (_dataType)
                {
                    case DbType.MySql:
                    case DbType.Tidb:
                    case DbType.OceanBase:
                        DataSource=((MySqlConnector.MySqlConnection)Connection).DataSource;
                        break;
                    case DbType.SqlServer:
                        DataSource = ((Microsoft.Data.SqlClient.SqlConnection)Connection).DataSource;
                        break;
                    case DbType.HG:
                    case DbType.OpenGauss:
                    case DbType.PostgreSQL:
                        DataSource = ((Npgsql.NpgsqlConnection)Connection).DataSource;
                        if (string.IsNullOrEmpty(DataSource))
                        {
                            DataSource = ((Npgsql.NpgsqlConnection)Connection).Host;
                        }
                        break;
                    case DbType.Dm:
                        Connection = _orm.CreateContext().Db.Ado.Connection;
                        DataSource = ((Dm.DmConnection)Connection).DataSource;
                        break;
                    case DbType.Kdbndp:
                    case DbType.Sqlite:
                        DataSource = ((Microsoft.Data.Sqlite.SqliteConnection)Connection).DataSource;
                        break;
                    case DbType.Oracle:
                        DataSource = ((Oracle.ManagedDataAccess.Client.OracleConnection)Connection).DataSource;
                        break;
                    case DbType.Oscar://神通数据库
                        DataSource = ((System.Data.OscarClient.OscarConnection)Connection).DataSource;
                        break;
                    case DbType.QuestDB://时序数据库-QuestDB
                        DataSource = ((Npgsql.NpgsqlConnection)((QuestDBProvider)_orm.Ado).Connection).Host;
                        break;
                    case DbType.ClickHouse://时序数据库
                        DataSource = ((ClickHouseConnection)((ClickHouseProvider)_orm.Ado).Connection).DataSource;
                        break;
                    case DbType.TDengine:
                        DataSource = ((TDengineConnection)((TDengineProvider)_orm.Ado).Connection).DataSource;
                        break;
                    case DbType.GBase:
                        DataSource = (((GBaseProvider)_orm.Ado).Connection).Database;
                        break;
                    default:
                        break;
                }
                //_orm.DbMaintenance.
                
                return DataSource;
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 测试数据库是否能链接成功
        /// </summary>
        /// <returns></returns>
        public bool TestConnection()
        {
            return  _orm.Ado.IsValidConnection();
        }
        /// <summary>
        /// 获取sql语句的字段
        /// </summary>
        /// <param name="sqlStr">查询语句</param>
        /// <param name="whereStr">查询条件</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetSqlColName(string sqlStr, string whereStr=null)
        {
            try
            {

                DataTable dt = null;
                if (sqlStr.IndexOf("with", StringComparison.OrdinalIgnoreCase) != -1)
                {
                    dt = await _orm.SqlQueryable<object>(sqlStr).Where(whereStr).ToDataTableAsync();
                }
                else
                {
                    dt = await _orm.SqlQueryable<object>(sqlStr).Where(whereStr).ToDataTablePageAsync(1, 1);
                }

               

                //DataTable dt = await _orm.Ado.GetDataSetAllAsync(strSql);// 此处需要做分页

                List<string> res = new List<string>();
                foreach (DataColumn item in dt.Columns)
                {
                    res.Add(item.ColumnName.ToLower());
                }
                return res;

            }
            catch (Exception)
            {
                throw;
            }
        }



        /// <summary>
        /// 插入数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<int> ExecuteInsert(string tableName, List<SugarParameter> dbParameters)
        {
            var sql = new StringBuilder();
            var sqlValue = new StringBuilder();

            if (this._isTenant) {
                dbParameters.Add(new SugarParameter("F_TenantId", ""));
            }
            


            sql.Append($" INSERT INTO {tableName} ( ");
            sqlValue.Append($" ( ");
            bool isfirt = true;
            foreach (var item in dbParameters)
            {
                if (!isfirt)
                {
                    sql.Append(",");
                    sqlValue.Append(",");
                }

                sql.Append(DBCommonHelper.GetTranslationColumnName(item.ParameterName,_dataType, _pgSqlIsAutoToLower));
                sqlValue.Append($"@{item.ParameterName}");
                isfirt = false;
            }
            
            
            
            sql.Append($" ) VALUES {sqlValue} )");


            var strSql = DBCommonHelper.SqlFormat(sql.ToString(), _dataType);

            return await _orm.Ado.ExecuteCommandAsync(strSql, dbParameters);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="pkey">主键名</param>
        /// <returns></returns>
        public async Task<int> ExecuteUpdate(string tableName, List<SugarParameter> dbParameters, string pkey)
        {
            var pkeyList = pkey.Split(",");
            var sql = new StringBuilder();
            sql.Append($" UPDATE {tableName} SET ");

            bool isfirt = true;
            foreach (var item in dbParameters)
            {
                if (!pkeyList.Contains(item.ParameterName))
                {
                    if (item.Value == null)
                    {
                        if (!isfirt)
                        {
                            sql.Append(",");
                        }
                        sql.Append($"{DBCommonHelper.GetTranslationColumnName(item.ParameterName,_dataType, _pgSqlIsAutoToLower)} = null ");
                        isfirt = false;
                    }
                    else
                    {
                        if (!isfirt)
                        {
                            sql.Append(",");
                        }
                        sql.Append($"{DBCommonHelper.GetTranslationColumnName(item.ParameterName,_dataType, _pgSqlIsAutoToLower)} = @{item.ParameterName} ");
                        isfirt = false;
                    }
                }
                else
                {
                    item.Value = item.Value.ToString().Trim();
                }


            }
            
            // 支持多主键
            
            sql.Append($" WHERE 1=1 ");
            foreach (var keyItem in pkeyList) {
                sql.Append($" AND {DBCommonHelper.GetTranslationColumnName(keyItem,_dataType, _pgSqlIsAutoToLower)} = @{keyItem} ");
            }

            if (this._isTenant)
            {
                dbParameters.Add(new SugarParameter("F_TenantId", ""));
                sql.Append($" AND {DBCommonHelper.GetTranslationColumnName("F_TenantId",_dataType, _pgSqlIsAutoToLower)} = @F_TenantId ");
            }

            
            var strSql = DBCommonHelper.SqlFormat(sql.ToString(), _dataType);

            return await _orm.Ado.ExecuteCommandAsync(strSql, dbParameters);

            //return await _orm.Ado.ExecuteCommandAsync(sql.ToString(), dbParameters);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <param name="whereSql">主键名</param>
        /// <returns></returns>
        public async Task<int> ExecuteUpdateByWhere(string tableName, List<SugarParameter> dbParameters, string whereSql)
        {
            if (string.IsNullOrEmpty(whereSql))
            {
                return 0;
            }

            var sql = new StringBuilder();
            sql.Append($" UPDATE {tableName} SET ");

            bool isfirt = true;
            foreach (var item in dbParameters)
            {
                if (!isfirt)
                {
                    sql.Append(",");
                }
                isfirt = false;
                if (item.Value == null)
                {
                    sql.Append($"{DBCommonHelper.GetTranslationColumnName(item.ParameterName,_dataType, _pgSqlIsAutoToLower)} = null ");
                }
                else
                {
                    sql.Append($"{DBCommonHelper.GetTranslationColumnName(item.ParameterName,_dataType, _pgSqlIsAutoToLower)} = @{item.ParameterName} ");
                }
            }
            
            // 支持多主键
            
            sql.Append($" WHERE  ");
            sql.Append(whereSql);
            

            if (this._isTenant)
            {
                dbParameters.Add(new SugarParameter("F_TenantId", ""));
                sql.Append($" AND {DBCommonHelper.GetTranslationColumnName("F_TenantId",_dataType, _pgSqlIsAutoToLower)} = @F_TenantId ");
            }

            
            var strSql = DBCommonHelper.SqlFormat(sql.ToString(), _dataType);

            return await _orm.Ado.ExecuteCommandAsync(strSql, dbParameters);
        }


        /// <summary>
        /// 删除表数据
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="dbParameters">参数</param>
        /// <returns></returns>
        public async Task<int> ExecuteDelete(string tableName, List<SugarParameter> dbParameters)
        {
            var sql = new StringBuilder();
            sql.Append($" DELETE FROM {tableName} WHERE 1=1 ");

            foreach (var item in dbParameters)
            {
                sql.Append($" AND [{item.ParameterName}] = @{item.ParameterName} ");
            }

            if (this._isTenant)
            {
                dbParameters.Add(new SugarParameter("F_TenantId", ""));
                sql.Append($" AND [F_TenantId] = @F_TenantId ");
            }
            
            var strSql = DBCommonHelper.GetTranslationColumnNameBySql(sql.ToString(), _dataType,_pgSqlIsAutoToLower);
            
            
            strSql = DBCommonHelper.SqlFormat(strSql, _dataType);

            return await _orm.Ado.ExecuteCommandAsync(strSql, dbParameters);
        }
        #endregion

        #region 数据库操作
        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        public bool CreateDatabase(string databaseDirectory = null)
        {
            try
            {
                return _orm.DbMaintenance.CreateDatabase(databaseDirectory);
            }
            catch (Exception)
            {
                throw;
            }
        }
        /// <summary>
        /// 如果不存在创建数据库
        /// </summary>
        /// <param name="databaseName">数据库名称</param>
        /// <param name="databaseDirectory">数据库备注</param>
        /// <returns></returns>
        public bool CreateDatabase(string databaseName, string databaseDirectory = null)
        {
            try
            {
                return _orm.DbMaintenance.CreateDatabase(databaseName, databaseDirectory);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public void InitTables<T>()
        {
            try
            {
                _orm.CodeFirst.SetStringDefaultLength(255).InitTables<T>();//这样一个表就能成功创建了
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        /// <summary>
        /// 获取实体类型
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        /// <returns></returns>
        public TypeModel GetTypeBilder(EntitiesGen genentity,bool isDeleteOldField = false)
        {
            TypeModel typeModel = new TypeModel();
            genentity = GetEntitiesGen(genentity,false,isDeleteOldField);
            // 创建动态类
            var typeBilder = _orm.DynamicBuilder().CreateClass(genentity.TableName, new SugarTable(){});
            foreach (var propertyGen in  genentity.PropertyGens)
            {
                var col = new SugarColumn() {IsPrimaryKey=propertyGen.IsPrimaryKey};
                var colName = propertyGen.DbColumnName;
                if (propertyGen.IsPrimaryKey && !string.IsNullOrEmpty(genentity.NewPkName))
                {
                    colName = genentity.NewPkName;
                }
                            
                if (propertyGen.Type == "text")
                {
                    col.ColumnDataType = StaticConfig.CodeFirst_BigString;
                    typeModel.IsBigData = true;
                }
                            
                var pType = GetCsType(propertyGen.Type);
                if (pType == typeof(byte[]))
                {
                    typeModel.ByteColName = colName;
                    typeModel.IsBigData = true;
                }
                typeBilder.CreateProperty(colName, pType, col);
            }
            typeModel.ClassType =  typeBilder.BuilderType();
            return typeModel;
        }

        /// <summary>
        /// 创建或更新数据库表
        /// </summary>
        /// <param name="genentity">虚拟类信息</param>
        /// <param name="isDeleteOldField">是否删除没有的字段</param>
        public void CreateOrUpdateTable(EntitiesGen genentity,bool isDeleteOldField = false)
        {
            genentity = GetEntitiesGen(genentity,true,isDeleteOldField);
            _CreateOrUpdateTable(genentity);

            if (!string.IsNullOrEmpty(genentity.NewPkName))
            {
                var oldName = genentity.PropertyGens.Find(t => t.IsPrimaryKey).DbColumnName;
                _orm.DbMaintenance.RenameColumn(genentity.TableName,oldName,genentity.NewPkName);
                Trace.WriteLine($"新增表{genentity.TableName} 主键名！{oldName} 变为 {genentity.NewPkName}");
            }
        }

        /// <summary>
        /// 回收列：CodeFirst 只会建表和加列，模型里已经去掉的列要靠这一步才能真正从物理表上删掉。
        /// 只回收调用方逐条点过名的列——列名不在表上就算已经回收，主键列与自增列一律拒绝。
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="columnNames">确认过要回收的列名</param>
        public void DropColumns(string tableName, IEnumerable<string> columnNames)
        {
            var names = (columnNames ?? new List<string>())
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
            if (names.Count == 0)
            {
                return;
            }
            if (!IsAnyTable(tableName))
            {
                return;
            }

            var oldCols = GetColumnInfosByTableName(tableName);
            foreach (var name in names)
            {
                var col = oldCols == null ? null : oldCols.Find(t => string.Equals(t.DbColumnName, name, StringComparison.OrdinalIgnoreCase));
                if (col == null)
                {
                    continue;
                }
                if (col.IsPrimarykey || col.IsIdentity)
                {
                    throw new Exception($"表【{tableName}】的列【{name}】是主键列或自增列，不允许回收");
                }
                _orm.DbMaintenance.DropColumn(tableName, col.DbColumnName);
                Trace.WriteLine($"回收表{tableName} 列！{col.DbColumnName}");
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="genentity"></param>
        /// <param name="isExecute"></param>
        /// <param name="isDeleteOldField"></param>
        /// <returns></returns>
        private EntitiesGen GetEntitiesGen(EntitiesGen genentity, bool isExecute,bool isDeleteOldField = false)
        { 
            List<DbColumnInfo> oldCols = null;
            if (IsAnyTable(genentity.TableName))
            {
                oldCols = GetColumnInfosByTableName(genentity.TableName);
            }
            if (oldCols != null)
            {
                foreach (var oldCol in oldCols)
                {
                    
                    
                    if (!string.IsNullOrEmpty(oldCol.OracleDataType))
                    {
                        oldCol.DataType = oldCol.OracleDataType;
                    }
                    var prop = genentity.PropertyGens.Find(t => t.DbColumnName.ToLower() == oldCol.DbColumnName.ToLower());
                    if (prop != null)
                    {
                        
                        // 属性不变
                        if (oldCol.PropertyName == prop.Type && oldCol.DataType != "int4")
                        {
                            prop.IsNotUpdate = true;
                            prop.DbType = oldCol.DataType;
                        }
                        
                        // 设置string类型的长度
                        if (prop.Length > oldCol.Length && prop.Type == "string")
                        {
                            oldCol.Length = (int)prop.Length + 2;
                            if(oldCol.Length > 4000)
                            {
                                oldCol.Length = 4000;
                            }

                            if (oldCol.DataType == "NVARCHAR2" && oldCol.Length == 4000)
                            {
                                oldCol.Length = 2000;
                            }

                            if (isExecute)
                            {
                                _orm.DbMaintenance.UpdateColumn(genentity.TableName,oldCol);
                            }

                            
                        }
                        
                        // 兼容 oracle nullable 不能重复设置问题
                        if (!prop.IsPrimaryKey && !oldCol.IsNullable && DataType == DbType.Oracle)
                        {
                            if (isExecute)
                            {
                                var sql =
                                    $"ALTER TABLE \"{genentity.TableName.ToUpper()}\" modify (\"{oldCol.DbColumnName.ToUpper()}\"  NULL )";
                                _orm.Ado.ExecuteCommand(sql);
                            }
                        }
                    }
                    else if (oldCol.IsPrimarykey)
                    {
                        var pkProp = genentity.PropertyGens.Find(t => t.IsPrimaryKey);
                        if (pkProp == null)
                        {
                            if (!isDeleteOldField)
                            {
                                genentity.PropertyGens.Add(new PropertyGen()
                                {
                                    DbColumnName = oldCol.DbColumnName,
                                    PropertyName = oldCol.DbColumnName,
                                    Description = oldCol.ColumnDescription,
                                    IsIdentity = oldCol.IsIdentity,
                                    IsPrimaryKey = oldCol.IsPrimarykey,
                                    IsNullable = oldCol.IsNullable,
                                    Type = oldCol.PropertyName,
                                    DbType = oldCol.DataType,
                                    Length = oldCol.Length,
                                    DecimalDigits = oldCol.DecimalDigits,
                                    IsSpecialType = false,
                                    IsIgnore = false
                                });
                            }
                        }
                        else
                        {
                            genentity.NewPkName = pkProp.DbColumnName;
                            pkProp.DbColumnName = oldCol.DbColumnName;
                            pkProp.PropertyName = oldCol.DbColumnName;
                        }

                        
                    }
                    else if (!isDeleteOldField)
                    {
                        genentity.PropertyGens.Add(new PropertyGen()
                        {
                            DbColumnName = oldCol.DbColumnName,
                            PropertyName = oldCol.DbColumnName,
                            Description = oldCol.ColumnDescription,
                            IsIdentity = oldCol.IsIdentity,
                            IsPrimaryKey = oldCol.IsPrimarykey,
                            IsNullable = oldCol.IsNullable,
                            Type = oldCol.PropertyName,
                            DbType = oldCol.DataType,
                            Length = oldCol.Length,
                            DecimalDigits = oldCol.DecimalDigits,
                            IsSpecialType = false,
                            IsIgnore = false
                        });
                    }
                }
            }

            return genentity;
        }
        

        private void _CreateOrUpdateTable(EntitiesGen genentity) {
            foreach (var column in genentity.PropertyGens) {
                column.DbType = DBCommonHelper.GetDbType(Orm.CurrentConnectionConfig.DbType, column.DbType, column.Type);
                if (column.Type == "text") {
                    column.Type = "string";
                }
                if (column.Type == "nstring")
                {
                    column.Type = "string";
                    column.DbType = "nvarchar";
                }
            }
            
            //处理生成数据表对应的实体信息
            var classString = SyntaxTreeHelper.GetClassString(genentity);
            var type = SyntaxTreeHelper.GetModelTypeByClass(classString, genentity.ClassName);
            Orm.CurrentConnectionConfig.ConfigureExternalServices = new ConfigureExternalServices()
            {
                EntityNameService = (type, info) =>
                {
                    if (info.EntityName == genentity.ClassName || (info.EntityName == null && info.DbTableName == genentity.ClassName))
                    {
                        info.EntityName = genentity.ClassName;
                        info.DbTableName = genentity.TableName;
                        info.TableDescription = genentity.Description;
                    }
                },
                EntityService = (type, info) =>
                {
                    if (info.EntityName == genentity.ClassName)
                    {
                        var column = genentity.PropertyGens.FirstOrDefault(it => it.DbColumnName == info.DbColumnName);
                        info.DbColumnName = column.DbColumnName;
                        info.ColumnDescription = column.Description;
                        info.IsNullable = column.IsNullable;
                        info.Length = Convert.ToInt32(column.Length);
                        info.DecimalDigits = Convert.ToInt32(column.DecimalDigits);
                        info.IsPrimarykey = column.IsPrimaryKey;
                        info.IsIdentity = column.IsIdentity;
                        info.IsIgnore = column.IsIgnore;
                        //info.DataType = column.DbType;
                        
                        //if (Orm.CurrentConnectionConfig.DbType == DbType.PostgreSQL || Orm.CurrentConnectionConfig.DbType == DbType.SqlServer) {
                        if (
                            column.DbType == "int" 
                            || column.Type == "DateTime" 
                            || column.Type == "TimeSpan" 
                            || column.DbType == "clob"
                            || column.DbType == "text"
                            || column.DbType == "longtext"
                            || column.DbType == "timestamp"
                            || column.DbType == "bit"
                            || column.DbType == "tinyint" 
                            || column.Type == "byte[]"
                            )
                        {
                            info.Length = 0;
                            info.DecimalDigits = 0;
                        }


                        if (Orm.CurrentConnectionConfig.DbType == DbType.Oracle)
                        {
                            if (column.DbType == "clob") {
                                info.DataType = "clob";
                            }
                        }
                        else {
                            if (column.DbType == "text")
                            {
                                if (Orm.CurrentConnectionConfig.DbType == DbType.SqlServer)
                                {
                                    info.DataType = "nvarchar(max),ntext,text";
                                }
                                else {
                                    info.DataType = "text";
                                }
                            }

                            if (column.DbType == "longtext")
                            {

                                if (Orm.CurrentConnectionConfig.DbType == DbType.SqlServer)
                                {
                                    info.DataType = "nvarchar(max),ntext,text";
                                }
                                else
                                {
                                    info.DataType = "longtext,ntext,text";
                                }
                            }

                            if (column.DbType == "timestamp")
                            {
                                info.DataType = "timestamp";
                            }
                        }
                        
                        if (Orm.CurrentConnectionConfig.DbType == DbType.Sqlite && info.IsIdentity)
                        {
                            info.DataType = "integer";
                        }

                        if (Orm.CurrentConnectionConfig.DbType == DbType.SqlServer && column.Type == "DateTime")
                        {
                            if (column.DbType == "datetime")
                            {
                                info.DataType = "datetime,datetime2";
                            }
                            else {
                                info.DataType = "datetime2,datetime";
                            }
                        }
                        
                        

                        if (column.Type == "string" && (column.DbType == "nvarchar" || column.DbType == "varchar")) {
                            info.DataType = "nvarchar,varchar";
                        }

                        if (info.Length == -1 && column.Type == "string" && info.DataType != "text" && info.DataType != "clob")
                        {
                            if (Orm.CurrentConnectionConfig.DbType == DbType.SqlServer)
                            {
                                if (column.DbType == "nvarchar")
                                {
                                    info.DataType = "nvarchar(MAX)";
                                }
                                else
                                {
                                    info.DataType = "varchar(MAX)";
                                }
                            }
                            if (Orm.CurrentConnectionConfig.DbType == DbType.Oracle)
                            {
                                if (column.DbType == "clob")
                                {
                                    info.DataType = "clob";
                                }
                            }
                            else
                            {
                                if (column.DbType == "text")
                                {
                                    info.DataType = "text";
                                }
                            }
                        }

                        /*if (column.Type == "byte[]")
                        {
                            info.Length = 0;
                            // info.DataType 
                        }*/

                        if (column.Type == "TimeSpan") {
                            info.DataType = "time";
                        }

                        // 如果本身数据库有这个字段，且对应的C#类型没有改变，则数据库字段类型不做调整
                        if (column.IsNotUpdate) {
                            info.DataType = column.DbType;
                        }
                    }
                }
            };
            Orm.CodeFirst.InitTables(type);
            Orm.CurrentConnectionConfig.ConfigureExternalServices = null;
        }
        
        private Type GetCsType(string csType)
        {
            string _csType = csType;
            switch (csType)
            {
                case "int":
                    return  typeof(int?);
                case "long":
                    return  typeof(long?);
                case "short":
                    return  typeof(short?);
                case "bool":
                    _csType = "System.Boolean";
                    break;
                case "float":
                    _csType = "System.Single";
                    break;
                case "text":
                    _csType = "System.String";
                    break;
                default:
                    if (!string.IsNullOrEmpty(csType))
                    {
                        _csType = $"System.{Str.FirstUpper(csType)}";
                    }
                    break;
            }

            return System.Type.GetType(_csType);
        }
        
        #endregion

        #region 多租户
        /// <summary>
        /// 开启租户模式
        /// </summary>
        public void OpenTenant() { 
            this._isTenant = true;
        }
        /// <summary>
        /// 关闭租户模式
        /// </summary>
        public void ColseTenant()
        {
            this._isTenant = false;
        }
        #endregion
    }
}
