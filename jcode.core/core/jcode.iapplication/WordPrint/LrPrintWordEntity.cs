using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2023-05-06 14:06:35
    /// 描 述：lr_print_word(打印模版表)表的实体
    /// </summary>
    [MyTable("lr_print_word")]
    public class LrPrintWordEntity
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
        /// 名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string F_Category { get; set; }
        
        /// <summary>
        /// word文件id，支持docx
        /// </summary>
        public string F_FileId { get; set; }
        
        /// <summary>
        /// 是否禁用 0 是 1不是
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 创建者id
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建者名称
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDateTime { get; set; }
        /// <summary>
        /// 最后修改者ID
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 最后修改者名称
        /// </summary>
        public string F_ModifyUserName { get; set; }
        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? F_ModifyDateTime { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
        /// </summary>
        public string F_AppId { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建时间(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateDateTimeQRange { get; set; }
        /// <summary>
        /// 最后修改时间(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_ModifyDateTimeQRange { get; set; }
        
        /// <summary>
        /// 关键字
        /// </summary>
        [Column(IsIgnore = true)]
        public string Keyword { get; set; }

        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}