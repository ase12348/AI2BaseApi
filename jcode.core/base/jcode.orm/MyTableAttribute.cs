using System;

namespace jcode.orm
{
    /// <summary>
    /// 自定义参数
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class MyTableAttribute : Attribute
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string Name { get;}

        /// <summary>
        /// 表备注
        /// </summary>
        public string TableDescription { get; set; }

        /// <summary>
        /// 是否禁用删除
        /// </summary>
        public bool IsDisabledDelete { get; set; }

        /// <summary>
        /// 是否禁用更新全部
        /// </summary>
        public bool IsDisabledUpdateAll { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="name"></param>
        public MyTableAttribute(string name)
        {
            this.Name = name;
        }
        public MyTableAttribute(string Name, string tableDescription)
        {
            this.Name = Name;
            this.TableDescription = tableDescription;
        }

        public MyTableAttribute(string Name, string tableDescription, bool isDisabledDelete)
        {
            this.Name = Name;
            this.TableDescription = tableDescription;
            this.IsDisabledDelete = isDisabledDelete;
        }

    }

    [AttributeUsage(AttributeTargets.Property, Inherited = true)]
    public class Column : Attribute
    {
        /// <summary>
        /// 创建数据库字段的名称（默认取实体类属性名称）
        /// </summary>
        private string _ColumnName;
        public string ColumnName
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }
        public string Name
        {
            get { return _ColumnName; }
            set { _ColumnName = value; }
        }

        /// <summary>
        /// ORM不处理该列
        /// </summary>
        private bool _IsIgnore;
        public bool IsIgnore
        {
            get { return _IsIgnore; }
            set { _IsIgnore = value; }
        }

        /// <summary>
        /// 是否创建主键标识
        /// </summary>
        private bool _IsPrimaryKey;
        public bool IsPrimaryKey
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }
        public bool IsPrimary
        {
            get { return _IsPrimaryKey; }
            set { _IsPrimaryKey = value; }
        }

        /// <summary>
        /// 是否创建自增标识
        /// 如果是Oracle请设置OracleSequenceName 设置后和自增一样使用
        /// </summary>
        private bool _IsIdentity;
        public bool IsIdentity
        {
            get { return _IsIdentity; }
            set { _IsIdentity = value; }
        }

        private string _MappingKeys;
        public string MappingKeys
        {
            get { return _MappingKeys; }
            set { _MappingKeys = value; }
        }

        /// <summary>
        /// 备注 表注释
        /// </summary>
        private string _ColumnDescription;
        public string ColumnDescription
        {
            get { return _ColumnDescription; }
            set { _ColumnDescription = value; }
        }

        /// <summary>
        /// 长度
        /// </summary>

        private int _Length;
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        /// <summary>
        /// 是否可以为null默为false
        /// </summary>
        private bool _IsNullable;
        public bool IsNullable
        {
            get { return _IsNullable; }
            set { _IsNullable = value; }
        }


        /// <summary>
        /// 修改列名用，这样不会新增或者删除列
        /// </summary>
        private string _OldColumnName;
        public string OldColumnName
        {
            get { return _OldColumnName; }
            set { _OldColumnName = value; }
        }

        /// <summary>
        /// 创建数据库字段的类型
        /// 用法1： “varchar(20)” 不需要设置长度
        /// 用法2:      不设置该参数   系统会根据C#类型自动生成相应的数据库类型 
        /// 用法3:     "nvarchar(20),text"  多个以逗号隔开 例如 B数据库不支持nvarchar那么会选择text
        /// </summary>
        private string _ColumnDataType;
        public string ColumnDataType
        {
            get { return _ColumnDataType; }
            set { _ColumnDataType = value; }
        }

        /// <summary>
        /// 精度 如 decimal(18,2) length=18,DecimalDigits=2
        /// </summary>
        private int _DecimalDigits;
        public int DecimalDigits
        {
            get { return _DecimalDigits; }
            set { _DecimalDigits = value; }
        }

        /// <summary>
        /// 设置Oracle序列，设置后该列等同于自增列
        /// </summary>
        private string _OracleSequenceName;
        public string OracleSequenceName
        {
            get { return _OracleSequenceName; }
            set { _OracleSequenceName = value; }
        }

        private bool _IsOnlyIgnoreInsert;
        public bool IsOnlyIgnoreInsert
        {
            get { return _IsOnlyIgnoreInsert; }
            set { _IsOnlyIgnoreInsert = value; }
        }

        private bool _IsOnlyIgnoreUpdate;
        public bool IsOnlyIgnoreUpdate
        {
            get { return _IsOnlyIgnoreUpdate; }
            set { _IsOnlyIgnoreUpdate = value; }
        }


        private bool _IsEnableUpdateVersionValidation;
        public bool IsEnableUpdateVersionValidation
        {
            get { return _IsEnableUpdateVersionValidation; }
            set { _IsEnableUpdateVersionValidation = value; }
        }



        private bool _IsTranscoding;
        public bool IsTranscoding
        {
            get { return _IsTranscoding; }
            set { _IsTranscoding = value; }
        }

        private bool _NoSerialize;
        public bool NoSerialize
        {
            get { return _NoSerialize; }
            set { _NoSerialize = value; }
        }

        private string _SerializeDateTimeFormat;
        public string SerializeDateTimeFormat
        {
            get { return _SerializeDateTimeFormat; }
            set { _SerializeDateTimeFormat = value; }
        }

        private bool _IsJson;
        public bool IsJson
        {
            get { return _IsJson; }
            set { _IsJson = value; }
        }

        /// <summary>
        /// 默认值
        /// </summary>
        private string _DefaultValue;
        public string DefaultValue
        {
            get { return _DefaultValue; }
            set { _DefaultValue = value; }
        }

        /// <summary>
        /// 创建索引用
        /// </summary>
        private string[] _IndexGroupNameList;
        public string[] IndexGroupNameList
        {
            get { return _IndexGroupNameList; }
            set { _IndexGroupNameList = value; }
        }

        /// <summary>
        /// 创建唯一索引
        /// </summary>
        public string[] UniqueGroupNameList { get; set; }

        private bool _IsArray;
        public bool IsArray
        {
            get { return _IsArray; }
            set { _IsArray = value; }
        }

    }


    [AttributeUsage(AttributeTargets.Class, Inherited = true)]
    public class TenantAttribute : Attribute
    {
        public object configId { get; set; }
        public TenantAttribute(object configId)
        {
            this.configId = configId;
        }
    }
}
