using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：用户关联对象
    /// </summary>
    [MyTable("lr_base_userrelation")]
    public class UserRelationEntity
    {
        #region 实体成员
        /// <summary>
        /// 用户关系主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 用户主键
        /// </summary>
        public string F_UserId { get; set; }
        /// <summary>
        /// 分类:1-角色2-岗位-3部门角色
        /// </summary>
        public int? F_Category { get; set; }
        
        /// <summary>
        /// 名称
        /// </summary>
        public string F_Name{ get; set; }
        /// <summary>
        /// 组织架构扩展角色属性编码或者岗位主岗或者兼容岗
        /// </summary>
        public string F_AttrCode{ get; set; }
        /// <summary>
        /// 分类 f_userid 代表的意义 1.用户 2.部门 3.密级 4.行政级别 5.行政职务 6.技术职务 7.职称等级 8.管理职务 9.集成角色  10.限定部门 11.排除 部门
        /// </summary>
        public int? F_Type { get; set; }
        
        /// <summary>
        /// 对象主键
        /// </summary>
        public string F_ObjectId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        #endregion
        
        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
