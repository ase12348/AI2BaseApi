using System.Collections.Generic;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.23
    /// 描 述：表格属性模型
    /// </summary>
    public class JfGridModel
    {
        /// <summary>
        /// 绑定字段名
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 显示名称
        /// </summary>
        public string label { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int width { get; set; }
        /// <summary>
        /// 对齐方式
        /// </summary>
        public string align { get; set; }
        /// <summary>
        /// 高度
        /// </summary>
        public int height { get; set; }
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public string hidden { get; set; }
        /// <summary>
        /// 子集
        /// </summary>
        public IEnumerable<JfGridModel> children { get; set; }
    }
}
