using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-05-09 14:13:34
    /// 描 述：lr_print_template(打印模板【lr_print_template】)表的实体
    /// </summary>
    [MyTable("lr_print_template")]
    public class LrPrintTemplateEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 模板信息主键
        /// </summary>
        public string F_TemplateInfoId { get; set; }
        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 组件json
        /// </summary>
        public string F_Component { get; set; }
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
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }

        #endregion



        #region 扩展属性
        /// <summary>
        /// 创建日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateDateQRange { get; set; }
        #endregion
    }
}