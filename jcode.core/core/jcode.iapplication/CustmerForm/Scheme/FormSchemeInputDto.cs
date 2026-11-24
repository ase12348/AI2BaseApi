using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 自定义表单模板保存数据
    /// </summary>
    public class FormSchemeInputDto
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
