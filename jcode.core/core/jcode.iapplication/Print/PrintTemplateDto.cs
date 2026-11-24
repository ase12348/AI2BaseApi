namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-05-09 14:13:34
    /// 描 述：打印设计-打印模板表单提交参数
    /// </summary>
    public class PrintTemplateDto
    {
        /// <summary>
        /// lr_print_template(打印模板【lr_print_template】)表的实体
        /// </summary>
        public LrPrintTemplateEntity LrPrintTemplateEntity { get; set; }
        /// <summary>
        /// lr_print_templateinfo(打印模板信息【lr_print_templateinfo】)表的实体
        /// </summary>
        public LrPrintTemplateinfoEntity LrPrintTemplateinfoEntity { get; set; }

    }
}