using System;
using System.Linq;

namespace jcode.web.util.Swagger
{
    /// <summary>
    /// Swagger schemaId 规则：程序集名 + 命名空间 + 类型名，泛型参数递归展开。
    /// </summary>
    public static class SchemaIdConvention
    {
        /// <summary>
        /// 生成带程序集的 schemaId，避免不同程序集里的同全名类型互相覆盖。
        /// </summary>
        public static string Of(Type type)
        {
            if (type == null)
            {
                return string.Empty;
            }
            if (type.IsGenericParameter)
            {
                return type.Name;
            }
            if (type.IsArray)
            {
                return Of(type.GetElementType()) + "Array";
            }

            var assembly = type.Assembly.GetName().Name ?? "unknown";
            if (type.IsGenericType)
            {
                var name = type.Name;
                var tick = name.IndexOf('`');
                if (tick > 0)
                {
                    name = name.Substring(0, tick);
                }
                var args = string.Join("And", type.GetGenericArguments().Select(Of));
                return assembly + "." + name + "Of" + args;
            }
            return assembly + "." + (type.FullName?.Replace('+', '.') ?? type.Name);
        }
    }
}
