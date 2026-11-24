using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导出设置
    /// </summary>
    [MyTable("lr_excel_export")]
    public class ExcelExportEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键Id
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 绑定的JQgirdId
        /// </summary>
        public string F_GridId { get; set; }
        /// <summary>
        /// 功能模块Id
        /// </summary>
        public string F_ModuleId { get; set; }
        /// <summary>
        /// 按钮Id
        /// </summary>
        public string F_ModuleBtnId { get; set; }
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string F_BtnName { get; set; }
        /// <summary>
        /// 数据接口地址
        /// </summary>
        public string F_Url { get; set; }
        /// <summary>
        /// 是否有效
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 是否导出勾选部分
        /// </summary>
        public int? F_IsCheck { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建人ID
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
        /// 修改人Id
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改人名称
        /// </summary>
        public string F_ModifyUserName { get; set; }
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
