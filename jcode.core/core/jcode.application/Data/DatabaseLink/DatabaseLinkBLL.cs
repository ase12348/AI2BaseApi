using jcode.autofac;
using jcode.orm;
using jcode.iapplication;
using jcode.util;
using SqlSugar;
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
    public class DatabaseLinkBLL : BLLBase, DatabaseLinkIBLL, BLL
    {
        #region 属性
        private readonly DatabaseLinkService databaseLinkService = new DatabaseLinkService();
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DatabaseLinkEntity>> GetList()
        {
            return databaseLinkService.GetList();;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除自定义查询条件
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await databaseLinkService.Delete(keyValue);
        }
        /// <summary>
        /// 保存自定义查询（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="databaseLinkEntity">部门实体</param>
        /// <returns></returns>
        public async Task<bool> SaveEntity(string keyValue, DatabaseLinkEntity databaseLinkEntity)
        {
            databaseLinkEntity.F_DESEncrypt = 1;//开启加密模式
            if (databaseLinkEntity.F_Type == 0)
            {
                databaseLinkEntity.F_DbConnection = AESHelper.AesEncrypt(DBCommonHelper.SpliceConnection(
                    databaseLinkEntity.F_DbType,
                    databaseLinkEntity.F_Server,
                    databaseLinkEntity.F_Port,
                    databaseLinkEntity.F_UserId,
                    AESHelper.AesDecrypt(databaseLinkEntity.F_Password, "jcode_db"),
                    databaseLinkEntity.F_DataBase), "jcode_db");
            }
            var res = await databaseLinkService.SaveEntity(keyValue, databaseLinkEntity);
            return res;
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 测试数据数据库是否能连接成功
        /// </summary>
        /// <param name="type"></param>
        /// <param name="connection"></param>
        /// <param name="dbType"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public  string TestConnection(int? type, string connection, string dbType,string server, string port, string userId, string password, string dataBase)
        {
            if (type == 0)
            {
                connection = DBCommonHelper.SpliceConnection(dbType,server,port,userId,AESHelper.AesDecrypt(password, "jcode_db"),dataBase);
            }
            else
            {
                connection = AESHelper.AesDecrypt(connection, "jcode_db");
            }

            return databaseLinkService.TestConnection(connection, dbType);
        }
        
        /// <summary>
        /// 获取数据连接实体-根据编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public async Task<DbType> GetDbTypeByCode(string code)
        {
            return await databaseLinkService.GetDbTypeByCode(code);
        }
        #endregion
    }
}
