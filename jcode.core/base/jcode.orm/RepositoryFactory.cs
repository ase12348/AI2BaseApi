using jcode.util;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using SqlSugar;

namespace jcode.orm
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：定义仓储模型工厂
    /// </summary>
    public class RepositoryFactory
    {
        
        
        /// <summary>
        /// 开启事务
        /// </summary>
        /// <returns>The trans.</returns>
        public void BeginTrans()
        {
            string BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
            if (string.IsNullOrEmpty(BeginTransId))
            {
                BeginTransId = Guid.NewGuid().ToString();
                ContextHelper.SetItem("BeginTransId", BeginTransId);
                DbCaChe.SetTrans(BeginTransId, new List<IRepositoryWarper>());
                DbCaChe.SetTransNum(BeginTransId,1);
            }
            else {
                var num = DbCaChe.GetTransNum(BeginTransId);
                DbCaChe.SetTransNum(BeginTransId, num + 1);
            }
        }
        /// <summary>
        /// 提交
        /// </summary>
        public void Commit()
        {
            string BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
            if (!string.IsNullOrEmpty(BeginTransId)) {
                var num = DbCaChe.GetTransNum(BeginTransId);
                if (num == 1)
                {
                    var list = DbCaChe.GetTrans(BeginTransId);
                    foreach (var item in list)
                    {
                        item.Db.Commit(true);
                    }
                    DbCaChe.RemoveTrans(BeginTransId);
                    DbCaChe.RemoveTransNum(BeginTransId);
                    ContextHelper.RemoveItem("BeginTransId");
                }
                else {
                    DbCaChe.SetTransNum(BeginTransId, num - 1);
                }


                
            }
        }

        /// <summary>
        /// 把当前操作回滚成未提交状态
        /// </summary>
        public void Rollback()
        {
            string BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
            if (!string.IsNullOrEmpty(BeginTransId))
            {
                var num = DbCaChe.GetTransNum(BeginTransId);
                if (num == 1)
                { 
                    var list = DbCaChe.GetTrans(BeginTransId);
                    foreach (var item in list)
                    {
                        item.Db.Rollback(true);
                    }
                    DbCaChe.RemoveTrans(BeginTransId);
                    DbCaChe.RemoveTransNum(BeginTransId);
                    ContextHelper.RemoveItem("BeginTransId");
                }
                else
                {
                    DbCaChe.SetTransNum(BeginTransId, num - 1);
                }
            }
        }

        /// <summary>
        /// 获取数据库访问对象
        /// </summary>
        /// <param name="dbtype"></param>
        /// <param name="dblink"></param>
        /// <param name="isSlave"></param>
        /// <param name="slaveJson"></param>
        /// <param name="isTenant"></param>
        /// <param name="ignoreBeginTransId"></param>
        /// <returns></returns>
        private IRepository GetIRepository(string dbtype,string dblink,bool isSlave,string slaveJson, bool isTenant = true, bool ignoreBeginTransId = false) {
            string _BeginTransId = ContextHelper.GetItem("BeginTransId") as string;
            if (string.IsNullOrEmpty(_BeginTransId) || ignoreBeginTransId)
            {
                return new Repository(new DataBase(DBCommonHelper.GetDbType(dbtype),dblink,isSlave,GetSlaveConnectionConfigs(isSlave,slaveJson),  isTenant));
            }
            else {
                var list = DbCaChe.GetTrans(_BeginTransId);
                var dbWarper = list.Find(t => t.Link == dblink);
                if (dbWarper == null) { 
                    var db = new Repository(new DataBase(DBCommonHelper.GetDbType(dbtype),dblink,isSlave,GetSlaveConnectionConfigs(isSlave,slaveJson),  isTenant));
                    dbWarper = new IRepositoryWarper();
                    dbWarper.Link = dblink;
                    dbWarper.Db = db.BeginTrans();
                    list.Add(dbWarper);
                }

                return dbWarper.Db;
            }
        }

        private List<SlaveConnectionConfig> GetSlaveConnectionConfigs(bool isSlave,string slaveJson)
        {
            if (!isSlave || string.IsNullOrEmpty(slaveJson))
            {
                return null;
            }
            else
            {
                var list = new List<SlaveConnectionConfig>();
                try
                {
                    list = slaveJson.ToObject<List<SlaveConnectionConfig>>();
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message);
                }
                
                return list;
            }
        }


        /// <summary>
        /// 定义仓储（基础库）
        /// </summary>
        /// <param name="isTenant">是否开启租户模式</param>
        /// <param name="isIgnoreHttpItem">是否忽略上下文值</param>
        /// <param name="ignoreBeginTransId">是否忽略事务id</param>
        /// <returns></returns>
        public IRepository BaseRepository(bool isTenant = true, bool isIgnoreHttpItem = false, bool ignoreBeginTransId = false)
        {
            var serverOp = ConfigHelper.GetConfig();
            var dbConn = serverOp.dbConn;
            if (dbConn.IndexOf("jcodeDESEncrypt") != -1)
            {
                dbConn = AESHelper.AesDecrypt(dbConn.Replace("jcodeDESEncrypt",""), "jcode_db");
            }


            if (isIgnoreHttpItem) {
                return GetIRepository(serverOp.dbType, dbConn,serverOp.IsSlave,serverOp.DbConnSlave, false, ignoreBeginTransId);
            }

            string dblink = ContextHelper.GetItem("jcodedblink") as string;
            string dbtype = ContextHelper.GetItem("jcodedbtype") as string;
            string isSlave = ContextHelper.GetItem("jcodeisSlave") as string;
            string slaveJson = ContextHelper.GetItem("jcodeslaveJson") as string;
            if (!string.IsNullOrEmpty(dblink) && !string.IsNullOrEmpty(dbtype))
            {
                return GetIRepository(dbtype, dblink,isSlave == "1",slaveJson, isTenant, ignoreBeginTransId);
            }
            else {
                return GetIRepository(serverOp.dbType, dbConn,serverOp.IsSlave,serverOp.DbConnSlave, false, ignoreBeginTransId);
            }
        }

        /// <summary>
        /// 定义仓储（扩展库）
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="isTenant">是否开启租户模式</param>
        /// <param name="ignoreBeginTransId">是否忽略事务id</param>
        /// <returns></returns>
        public IRepository BaseRepository(string code, bool isTenant = true, bool ignoreBeginTransId = false)
        {
            if (code == "lrsystemdb" || code.IsEmpty())
            {
                return BaseRepository();
            }
            
            var model = DbCaChe.GetValue(code);
            if (model == null)
            {
                if (ConfigHelper.GetConfig().IsSlave)
                {
                    if (ConfigHelper.GetConfig().IsSass && ConfigHelper.GetConfig().IsSassTable)
                    {
                        model = BaseRepository().FindEntity<DbModel>(" select F_DBName,F_DbType,F_DbConnection,F_TenantId,F_IsEdit,F_IsSlave,F_SlaveJson,F_DESEncrypt from lr_base_databaselink t where F_DBName = @code {JCODE_SASSID} ", new { code }).GetAwaiter().GetResult();
                    }
                    else {
                        model = BaseRepository().FindEntity<DbModel>(" select F_DBName,F_DbType,F_DbConnection,F_IsSlave,F_SlaveJson,F_DESEncrypt from lr_base_databaselink where F_DBName = @code", new { code }).GetAwaiter().GetResult();
                    }
                }
                else
                {
                    // 兼容之前的库
                    if (ConfigHelper.GetConfig().IsSass && ConfigHelper.GetConfig().IsSassTable)
                    {
                        model = BaseRepository().FindEntity<DbModel>(" select F_DBName,F_DbType,F_DbConnection,F_TenantId,F_IsEdit,F_DESEncrypt from lr_base_databaselink t where F_DBName = @code {JCODE_SASSID} ", new { code }).GetAwaiter().GetResult();
                    }
                    else {
                        model = BaseRepository().FindEntity<DbModel>(" select F_DBName,F_DbType,F_DbConnection,F_DESEncrypt from lr_base_databaselink where F_DBName = @code", new { code }).GetAwaiter().GetResult();
                    }
                }



                if (model == null)
                {
                    throw (new Exception("此编码找不到对应数据库：" + code));
                }
                DbCaChe.SetValue(code, model);
            }
            if (isTenant && model.F_IsEdit != 0) {
                // 租户自己加的表不需要租户模式或者不是表隔离模式下
                isTenant = false;
            }
            return GetIRepository(model.F_DbType, model.F_DbConnection,model.F_IsSlave == 1,model.F_SlaveJson, isTenant, ignoreBeginTransId);
        }
        /// <summary>
        /// 定义仓储
        /// </summary>
        /// <param name="conn">链接串</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="isTenant">是否开启租户模式</param>
        /// <param name="ignoreBeginTransId">是否忽略事务id</param>
        /// <returns></returns>
        public IRepository BaseRepository(string conn, string dbType, bool isTenant = true,bool ignoreBeginTransId = false)
        {
            return GetIRepository(dbType, conn,false,"", isTenant, ignoreBeginTransId);
        }
        
        /// <summary>
        /// 数据库类型
        /// </summary>
        /// <param name="code"></param>
        /// <returns>SqlServer,Oracle,MySql</returns>
        public string DBType(string code)
        {
            if (code == "lrsystemdb")
            {
                return ConfigHelper.GetConfig().dbType;
            }
            else
            {
                var tenantNo = ContextHelper.GetItem("TenantNo") as string;
                var cacheCode = code;
                if (!string.IsNullOrEmpty(tenantNo)) {
                    cacheCode = $"{tenantNo}_{code}";
                }

                var model = DbCaChe.GetValue(cacheCode);
                if (model == null)
                {
                    model = BaseRepository().FindEntity<DbModel>(" select F_DBName,F_DbType,F_DbConnection,F_DESEncrypt  form lr_base_databaselink where F_DBName = @code ", new { code }).GetAwaiter().GetResult();
                    if (model == null)
                    {
                        throw (new Exception("此编码找不到对应数据库：" + code));
                    }
                    DbCaChe.SetValue(cacheCode, model);
                }

                return model.F_DbType;
            }
        }
    }
}
