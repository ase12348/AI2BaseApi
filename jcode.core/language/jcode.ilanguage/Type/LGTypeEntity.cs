using jcode.orm;

namespace jcode.ilanguage
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言类型
    /// </summary>
    [MyTable("lr_lg_type")]
    public class LGTypeEntity 
    {
        #region 实体成员
        /// <summary>
        /// 主键ID
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 语言名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 语言编码（不予许重复）
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 是否是主语言0不是1是
        /// </summary>
        public int? F_IsMain { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}

