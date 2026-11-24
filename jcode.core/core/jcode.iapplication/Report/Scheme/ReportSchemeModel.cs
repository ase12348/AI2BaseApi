namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期：2022-05-05 10:40:00
    /// 描 述：lr_report_scheme表的实体
    /// </summary>
    public class ReportSchemeModel
    {
        /// <summary>
        /// 数据库编码
        /// </summary>
        public string F_DataSourceId { get; set; }
        /// <summary>
        /// 图表sql
        /// </summary>
        public string F_ChartSqlString { get; set; }
        /// <summary>
        /// 列表sql
        /// </summary>
        public string F_ListSqlString { get; set; }
    }
}
