using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：授權功能
    /// </summary>
    [MyTable("lr_base_authorize")]
    public class AuthorizeEntity
    {
        #region 实体成员
        /// <summary>
        /// 对象分类:1-角色2-用户
        /// </summary>
        public int? F_ObjectType { get; set; }
        /// <summary>
        /// 对象主键
        /// </summary>
        public string F_ObjectId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 项目类型:1-菜单（10反向） 2-按钮（反向20） 3-视图（反向30） 4-表单（反向40） 5-移动功能（反向50） 6 移动功能按钮（反向60） 7 移动功能列表（反向70） 8 移动功能表单字段（反向80） 
        /// </summary>
        public int? F_ItemType { get; set; }
        /// <summary>
        /// 项目主键
        /// </summary>
        public string F_ItemId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 权限类型，目前表单权限适用;1可见2可编辑
        /// </summary>
        public int? F_Type { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
