using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2024.04.18
    /// 描 述：页面模板
    /// </summary>
    [MyTable("lr_index_scheme")]
    public class IndexSchemeEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }

        /// <summary>
        /// 版本号
        /// </summary>
        public string F_Ver { get; set; }
        /// <summary>
        /// 版本描述
        /// </summary>
        public string F_Des { get; set; }

        /// <summary>
        /// 模板信息主键
        /// </summary>
        public string F_SchemeInfoId { get; set; }
        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 模板
        /// </summary>
        public string F_Scheme { get; set; }
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
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
