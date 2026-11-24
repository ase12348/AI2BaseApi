using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-03-28 16:23:13
    /// 描 述：日志接口配置表单提交参数
    /// </summary>
    public class LogConfigDto
    {
        /// <summary>
        /// lr_other_logconfig(日志接口配置【lr_other_logconfig】)表的实体
        /// </summary>
        public LrOtherLogconfigEntity LrOtherLogconfigEntity { get; set; }
        /// <summary>
        /// lr_other_logscheme(日志接口配置-详情【lr_other_logscheme】)表的实体
        /// </summary>
        public IEnumerable<LrOtherLogschemeEntity> LrOtherLogschemeList { get; set; }

    }
}