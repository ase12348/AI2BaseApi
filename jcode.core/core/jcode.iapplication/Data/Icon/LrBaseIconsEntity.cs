using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2024-03-01 10:15:24
    /// 描 述：lr_base_icons(图标保存)表的实体
    /// </summary>
    [MyTable("lr_base_icons")]
    public class LrBaseIconsEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true,ColumnName = "F_Id")]
        public string F_Id { get; set; }
        /// <summary>
        /// 高
        /// </summary>
        [Column(ColumnName = "F_Height")]
        public int? F_Height { get; set; }
        /// <summary>
        /// 宽
        /// </summary>
        [Column(ColumnName = "F_Width")]
        public int? F_Width { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Column(ColumnName = "F_Name")]
        public string F_Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        [Column(ColumnName = "F_Code")]
        public string F_Code { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        [Column(ColumnName = "F_Category")]
        public string F_Category { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        [Column(ColumnName = "F_CreateDate")]
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        [Column(ColumnName = "F_CreateUserId")]
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        [Column(ColumnName = "F_CreateUserName")]
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        [Column(ColumnName = "F_ModifyDate")]
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        [Column(ColumnName = "F_ModifyUserId")]
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        [Column(ColumnName = "F_ModifyUserName")]
        public string F_ModifyUserName { get; set; }
        /// <summary>
        /// svg数据
        /// </summary>
        [Column(ColumnName = "F_Body")]
        public string F_Body { get; set; }
        #endregion
        
        /// <summary>
        /// 查询关键字
        /// </summary>
        [Column(IsIgnore = true)]
        public string Keyword{ get; set; }
    }
}