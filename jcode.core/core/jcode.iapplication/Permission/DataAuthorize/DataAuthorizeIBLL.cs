using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.25
    /// 描 述：数据权限
    /// </summary>
    public interface DataAuthorizeIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取数据权限对应关系数据列表
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="objectIds">用户或角色主键</param>
        /// <returns></returns>
        Task<IEnumerable<DataAuthorizeEntity>> GetList(string code, string[] objectIds);
        /// <summary>
        /// 获取数据权限列表（分页）
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询关键字</param>
        /// <param name="objectId">用户或角色主键</param>
        /// <param name="type">1.普通权限 2.自定义表单权限</param>
        /// <returns></returns>
        Task<IEnumerable<DataAuthorizeEntity>> GetPageList(Pagination pagination, string keyword, string objectId, int? type);
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<DataAuthorizeEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, DataAuthorizeEntity entity);
        #endregion

        #region 扩展方法

        /// <summary>
        /// 获取数据权限设置sql
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="isForm">是否是自定义表单功能</param>
        /// <returns></returns>
        Task<string> GetWhereSql(string code, bool isForm = false);

        #endregion
    }
}
