using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导入设置
    /// </summary>
    [MyTable("lr_excel_import")]
    public class ExcelImportEntity
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
        /// 关联模块Id
        /// </summary>
        public string F_ModuleId { get; set; }
        /// <summary>
        /// 关联按钮Id
        /// </summary>
        public string F_ModuleBtnId { get; set; }
        /// <summary>
        /// 按钮名称
        /// </summary>
        public string F_BtnName { get; set; }
        /// <summary>
        /// 导入数据库ID
        /// </summary>
        public string F_DbId { get; set; }
        /// <summary>
        /// 导入数据库表
        /// </summary>
        public string F_DbTable { get; set; }
        /// <summary>
        /// 错误处理机制0终止,1跳过
        /// </summary>
        public int? F_ErrorType { get; set; }
        /// <summary>
        /// 是否有效0暂停,1启用
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建人Id
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人名字
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
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
