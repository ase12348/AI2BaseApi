using jcode.util;
using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;
using SqlSugar;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据库连接
    /// </summary>
    public interface DatabaseLinkIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DatabaseLinkEntity>> GetList();
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除自定义查询条件
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存自定义查询（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="databaseLinkEntity">实体</param>
        /// <returns></returns>
        Task<bool> SaveEntity(string keyValue, DatabaseLinkEntity databaseLinkEntity);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 测试数据数据库是否能连接成功
        /// </summary>
        /// <param name="type">1高级，0 常规</param>
        /// <param name="connection"></param>
        /// <param name="dbType"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        string TestConnection(int? type, string connection, string dbType,string server, string port, string userId, string password, string dataBase);

        /// <summary>
        /// 获取数据连接实体-根据编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<DbType> GetDbTypeByCode(string code);
        #endregion
    }
}
