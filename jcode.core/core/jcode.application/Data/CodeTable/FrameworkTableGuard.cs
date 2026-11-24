using System;
using System.Collections.Generic;
using System.Linq;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：宿主自有表判据——这类表由平台实体拥有，不允许登记进数据表模型，也不允许由模型反向改结构
    /// </summary>
    public static class FrameworkTableGuard
    {
        /// <summary>
        /// 平台实体映射（[MyTable]）与按日期分表的表名都落在这两个前缀里
        /// </summary>
        private static readonly string[] OwnPrefixes = { "lr_", "api_" };

        /// <summary>
        /// 是否宿主自有表（按 trim + 小写比较）
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static bool IsFrameworkTable(string tableName)
        {
            if (string.IsNullOrWhiteSpace(tableName))
            {
                return false;
            }
            var name = tableName.Trim().ToLowerInvariant();
            return OwnPrefixes.Any(p => name.StartsWith(p));
        }

        /// <summary>
        /// 挑出清单里的宿主自有表（保持原样返回，方便把名字还给操作者）
        /// </summary>
        /// <param name="tableNames">表名清单</param>
        /// <returns></returns>
        public static List<string> PickFrameworkTables(IEnumerable<string> tableNames)
        {
            if (tableNames == null)
            {
                return new List<string>();
            }
            return tableNames.Where(t => !string.IsNullOrWhiteSpace(t))
                .Where(IsFrameworkTable)
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        /// <summary>
        /// 名字里带这些字符就没法安全拼进 DDL：表名一旦登记进模型，后面「更新到表」会拿它拼语句
        /// </summary>
        private static readonly char[] DdlUnsafeChars = { '\'', '"', '`', ';', '[', ']', '\\', '/', ' ', '\t', '\r', '\n' };

        /// <summary>
        /// 表名是否带 DDL 危险字符（只拦拼装用的分隔符，不拦中文表名）
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public static bool HasDdlUnsafeChar(string tableName)
        {
            if (string.IsNullOrEmpty(tableName))
            {
                return false;
            }
            return tableName.IndexOfAny(DdlUnsafeChars) >= 0 || tableName.Contains("--");
        }
    }
}
