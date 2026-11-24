using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：用户关联对象
    /// </summary>
    public interface UserRelationIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位3-负责部门4-负责公司</param>
        /// <returns></returns>
        Task<IEnumerable<UserRelationEntity>> GetObjectIdList(string userId, int category);
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位3-负责部门4-负责公司</param>
        /// <returns></returns>
        Task<List<string>> GetObjectIds(string userId, int category);

        /// <summary>
        /// 获取登录者角色
        /// </summary>
        /// <returns></returns>
        Task<List<string>> GetRoleIds();
        
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">用户主键</param>
        /// <returns></returns>
        Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId);

        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联角色或岗位或部门</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId, string attrCode);
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <returns></returns>
        Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList);

        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList, string attrCode);

        /// <summary>
        /// 获取用户信息通过角色
        /// </summary>
        /// <param name="objectId">关联或角色主键集合</param>
        /// <returns></returns>
        Task<List<UserEntity>> GetUserIdByRoleList(string objectId);

        /// <summary>
        /// 获取用户信息通过角色
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <returns></returns>
        Task<List<UserEntity>> GetUserIdByRoleList(IEnumerable<string> objectIdList);
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存对象对应用户数据
        /// </summary>
        /// <param name="objectId">对应对象主键</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <param name="userIds">对用户主键列表</param>
        Task SaveEntityList(string objectId, int category, string userIds);
        
        /// <summary>
        /// 保存对象对应用户数据
        /// </summary>
        /// <param name="objectId">对应对象主键</param>
        /// <param name="category">分类:1-角色2-岗位3-部门角色4-公司角色</param>
        /// <param name="userRelationEntityList"></param>
        Task SaveEntityList(string objectId, int category,  IEnumerable<UserRelationEntity> userRelationEntityList);

        /// <summary>
        /// 保存用户对应对象数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="category">分类:1-角色2-岗位-3部门</param>
        /// <param name="userRelationEntityList"></param>
        /// <returns></returns>
        Task SaveEntityList(string userId, string userName, int category,
            IEnumerable<UserRelationEntity> userRelationEntityList);

        #endregion
    }
}
