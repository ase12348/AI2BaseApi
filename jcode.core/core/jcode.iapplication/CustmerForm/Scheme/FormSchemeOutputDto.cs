namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单模板获取数据
    /// </summary>
    public class FormSchemeOutputDto
    {
        /// <summary>
        /// 模板基础信息
        /// </summary>
        public FormSchemeInfoEntity Info { get; set; }
        /// <summary>
        /// 模板信息
        /// </summary>
        public FormSchemeEntity Scheme { get; set; }
    }
}
