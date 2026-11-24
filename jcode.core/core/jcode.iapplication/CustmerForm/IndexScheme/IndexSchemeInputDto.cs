using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 自定义页面模板保存数据
    /// </summary>
    public class IndexSchemeInputDto
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
