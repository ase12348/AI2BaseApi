using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.05.07
    /// 描 述：自定义表单功能
    /// </summary>
    [MyTable("lr_form_module")]
    public class FormModuleEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 自定义表单编码
        /// </summary>
        public string F_FormCode { get; set; }
        /// <summary>
        /// 表单版本
        /// </summary>
        public string F_FormVerison { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        public string F_Icon { get; set; }
        /// <summary>
        /// 功能名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 1.pc 2 移动
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 上级功能
        /// </summary>
        public string F_PModuleId { get; set; }
        /// <summary>
        /// 对应菜单id
        /// </summary>
        public string F_ModuleId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int?  F_SortCode { get; set; }
        /// <summary>
        /// 状态 1 发布 2未发布
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string F_Description { get; set; }
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
        /// <summary>
        /// 发布的设置信息
        /// </summary>
        public string F_Scheme { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否租户功能 1是 其它都不是
        /// </summary>
        public int? F_IsMain { get; set; }
        #endregion
    }
}
