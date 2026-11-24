using jcode.orm;
using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据库连接
    /// </summary>
    public class DatabaseLinkService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取数据库信息列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DatabaseLinkEntity>> GetList()
        {
            return this.BaseRepository().FindList<DatabaseLinkEntity>(t=>t.F_DeleteMark == 0);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">键值</param>
        /// <returns></returns>
        public Task<DatabaseLinkEntity> GetEntity(string keyValue)
        {

            return this.BaseRepository().FindEntityByKey<DatabaseLinkEntity>(keyValue);
        }


        #endregion

        #region 提交数据
        /// <summary>
        /// 删除自定义查询条件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var entity = await GetEntity(keyValue);
            await this.BaseRepository().Delete<DatabaseLinkEntity>(t=>t.F_Id == keyValue );
            DbCaChe.RemoveValue(entity.F_DBName);
        }
        /// <summary>
        /// 保存自定义查询（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="databaseLinkEntity">数据库实体</param>
        /// <returns></returns>
        public async Task<bool> SaveEntity(string keyValue, DatabaseLinkEntity databaseLinkEntity)
        {
            var dbConnection = databaseLinkEntity.F_DbConnection;
            if (databaseLinkEntity.F_DESEncrypt == 1)
            {
                dbConnection = AESHelper.AesDecrypt(dbConnection, "jcode_db");
            }
            
            var testRes = TestConnection(dbConnection, databaseLinkEntity.F_DbType);
            if (testRes != "success") {
                return false;
            }
            try
            {
                databaseLinkEntity.F_ServerAddress = this.BaseRepository(dbConnection, databaseLinkEntity.F_DbType).GetDataSource();
            }
            catch (Exception)
            {
                return false;
            }
            
            if (!string.IsNullOrEmpty(keyValue))
            {
                databaseLinkEntity.F_Id = keyValue;
                databaseLinkEntity.F_ModifyDate = DateTime.Now;
                databaseLinkEntity.F_ModifyUserId = this.GetUserId();
                databaseLinkEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(databaseLinkEntity,true);
            }
            else
            {
                databaseLinkEntity.F_Id = Guid.NewGuid().ToString();
                databaseLinkEntity.F_CreateDate = DateTime.Now;
                databaseLinkEntity.F_DeleteMark = 0;
                databaseLinkEntity.F_EnabledMark = 1;
                databaseLinkEntity.F_CreateUserId = this.GetUserId();
                databaseLinkEntity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(databaseLinkEntity);
            }
            DbCaChe.RemoveValue(databaseLinkEntity.F_DBName);
            return true;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 测试数据数据库是否能连接成功
        /// </summary>
        /// <param name="connection">连接串</param>
        /// <param name="dbType">数据库类型</param>
        public string TestConnection(string connection, string dbType)
        {
            try
            {
                if (dbType == "TDengine")
                {
                    this.BaseRepository(connection, dbType).ExecuteSql("SELECT SERVER_VERSION()");
                }
                else
                {
                    this.BaseRepository(connection, dbType).IsAnyTable("jcode");
                }
                
                return "success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        /// <summary>
        /// 获取数据连接实体-根据编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public async Task<DbType> GetDbTypeByCode(string code)
        {
            var DbType = string.Empty;
            if (code == "lrsystemdb" || code.IsEmpty())
            {
                DbType= ConfigHelper.GetConfig().dbType;
            }
            else
            {
                var entity=await this.BaseRepository().FindEntity<DatabaseLinkEntity>(it=>it.F_DBName== code);
                if (entity!=null)
                {
                    DbType= entity.F_DbType;
                }
            }
            return DBCommonHelper.GetDbType(DbType);
        }
        #endregion
    }
}
