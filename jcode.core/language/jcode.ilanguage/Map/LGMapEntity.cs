using jcode.orm;

namespace jcode.ilanguage
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言映射
    /// </summary>
    [MyTable("lr_lg_map")]
    public class LGMapEntity 
    {
        #region 实体成员
        /// <summary>
        /// 显示内容
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 编码(系统自动产生，作为关联项)
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 对应语言显示编码
        /// </summary>
        public string F_TypeCode { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}

