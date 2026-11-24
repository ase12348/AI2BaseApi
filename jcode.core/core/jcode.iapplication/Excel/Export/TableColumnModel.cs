using System.Collections.Generic;

namespace jcode.iapplication.Excel.Export
{
    /// <summary>
    /// 表格列设置值
    /// </summary>
    public class TableColumnModel
    {
        /// <summary>
        /// 列名称
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 列id
        /// </summary>
        public string Prop { get; set; }

        /// <summary>
        /// 对齐方式
        /// </summary>
        public string Align { get; set; }
        /// <summary>
        /// 最小宽度
        /// </summary>
        public int? MinWidth { get; set; }
        /// <summary>
        /// 宽度
        /// </summary>
        public int? Width { get; set; }
        
        /// <summary>
        /// 是否隐藏
        /// </summary>
        public bool isHidden { get; set; }
        
        
        /// <summary>
        /// x坐标
        /// </summary>
        /// <value></value>
        public int X { get; set; }
        /// <summary>
        /// y坐标
        /// </summary>
        /// <value></value>
        public int Y { get; set; }
        /// <summary>
        /// 列合并
        /// </summary>
        public int? ColSpan{ get; set; }
        /// <summary>
        /// 行合并
        /// </summary>
        public int? RowSpan{ get; set; }
        /// <summary>
        /// 列配置
        /// </summary>
        public TableColumnConfig Config { get; set; }
    }

    /// <summary>
    /// 列配置
    /// </summary>
    public class TableColumnConfig
    {
        /// <summary>
        /// 数据类型mydata，dataItem，dataSource，datetime，company，department，user
        /// </summary>
        public string DataType { get; set; }
        /// <summary>
        /// 数据字典或者数据源编码
        /// </summary>
        public string DataCode { get; set; }
        /// <summary>
        /// 数据源值的字段名
        /// </summary>
        public string ValueKey { get; set; }
        /// <summary>
        /// 数据源显示值字段名
        /// </summary>
        public string LabelKey { get; set; }
        /// <summary>
        /// 日期数据格式
        /// </summary>
        public string Format { get; set; }
        /// <summary>
        /// 下拉框数据值
        /// </summary>
        public List<TableDataOptions> Options { get; set; }
    }

    /// <summary>
    /// 下拉框数据
    /// </summary>
    public class TableDataOptions { 
        /// <summary>
        /// 显示的数据值
        /// </summary>
        public string Label { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string Value { get; set; }
    }
}
