namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单模板获取数据
    /// </summary>
    public class IndexSchemeOutputDto
    {
        /// <summary>
        /// 模板基础信息
        /// </summary>
        public IndexSchemeInfoEntity Info { get; set; }
        /// <summary>
        /// 模板信息
        /// </summary>
        public IndexSchemeEntity Scheme { get; set; }
    }
}
