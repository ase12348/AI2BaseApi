using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单历史记录实体
    /// add by torres 20210902
    /// </summary>
    [MyTable("lr_form_datahistory")]
    public class FormHistoryEntity
    {
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string Table_Name { get; set; }
        /// <summary>
        /// 表主键
        /// </summary>
        public string TablePKey { get; set; }
        /// <summary>
        /// 表主键的值
        /// </summary>
        public string TablePKeyValue { get; set; }
        /// <summary>
        /// 更新表的字段
        /// </summary>
        public string TableField { get; set; }
        /// <summary>
        /// 表单id
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 表单信息id
        /// </summary>
        public string SchemeInfoId { get; set; }
        /// <summary>
        /// 表单显示名
        /// </summary>
        public string FormColumn { get; set; }
        /// <summary>
        /// 旧值（value）
        /// </summary>
        public string OldVal { get; set; }
        /// <summary>
        /// 新值（value）
        /// </summary>
        public string NewVal { get; set; }
        /// <summary>
        /// 旧显示值
        /// </summary>
        public string OldShowVal { get; set; }
        /// <summary>
        /// 新显示值
        /// </summary>
        public string NewShowVal { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? CreaterTime { get; set; }


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
