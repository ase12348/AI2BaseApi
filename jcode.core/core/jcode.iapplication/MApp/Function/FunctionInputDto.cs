using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.03.02
    /// 描 述：移动功能模信息
    /// </summary>
    public class FunctionInputDto
    {
        /// <summary>
        /// 模块基础信息
        /// </summary>
        public FunctionEntity FunctionEntity { get;set;}
        /// <summary>
        /// 按钮信息
        /// </summary>
        public List<ModuleItemEntity> ModuleItemEntitys { get; set; }
    }
}
