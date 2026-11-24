using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期：2022-05-05 10:40:00
    /// 描 述：lr_report_scheme表的实体
    /// </summary>
    [MyTable("lr_report_scheme")]
    public class LrReportSchemeEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 报表分类
        /// </summary>
        public string F_TempCategory { get; set; }
        /// <summary>
        /// 报表类型1列表 2图标 3混合
        /// </summary>
        public string F_TempStyle { get; set; }
        /// <summary>
        /// 图标类型
        /// </summary>
        public string F_TempType { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 设置参数
        /// </summary>
        public string F_ParamJson { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改人id
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改人姓名
        /// </summary>
        public string F_ModifyUserName { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateDateQRange { get; set; }
        /// <summary>
        /// 修改日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_ModifyDateQRange { get; set; }

        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}