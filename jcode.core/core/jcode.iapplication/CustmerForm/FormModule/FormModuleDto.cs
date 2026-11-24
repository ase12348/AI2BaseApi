using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 提交参数
    /// </summary>
    public class FormModuleDto
    {
        /// <summary>
        /// 设计数据
        /// </summary>
        public FormModuleEntity Entity { get; set; }
        /// <summary>
        /// 功能模块实体
        /// </summary>
        public ModuleEntity ModuleEntity { get; set; }
    }
}
