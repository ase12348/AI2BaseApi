using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单数据
    /// </summary>
    public class FormDataDto
    {
        /// <summary>
        /// 模板Id
        /// </summary>
        public string SchemeId { get; set; }
        /// <summary>
        /// 是否更新
        /// </summary>
        public bool IsUpdate { get; set; }
        /// <summary>
        /// 表单数据
        /// </summary>
        public string Data { get; set; }
        /// <summary>
        /// 主键（更新数据时采用）
        /// </summary>
        public string Pkey { get; set; }
        /// <summary>
        /// 主键值（更新数据时采用）
        /// </summary>
        public string PkeyValue { get; set; }
        /// <summary>
        /// 变更数据
        /// </summary>
        public List<FormHistoryEntity> DiffFormData { get; set; }
    }
}
