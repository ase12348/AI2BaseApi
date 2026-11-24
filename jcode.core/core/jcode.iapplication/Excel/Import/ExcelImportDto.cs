using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 提交参数
    /// </summary>
    public class ExcelImportDto
    {
        /// <summary>
        /// Excel数据导入设置
        /// </summary>
        public ExcelImportEntity Entity { get; set; }
        /// <summary>
        /// Excel数据导入设置字段
        /// </summary>
        public List<ExcelImportFieldEntity> List { get; set; }
    }
}
