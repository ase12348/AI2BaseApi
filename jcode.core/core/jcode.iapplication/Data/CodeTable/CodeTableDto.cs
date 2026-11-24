using SqlSugar;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace jcode.iapplication
{
    /// <summary>
    /// 数据库操作中间类
    /// </summary>
    public class CodeTableDto
    {
        /// <summary>
        /// 导入的数据表字符串
        /// </summary>
        public string CodeTableStr { get; set; }

        /// <summary>
        /// 需要导入表数据
        /// </summary>
        [Required(ErrorMessage = "请选择导入的数据表")]
        public List<DbTableInfo> TableList { get; set; }

    }
}
