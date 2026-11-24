using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单查询方法
    /// </summary>
    public class FormQueryDto
    {
        /// <summary>
        /// 分页参数
        /// </summary>
        public PaginationInputDto PaginationInputDto { get; set; }
        /// <summary>
        /// 查询参数
        /// </summary>
        public string QueryJson { get; set; }
        /// <summary>
        /// 桌面参数
        /// </summary>
        public string Ldparam { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string Sidx { get; set; }

    }
}
