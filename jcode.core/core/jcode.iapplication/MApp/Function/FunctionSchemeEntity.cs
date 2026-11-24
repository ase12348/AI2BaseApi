using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：移动端功能管理
    /// </summary>
    [MyTable("lr_app_fnscheme")]
    public class FunctionSchemeEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 功能模板
        /// </summary>
        /// <returns></returns>
        public string F_Scheme { get; set; }
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
