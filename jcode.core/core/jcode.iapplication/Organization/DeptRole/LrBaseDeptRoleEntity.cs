using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2024-05-07 11:16:00
    /// 描 述：lr_base_dept_role(部门角色表)表的实体
    /// </summary>
    [MyTable("lr_base_dept_role")]
    public class LrBaseDeptRoleEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true,ColumnName = "F_Id")]
        public string FId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [Column(ColumnName = "F_Code")]
        public string FCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Column(ColumnName = "F_Name")]
        public string FName { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [Column(ColumnName = "F_Category")]
        public string FCategory { get; set; }
        /// <summary>
        /// 是否可用 1 是 0 不是
        /// </summary>
        [Column(ColumnName = "F_EnabledMark")]
        public int? FEnabledMark { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        [Column(ColumnName = "F_SortCode")]
        public int? FSortCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Column(ColumnName = "F_Des")]
        public string FDes { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column(ColumnName = "F_CreateDate")]
        public DateTime? FCreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        [Column(ColumnName = "F_CreateUserId")]
        public string FCreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column(ColumnName = "F_CreateUserName")]
        public string FCreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Column(ColumnName = "F_ModifyDate")]
        public DateTime? FModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        [Column(ColumnName = "F_ModifyUserId")]
        public string FModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        [Column(ColumnName = "F_ModifyUserName")]
        public string FModifyUserName { get; set; }
        /// <summary>
        /// 租户Id
        /// </summary>
        [Column(ColumnName = "F_TenantId")]
        public string FTenantId { get; set; }

        #endregion
        
    }
}