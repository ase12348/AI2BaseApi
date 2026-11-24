using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：授權功能
    /// </summary>
    public interface AuthorizeIBLL: IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取被授权的功能的主键字串
        /// </summary>
        /// <param name="objectId">对象主键（角色,用户）</param>
        /// <param name="itemType">项目类型:1-菜单2-按钮3-视图</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        Task<IEnumerable<AuthorizeEntity>> GetList(string objectId, int itemType, string parentId);
        /// <summary>
        /// 获取被授权的功能的主键字串
        /// </summary>
        /// <param name="objectIds">对象主键串（角色,用户）</param>
        /// <param name="itemType">功能类型1菜单功能2按钮3视图列表 4表单  5 移动端功能 6 移动功能按钮 7 移动功能列表 8 移动功能表单字段</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetItemIdListByobjectIds(string[] objectIds, int itemType, string parentId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存授权信息
        /// </summary>
        /// <param name="id">角色或用户id</param>
        /// <param name="authorizeDto">授权数据</param>
        /// <returns></returns>
        Task SaveAuthorize(string id, AuthorizeDto authorizeDto);


        /// <summary>
        /// 删除无效项
        /// </summary>
        /// <param name="itemIds">有效项</param>
        /// <param name="itemType">功能类型1菜单功能2按钮3视图列表4表单5App</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        Task RemoveInvalid(List<string> itemIds, int itemType, string parentId);
        #endregion
    }
}
