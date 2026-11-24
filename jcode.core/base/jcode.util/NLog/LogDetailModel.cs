namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2023.03.31
    /// 描 述：审计日志配置详情实体类
    /// </summary>
    public class LogDetailModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        public string FullName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string EnCode { get; set; }
        /// <summary>
        /// 关联类型0:数据字典;1:操作人ID;2:操作人名字
        /// </summary>
        public int? RelationType { get; set; }
        /// <summary>
        /// 数据字典编号
        /// </summary>
        public string DataItemCode { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 数据视图编码
        /// </summary>
        public string F_DSourceCode { get; set; }
        /// <summary>
        /// 关联字段
        /// </summary>
        public string F_DSourceRelevance { get; set; }
        /// <summary>
        /// 显示字段
        /// </summary>
        public string F_DSourceReveal { get; set; }

    }
}
