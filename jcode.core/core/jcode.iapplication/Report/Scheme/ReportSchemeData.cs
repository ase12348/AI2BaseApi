using System.Data;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期：2022-05-05 10:40:00
    /// 描 述：lr_report_scheme表的实体
    /// </summary>
    public class ReportSchemeData
    {
        /// <summary>
        /// 报表主键
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 图表数据
        /// </summary>
        public DataTable ChartData { get; set; }
        /// <summary>
        /// 图标类型
        /// </summary>
        public string ChartType { get; set; }
        /// <summary>
        /// 列表数据
        /// </summary>
        public DataTable ListData { get; set; }
        /// <summary>
        /// 报表风格 1 列表 2 图表 3 混和
        /// </summary>
        public string TempStyle { get; set; }
    }
}
