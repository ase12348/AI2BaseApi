using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2024-03-01 10:15:24
    /// 描 述：icons 图标数据
    /// </summary>
    public class IconDto
    {
        /// <summary>
        /// 版本号
        /// </summary>
        public string Ver { get; set; }
        /// <summary>
        /// 图标集合
        /// </summary>
        public IEnumerable<LrBaseIconsEntity> List { get; set; }
    }
}