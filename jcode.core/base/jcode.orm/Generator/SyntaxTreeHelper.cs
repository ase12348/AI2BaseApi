using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Emit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;


namespace jcode.orm
{
    /// <summary>
    /// 语法树帮助类
    /// </summary>
    public class SyntaxTreeHelper
    {

        private static MetadataReference[] references;
        
        public static MetadataReference[] GetMetadataReferences()
        {
            if (references == null)
            {
                var refPaths = new[] {
                    typeof(System.Object).GetTypeInfo().Assembly.Location,
                    Path.Combine(Path.GetDirectoryName(typeof(System.Runtime.GCSettings).GetTypeInfo().Assembly.Location), "System.Runtime.dll")
                };
                references = refPaths.Select(r => MetadataReference.CreateFromFile(r)).ToArray();
            }

           
            return references;
        }

        /// <summary>
        /// 获取模型类型通过类
        /// </summary>
        /// <param name="classString">类字串</param>
        /// <param name="typeName">类型名称</param>
        /// <returns></returns>
        public static Type GetModelTypeByClass(string classString, string typeName)
        {
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(classString);
            string assemblyName = Path.GetRandomFileName();
            MetadataReference[] references = GetMetadataReferences();
            CSharpCompilation compilation = CSharpCompilation.Create(
                assemblyName,
                syntaxTrees: new[] { syntaxTree },
                references: references,
                options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
            using (var ms = new MemoryStream())
            {
                EmitResult result = compilation.Emit(ms);
              

                if (!result.Success)
                {
                    IEnumerable<Diagnostic> failures = result.Diagnostics.Where(diagnostic =>
                        diagnostic.IsWarningAsError ||
                        diagnostic.Severity == DiagnosticSeverity.Error);

                    foreach (Diagnostic diagnostic in failures)
                    {
                        Console.Error.WriteLine("\t{0}: {1}", diagnostic.Id, diagnostic.GetMessage());
                    }
                    throw new Exception("解析实体类出错，请检查命名" + classString);
                }
                else
                {
                    try
                    {
                        ms.Seek(0, SeekOrigin.Begin);
                        Assembly assembly = AssemblyLoadContext.Default.LoadFromStream(ms);
                        var type = assembly.GetType("Jcode.Database.Entity." + typeName);
                        return type;
                    }
                    finally
                    {
                        ms.Close();
                    }
                }
            }
        }

        public static string GetClassString(EntitiesGen genentity) {
            string str = @"using System;
            namespace  Jcode.Database.Entity
            {
            ";
            str += $"public class {genentity.ClassName}";
            str += "{";
            foreach (var item in genentity.PropertyGens) {
                str += $"public {item.Type} {item.PropertyName}" + "{ get; set; }";
            }
            str += "}";
            str += "}";


            return str;
        }
    }
}
