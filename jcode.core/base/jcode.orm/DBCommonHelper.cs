using jcode.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using DbType = SqlSugar.DbType;

namespace jcode.orm
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.05
    /// 描 述：db通用帮助类
    /// </summary>
    public class DBCommonHelper
    {



        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="dbtype">SqlServer,Oracle,MySql,PostgreSQL,Dameng,Kdbndp</param>
        /// <returns></returns>
        public static DbType GetDbType(string dbtype)
        {
            switch (dbtype)
            {
                case "SqlServer":
                    return DbType.SqlServer;
                case "Oracle":
                    return DbType.Oracle;
                case "MySql":
                    return DbType.MySql;
                case "HG":
                    return DbType.HG;
                case "OpenGauss"://华为OpenGauss
                    return DbType.OpenGauss;
                case "PostgreSQL":
                    return DbType.PostgreSQL;
                case "Dameng":
                    return DbType.Dm;//达梦
                case "Kdbndp":
                    return DbType.Kdbndp;//人大金仓
                case "Sqlite":
                    return DbType.Sqlite;//Sqlite 
                case "Oscar":
                    return DbType.Oscar;//神通
                case "QuestDB":
                    return DbType.QuestDB;
                case "Tidb":
                    return DbType.Tidb;
                case "TDengine": // 需要安装驱动，3.x
                    return DbType.TDengine;
                case "OceanBase": // （mysql和oracle）推荐使用mysql
                    return DbType.OceanBase;
                case "Odbc":
                    return DbType.Odbc;
                case "ClickHouse":
                    return DbType.ClickHouse;
                case "GBase":
                    return DbType.GBase;

                default:
                    return DbType.MySql;
            }
        }

        /// <summary>
        /// 将IDataReader转换为DataTable  
        /// </summary>
        /// <returns>datatable</returns>
        /// <param name="reader">IDataReader</param>
        public static DataTable IDataReaderToDataTable(IDataReader reader)
        {
            using (reader)
            {
                DataTable objDataTable = new DataTable("Table");
                int intFieldCount = reader.FieldCount;
                for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
                {
                    objDataTable.Columns.Add(reader.GetName(intCounter).ToLower(), reader.GetFieldType(intCounter));
                }
                objDataTable.BeginLoadData();
                object[] objValues = new object[intFieldCount];
                while (reader.Read())
                {
                    reader.GetValues(objValues);
                    objDataTable.LoadDataRow(objValues, true);
                }
                reader.Close();
                reader.Dispose();
                objDataTable.EndLoadData();
                return objDataTable;
            }
        }

        /// <summary>
        /// 将datatable的字段格式化为小写
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static DataTable DtColToLow(DataTable dt)
        {
            foreach (DataColumn item in dt.Columns)
            {
                item.ColumnName = item.ColumnName.ToLower();
            }
            return dt;
        }

        /// <summary>
        /// 判断是否为匿名对象
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool CheckIfAnonymousType(Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");

            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType")
                && (type.Name.StartsWith("<>"))
                && (type.Attributes & TypeAttributes.NotPublic) == TypeAttributes.NotPublic;
        }

        /// <summary>
        /// 将不是匿名对象转化为字典数据
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static object ConvertToDic(object param)
        {
            //判断是否匿名，不是匿名就转一下，是就直接加进去
            if (param != null && !CheckIfAnonymousType(param.GetType()))
            {
                return param.ToObject<Dictionary<string, object>>();
            }
            return param;
        }

        /// <summary>
        /// 获取c#字段类型
        /// </summary>
        /// <param name="dbtype">数据库类型</param>
        /// <param name="dbType">数据库字段类型</param>
        /// <returns></returns>

        public static string GetCsType(DbType dbtype, string dbType)
        {
            string csType = "string";
            switch (dbType.ToLower())
            {
                case "varchar":
                case "varchar2":
                case "char":
                case "nchar":
                case "nvarchar":
                case "nvarchar2":
                    csType = "string";
                    break;
                case "text":
                case "ntext":
                case "longtext":
                case "clob":
                case "nclog":
                    csType = "text";
                    break;
                case "int":
                case "int2"://pgsql
                case "int4"://pgsql
                case "int8"://pgsql
                case "int32"://达梦
                case "integer":
                case "mediumint":// mysql
                case "year":
                    csType = "int";
                    break;
                case "bigint":
                    csType = "long";
                    break;
                case "smallint":
                    csType = "short";
                    break;
                case "bit": 
                case "boolean":
                    csType = "bool";
                    break;
                case "tinyint":
                    csType = "byte";
                    break;
                case "float4"://pgsql
                case "float8"://pgsql
                case "interval"://pgsql
                case "decimal":
                case "numeric":
                case "number":
                case "number(8,2)":
                case "money":
                case "smallmoney":
                    csType = "decimal";
                    break;
                case "real":
                case "double":
                    if (dbtype == DbType.MySql)
                    {
                        csType = "double";
                    }
                    else
                    {
                        csType = "float";
                    }
                    break;
                case "float":
                    if (dbtype == DbType.MySql)
                    {
                        csType = "float";
                    }
                    else
                    {
                        csType = "double";
                    }
                    break;
                case "date":
                case "datetime":
                case "datetime2":
                case "smalldatetime":
                    csType = "DateTime";
                    break;
                case "datetimeoffset":
                case "timestamptz":
                    csType = "DateTimeOffset";
                    break;
                case "time":
                    csType = "TimeSpan";
                    break;
                case "timestamp":
                    if (dbtype == DbType.MySql)
                    {
                        csType = "DateTime";

                    }
                    else if (dbtype == DbType.PostgreSQL)
                    {
                        csType = "DateTime";
                    }
                    else
                    {
                        csType = "byte[]";
                    }
                    break;
                case "binary":
                case "varbinary":
                case "image":

                case "tinyblob":
                case "blob":
                case "mediumblob":
                case "longblob":

                    csType = "byte[]";
                    break;
                case "uniqueidentifier":
                    csType = "Guid";
                    break;
                case "variant":
                    csType = "Object";
                    break;

                // mysql
                case "set":
                case "enum":
                    csType = "Enum";
                    break;
                case "point":
                case "linestring":
                case "polygon":
                case "geometry":
                case "multipoint":
                case "multilinestring":
                case "multipolygon":
                case "geometrycollection":
                    csType = "Geometry";
                    break;
            }

            return csType;
        }

        public static string GetDbType(DbType dbtype, string dbType, string csType)
        {
            if (csType == "text")
            {
                if (DbType.Oracle == dbtype)
                {
                    return "clob";
                }
                else
                {
                    if (!string.IsNullOrEmpty(dbType))
                    {
                        return dbType;
                    }
                    return "text";
                }
            }
            if (!string.IsNullOrEmpty(dbType))
            {
                return dbType;
            }
            return string.Empty;
        }

        public static string SqlFiltersToSidx(string source)
        {
            source = SqlFilters(source)?.Replace("\"", "");
            return source;
        }

        public static string SqlFilters(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            source = Regex.Replace(source, "/add ", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", " ( ", RegexOptions.IgnoreCase);
            //去除执行SQL语句的命令关键字
            source = Regex.Replace(source, " select ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }
        
        
        public static string SqlFilters2(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }
            source = Regex.Replace(source, "/add ", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", "( ", RegexOptions.IgnoreCase);
            //去除执行SQL语句的命令关键字
            // source = Regex.Replace(source, " select ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }

        #region whereSql 占位符与 whereParam 对齐校验（改进建议 2.2）

        /// <summary>
        /// 校验 whereSql 中的 @参数占位符是否都能在 whereParam 中找到同名参数。
        /// 背景：FindList 的 exp + whereSql + whereParam 重载会把未对齐的占位符内联成字面量 NULL，
        /// 拼出「where and (NULL is null or F_Title like NULL)」这类 MySQL 语法错 / 条件永远不生效的静默错误。
        /// 只在「有占位符且对不上」时抛 ArgumentException；无占位符、已是参数化对象一律放行，不影响存量调用。
        /// 两个 FindList 重载共用本方法（改进建议 2.2）。
        /// </summary>
        /// <param name="whereSql">原始 whereSql（建议在 SqlFilters2 之前传入）</param>
        /// <param name="whereParam">匿名对象 / POCO / IDictionary / SugarParameter 及其集合 / null</param>
        public static void ValidateWhereParameters(string whereSql, object whereParam)
        {
            if (string.IsNullOrWhiteSpace(whereSql))
            {
                return;
            }

            // 1) 提取占位符：先剥离字符串字面量与注释，再用 (?<!@) 排除 @@ 系统变量，保守处理避免误报。
            var scan = Regex.Replace(whereSql, "'(?:''|[^'])*'", " ");
            scan = Regex.Replace(scan, "--[^\r\n]*", " ");
            scan = Regex.Replace(scan, @"/\*[\s\S]*?\*/", " ");
            var placeholders = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Match m in Regex.Matches(scan, @"(?<!@)@([A-Za-z_][A-Za-z0-9_]*)"))
            {
                placeholders.Add(m.Groups[1].Value);
            }

            // whereSql 无占位符：存量无参调用必须放行（无论 whereParam 是什么）。
            if (placeholders.Count == 0)
            {
                return;
            }

            // 2) 已是参数化对象（SugarParameter 及其数组/集合）：底层本来就按参数化绑定，跳过校验避免误伤。
            if (ShouldSkipPlaceholderCheck(whereParam))
            {
                return;
            }

            // 3) 收集 whereParam 实际提供的名字（统一去掉开头的 @，大小写不敏感）。
            var provided = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (whereParam != null)
            {
                if (whereParam is IDictionary<string, object> genericDict)
                {
                    foreach (var key in genericDict.Keys)
                    {
                        AddParameterName(provided, key);
                    }
                }
                else if (whereParam is System.Collections.IDictionary dict)
                {
                    foreach (System.Collections.DictionaryEntry entry in dict)
                    {
                        AddParameterName(provided, entry.Key?.ToString());
                    }
                }
                else
                {
                    // 匿名对象 / POCO：反射取公开实例属性名。
                    foreach (var prop in whereParam.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                    {
                        AddParameterName(provided, prop.Name);
                    }
                }
            }

            // 4) 找出缺失的参数；whereParam 为 null 时 provided 为空，等价于「有占位符却没给参数」直接抛。
            var missing = new List<string>();
            foreach (var name in placeholders)
            {
                if (!provided.Contains(name))
                {
                    missing.Add(name);
                }
            }
            if (missing.Count == 0)
            {
                return;
            }

            // 5) 抛异常：点名缺失参数，并列出全部占位符与实际提供的名字，方便一次改对。
            var missingNames = string.Join("、", missing.ConvertAll(n => "@" + n));
            var allNames = string.Join("、", new List<string>(placeholders).ConvertAll(n => "@" + n));
            var providedNames = provided.Count == 0
                ? "(whereParam 为 null 或未提供任何参数)"
                : string.Join("、", provided);
            throw new ArgumentException(
                "whereSql 中的参数 " + missingNames + " 在 whereParam 中不存在；请把 whereParam 换成同名属性，" +
                "或改用 FindList(sql, param, pagination) 原生重载（多条件动态查询）。" +
                " whereSql 占位符：" + allNames + "；" +
                " whereParam 实际提供的名字：" + providedNames + "。" +
                "（改进建议 2.2：占位符未对齐时底层会把未匹配参数内联成 NULL，导致 MySQL 语法错或条件永远不生效的静默错误。）",
                nameof(whereParam));
        }

        /// <summary>把参数名归一化（去掉开头的 @，兼容宿主习惯的 @kw / kw 两种写法）后加入集合。</summary>
        private static void AddParameterName(HashSet<string> names, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return;
            }
            if (name[0] == '@')
            {
                name = name.Substring(1);
            }
            if (name.Length > 0)
            {
                names.Add(name);
            }
        }

        /// <summary>
        /// 判断是否应跳过 @占位符 对齐校验：已是参数化对象或集合时，底层按参数化绑定，
        /// 占位符不需要与反射属性名对齐，必须跳过以免误伤全参数化调用。
        /// 普通 POCO / 匿名对象 / IDictionary 返回 false，继续做对齐校验。
        /// </summary>
        private static bool ShouldSkipPlaceholderCheck(object whereParam)
        {
            if (whereParam == null)
            {
                return false;
            }
            if (whereParam is SqlSugar.SugarParameter)
            {
                return true;
            }
            // SugarParameter[] / List<SugarParameter> / IEnumerable<SugarParameter>
            if (whereParam is IEnumerable<SqlSugar.SugarParameter>)
            {
                return true;
            }
            // 其余可枚举对象（非字符串、非字典）也保守放行：POCO / 匿名对象 / IDictionary 都不会是 IEnumerable，
            // 这类弱类型集合（object[] / ArrayList 等）交给底层原生处理，避免反射出 Length 之类属性造成误抛。
            return whereParam is System.Collections.IEnumerable
                && !(whereParam is string)
                && !(whereParam is System.Collections.IDictionary);
        }

        #endregion

        #region 不同数据库参数格式化

        /// <summary>
        /// 转化修饰符和大小写
        /// </summary>
        /// <param name="name">字段名</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="pgSqlIsAutoToLower">是否强制转小写</param>
        /// <returns></returns>
        public static string GetTranslationColumnName(string name, DbType dbType, bool pgSqlIsAutoToLower)
        {
            var sqlTranslationLeft = string.Empty;
            var sqlTranslationRight = string.Empty;
            switch (dbType)
            {
                case DbType.MySql:
                case DbType.Sqlite:
                case DbType.OceanBase: // （mysql和oracle）推荐使用mysql
                case DbType.Tidb:
                    sqlTranslationLeft = "`";
                    sqlTranslationRight = "`";
                    break;
                case DbType.SqlServer:
                    sqlTranslationLeft = "[";
                    sqlTranslationRight = "]";
                    break;
                case DbType.Oracle:
                case DbType.Dm:
                case DbType.Kdbndp:
                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    name = name.ToUpper();
                    break;
                case DbType.PostgreSQL:
                case DbType.OpenGauss:
                    if (pgSqlIsAutoToLower)
                    {
                        name = name.ToLower();
                    }
                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    break;
                case DbType.Oscar:
                case DbType.HG:
                case DbType.ClickHouse:
                case DbType.QuestDB:
                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    break;
                case DbType.GBase:
                case DbType.Odbc:
                case DbType.TDengine:
                    break;
            }
            return $"{sqlTranslationLeft}{name}{sqlTranslationRight}";
        }

        /// <summary>
        /// 转化修饰符和大小写
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="pgSqlIsAutoToLower">是否强制转小写</param>
        /// <returns></returns>
        public static string GetTranslationColumnNameBySql(string sql, DbType dbType, bool pgSqlIsAutoToLower)
        {
            if (sql.IndexOf("[") == -1 || sql.IndexOf("]") == -1)
            {
                return sql;
            }
            
            // 创建正则表达式对象并设置模式
            Regex regex = new Regex(@"\[(\w+)\]"); // 这里使用 \w 匹配任意单词字符（包括数字、字母）

            var sqlTranslationLeft = string.Empty;
            var sqlTranslationRight = string.Empty;
            switch (dbType)
            {
                case DbType.MySql:
                case DbType.Sqlite:
                case DbType.OceanBase: // （mysql和oracle）推荐使用mysql
                case DbType.Tidb:
                    sqlTranslationLeft = "`";
                    sqlTranslationRight = "`";
                    break;
                case DbType.SqlServer:
                    sqlTranslationLeft = "[";
                    sqlTranslationRight = "]";
                    break;
                case DbType.Oracle:
                case DbType.Kdbndp:
                case DbType.Dm:
                    sql = regex.Replace(sql, match => match.Value.ToUpper());
                    // sql = sql.ToUpper();
                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    break;
                case DbType.PostgreSQL:
                case DbType.OpenGauss:
                    if (pgSqlIsAutoToLower)
                    {
                      
        
                        sql = regex.Replace(sql, match => match.Value.ToLower());

                        
                        // sql = sql.ToLower();
                    }
                    sql = sql.Replace("{jcode_sassid}", "{JCODE_SASSID}");
                    sql = sql.Replace("{jcode_sassid_nott}", "{JCODE_SASSID_NOTT}");
                    sql = sql.Replace("{jcode_sassid_nota}", "{JCODE_SASSID_NOTA}");

                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    break;
                case DbType.Oscar:
                case DbType.HG:
                case DbType.QuestDB:
                case DbType.ClickHouse:
                    sqlTranslationLeft = "\"";
                    sqlTranslationRight = "\"";
                    break;
                case DbType.GBase:
                case DbType.Odbc:
                case DbType.TDengine:
                    break;
            }

            if (sqlTranslationLeft == "[")
            {
                return sql;
            }

            sql = sql.Replace("[", sqlTranslationLeft)
                .Replace("]", sqlTranslationRight);
            return sql;
        }


        /// <summary>
        /// 根据数据库账密拼接连接字符串
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="server"></param>
        /// <param name="port"></param>
        /// <param name="userId"></param>
        /// <param name="password"></param>
        /// <param name="dataBase"></param>
        /// <returns></returns>
        public static string SpliceConnection(string dbType, string server, string port, string userId, string password, string dataBase)
        {
            switch (dbType)
            {
                case "SqlServer":
                    return $"server={server},{port};uid={userId};pwd={password};database={dataBase}";
                case "Sqlite":
                    // SQLite 是单文件数据库，不需要服务器/端口/账号；dataBase 直接作为 .db 文件路径
                    return $"DataSource={dataBase};Pooling=True;Cache=Shared";
                case "Oracle":
                    return $"Data Source={server}:{port}/{dataBase};User ID={userId};Password={password};";
                case "OceanBase":
                case "MySql":
                case "Tidb":
                    // 不带 SslMode：默认 Preferred 会协商 TLS，caching_sha2_password 完整认证在加密通道内完成；
                    // 强制 none 且服务端 SHA2 缓存未命中时，连口令错都会被报成 "caching_sha2_password failed"。
                    return $"server={server};Port={port};Database={dataBase};Uid={userId};Pwd={password};";
                case "HG"://瀚高
                    return $"PORT={port};DATABASE={dataBase};HOST={server};PASSWORD={password};USER ID={userId}";
                case "OpenGauss"://华为OpenGauss
                    return $"PORT={port};DATABASE={dataBase};HOST={server};PASSWORD={password};USER ID={userId};No Reset On Close=true";
                case "PostgreSQL":
                    return $"PORT={port};DATABASE={dataBase};HOST={server};PASSWORD={password};USER ID={userId}";
                case "Dameng"://达梦
                    return $"Server={server};port={port};User Id={userId}; PWD={password};DATABASE={dataBase}";
                case "Kdbndp"://人大金仓
                    return $"Server={server};Port={port};UID={userId};PWD={password};database={dataBase}";
                case "Oscar"://神通
                    return $"PORT={port};DATABASE={dataBase};HOST={server};PASSWORD={password};USER ID={userId};";
                case "QuestDB":
                    return $"host={server};port={port};username={userId};password={password};database={dataBase};ServerCompatibilityMode=NoTypeLoading;";
                case "GBase":
                    return "Driver={GBase ODBC DRIVER (64-Bit)};" + $"Host={server};Service={port};Server=gbase01;Database={dataBase};Protocol=onsoctcp;Uid={userId};Pwd={password};Db_locale=zh_CN.utf8;Client_locale=zh_CN.utf8";
                case "TDengine":
                    return $"Host={server};Port={port};Username={userId};Password={password};Database={dataBase}";
                case "ClickHouse":
                    return $"Database={dataBase};Username={userId};Password={password};Host={server};Port={port};Compression=False;UseSession=False;Timeout=120;";
                default:
                    return $"server={server};Port={port};Database={dataBase};Uid={userId};Pwd={password};";
            }
        }
        /// <summary>
        /// 处理PgSql的大小写问题
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public static (bool PgSqlIsAutoToLower, string _connString) GetPgSqlIsAutoToLower(string connString)
        {
            bool PgSqlIsAutoToLower = true;
            string _connString = connString;
            if (!string.IsNullOrEmpty(connString))
            {
                if (Regex.IsMatch(connString.ToLower(), "pgsqlisautotolower="))
                {
                    var regValue = Regex.Match(connString.ToLower(), @"pgsqlisautotolower\=(\w+)").Groups[1].Value;
                    if (regValue.ToLower() == "false")
                    {
                        PgSqlIsAutoToLower = false;
                    }
                    _connString = _connString.Substring(0, _connString.LastIndexOf("PgSqlIsAutoToLower"));
                }
            }
            return (PgSqlIsAutoToLower, _connString);
        }


        public static string SqlFormat(string sql, DbType dbType)
        {
            if (dbType == DbType.Dm)
            {
                return sql.Replace("@", ":");
            }
            return sql;
        }

        #endregion
    }
}
