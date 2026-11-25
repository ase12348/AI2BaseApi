using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：功能模块
    /// </summary>
    [MyTable("lr_base_module")]
    public class ModuleEntity
    {
        #region 实体成员
        /// <summary>
        /// 功能主键
        /// </summary>
        /// <returns></returns>
        [Column(IsPrimary = true)]
        public string F_ModuleId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        /// <returns></returns>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        /// <returns></returns>
        public string F_EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        public string F_FullName { get; set; }
        /// <summary>
        /// 图标
        /// </summary>
        /// <returns></returns>
        public string F_Icon { get; set; }
        /// <summary>
        /// 导航地址
        /// </summary>
        /// <returns></returns>
        public string F_UrlAddress { get; set; }
        /// <summary>
        /// 导航目标
        /// </summary>
        /// <returns></returns>
        public string F_Target { get; set; }
        /// <summary>
        /// 菜单选项
        /// </summary>
        /// <returns></returns>
        public int? F_IsMenu { get; set; }
        /// <summary>
        /// 允许展开
        /// </summary>
        /// <returns></returns>
        public int? F_AllowExpand { get; set; }
        /// <summary>
        /// 是否公开
        /// </summary>
        /// <returns></returns>
        public int? F_IsPublic { get; set; }
        /// <summary>
        /// 允许编辑
        /// </summary>
        /// <returns></returns>
        public int? F_AllowEdit { get; set; }
        /// <summary>
        /// 允许删除
        /// </summary>
        /// <returns></returns>
        public int? F_AllowDelete { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        /// <returns></returns>
        public int? F_DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        /// <returns></returns>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        /// <returns></returns>
        public string F_Description { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        /// <returns></returns>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        /// <returns></returns>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        /// <returns></returns>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        /// <returns></returns>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        /// <returns></returns>
        public string F_ModifyUserName { get; set; }
        
        /// <summary>
        /// 应用编码
        /// </summary>
        public string F_AppCode { get; set; }
        /// <summary>
        /// 终端类型 0电脑端 1移动端
        /// </summary>
        public int? F_TerminalType { get; set; }
        /// <summary>
        /// 功能分类（移动端对应数据字典 function 的明细值）
        /// </summary>
        public string F_Type { get; set; }
        /// <summary>
        /// 移动端功能方案（按钮/列表字段/表单字段 JSON）
        /// </summary>
        public string F_Scheme { get; set; }
        #endregion
        
        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否可编辑 0 表示不可编辑 其它值都可编辑
        /// </summary>
        public int? F_IsEdit { get; set; }
        #endregion
    }
}
