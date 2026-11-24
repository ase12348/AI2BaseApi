using System;
using System.Collections.Generic;
using System.Text;

namespace jcode.iapplication.Data.CodeTable
{
    /// <summary>
    /// 自动建表用初始化字段信息
    /// </summary>
    public class CodeTableInitialColumnModel
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string DbColumnName { get; set; }
        /// <summary>
        /// 是否为空
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// C#字段类型
        /// </summary>
        public string CsType { get; set; }
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int? DecimalDigits { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Description { get; set; }
    }
}
