using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-05-09 14:13:34
    /// 描 述：lr_print_templateinfo(打印模板信息【lr_print_templateinfo】)表的实体
    /// </summary>
    [MyTable("lr_print_templateinfo")]
    public class LrPrintTemplateinfoEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 当前模板主键
        /// </summary>
        public string F_TemplateId { get; set; }
        /// <summary>
        /// 模板名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 模板编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 模板分类
        /// </summary>
        public string F_Category { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 状态1启用0未启用
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
        /// </summary>
        public string F_AppId { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        [Column(IsIgnore = true)]
        public int? F_Type { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(IsIgnore = true)]
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 关键字段
        /// </summary>
        [Column(IsIgnore = true)]
        public string Keyword { get; set; }
        #endregion
    }
}