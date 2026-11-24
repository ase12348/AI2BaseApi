using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2024.04.18
    /// 描 述：页面模板信息
    /// </summary>
    [MyTable("lr_index_schemeinfo")]
    public class IndexSchemeInfoEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 菜单id
        /// </summary>
        public string F_ModuleId{ get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string F_Category { get; set; }
        /// <summary>
        /// 当前模板主键
        /// </summary>
        public string F_SchemeId { get; set; }
        /// <summary>
        /// 发布的版本
        /// </summary>
        public string F_ReleaseId { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
        /// </summary>
        public string F_AppId { get; set; }
        #endregion

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


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion

    }
}
