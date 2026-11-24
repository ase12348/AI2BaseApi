namespace jcode.orm
{

    /// <summary>
    /// 属性和列
    /// </summary>
    public class PropertyGen
    {
        /// <summary>
        /// 属性名
        /// </summary>
        public string PropertyName { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string DbColumnName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }
        /// <summary>
        /// 自增列 
        /// </summary>
        public bool IsIdentity { get; set; }
        /// <summary>
        /// 备注 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 属性类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 是否是为NULL
        /// </summary>
        public bool IsNullable { get; set; }
        /// <summary>
        /// 精度
        /// </summary>
        public int? DecimalDigits { get; set; }
        /// <summary>
        /// 长度
        /// </summary>
        public int? Length { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DbType { get; set; }
        /// <summary>
        /// 是否忽略
        /// </summary>
        public bool IsIgnore { get; set; }
        /// <summary>
        /// 特殊类型
        /// </summary>
        public bool IsSpecialType { get; set; }
        /// <summary>
        /// 针对类型不需要更新
        /// </summary>
        public bool IsNotUpdate { get; set; }

    }
}
