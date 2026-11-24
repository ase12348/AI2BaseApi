using System;
using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.09.21
    /// 描 述：智能体管理
    /// </summary> 
    [MyTable("lr_base_agent")]
    public class AgentEntity
    {
        #region 实体成员 
        /// <summary> 
        /// 主键 
        /// </summary> 
        /// <returns></returns> 
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary> 
        /// 名称 
        /// </summary> 
        /// <returns></returns> 
        public string F_Name { get; set; }
        /// <summary> 
        /// 图标 
        /// </summary> 
        /// <returns></returns> 
        public string F_Icon { get; set; }
        /// <summary> 
        /// 描述 
        /// </summary> 
        /// <returns></returns> 
        public string F_Description { get; set; }
        /// <summary> 
        /// 是否内置 1/0 
        /// </summary> 
        /// <returns></returns> 
        public int? F_IsBuiltIn { get; set; }
        /// <summary> 
        /// 是否已发布 1/0 
        /// </summary> 
        /// <returns></returns> 
        public int? F_Published { get; set; }
        /// <summary>
        /// 创建日期
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
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_ModifyUserName { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
