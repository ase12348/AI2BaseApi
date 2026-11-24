using System.Collections.Generic;
using System.Data;

namespace jcode.iapplication.Excel.Export
{
    /// <summary>
    /// 数据导出参数
    /// </summary>
    public class ExcelExportDto
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 列设置
        /// </summary>
        public List<TableColumnModel> Columns { get; set; }
        
        /// <summary>
        /// 表格头部信息
        /// </summary>
        public List<TableColumnModel> TableHeader{ get; set; }

        /// <summary>
        /// 数据请求地址
        /// </summary>
        public string DataUrl { get; set; }
        /// <summary>
        /// 是否分页
        /// </summary>
        public bool IsPage { get; set; }
        /// <summary>
        /// 是否是post请求，目前只有自定义表单才会用到
        /// </summary>
        public bool IsPost { get; set; }
        /// <summary>
        /// post请求的请求数据
        /// </summary>
        public string ReqData { get; set; }

        /// <summary>
        /// 需要导出的数据
        /// </summary>
        public DataTable DataList { get; set; }
    }
}
