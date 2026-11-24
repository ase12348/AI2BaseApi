using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.03.02
    /// 描 述：移动功能模块子项 按钮 列表 表单
    /// </summary>
    [MyTable("lr_app_moduleitem")]
    public class ModuleItemEntity
    {
        #region 实体成员
        /// <summary>
        /// 按钮主键
        /// </summary>
        /// <returns></returns>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 功能主键
        /// </summary>
        /// <returns></returns>
        public string F_ModuleId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        /// <returns></returns>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 数据类型 1 按钮 2 列表 3 表单
        /// </summary>
        /// <returns></returns>
        public int F_Type { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        /// <returns></returns>
        public string F_Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        /// <returns></returns>
        public string F_Name { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        /// <returns></returns>
        public int? F_SortCode { get; set; }
        #endregion
        
        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否租户功能 1是 其它都不是
        /// </summary>
        public int? F_IsMain { get; set; }
        #endregion
    }
}
