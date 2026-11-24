using Autofac;
using Microsoft.Extensions.DependencyModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace jcode.autofac
{
    /// <summary>
    /// 版 本 autofac 扩展方法
    /// Copyright (c) 2019-present 橙意工作室
    /// 创建人：cgg
    /// 日 期：2019.11.18
    /// 描 述：扩展类
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 注册业务方法
        /// </summary>
        /// <param name="builder">容器</param>
        /// <returns></returns>
        public static ContainerBuilder RegisterIBLL(this ContainerBuilder builder)
        {
            try
            {
                foreach (Assembly assembly in Extensions.GetAssemblies())
                {
                    RegisterAssembly(builder, assembly);
                }
            }
            catch (Exception ex)
            {
                if (ex is ReflectionTypeLoadException)
                {
                    throw (ex as ReflectionTypeLoadException).LoaderExceptions[0];
                }
                throw ex;
            }
            return builder;
        }
        /// <summary>
        /// 注册子容器方法
        /// </summary>
        /// <param name="container">容器</param>
        /// <param name="assemblys">程序集</param>
        /// <returns></returns>
        public static ILifetimeScope RegisterIBLL(this ILifetimeScope container, List<Assembly> assemblys)
        {
            return container.BeginLifetimeScope(builder =>
            {
                foreach (Assembly assembly in assemblys)
                {
                    RegisterAssembly(builder, assembly);
                }
            });
        }
        /// <summary>
        /// 按 IBLL / BLL 约定注册单个程序集内的业务类型
        /// </summary>
        /// <param name="builder">容器</param>
        /// <param name="assembly">程序集</param>
        private static void RegisterAssembly(ContainerBuilder builder, Assembly assembly)
        {
            (from t in builder.RegisterAssemblyTypes(new Assembly[]{assembly})
             where typeof(IBLL).GetTypeInfo().IsAssignableFrom(t) && t.IsInterface && t.GetTypeInfo().GetCustomAttribute<BLLNameAttribute>() == null
             select t).AsImplementedInterfaces<object>();

            (from t in builder.RegisterAssemblyTypes(new Assembly[]{assembly})
             where typeof(BLL).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<BLLNameAttribute>() == null
             select t).AsImplementedInterfaces<object>();

            var types = assembly.GetTypes().Where(t => typeof(IBLL).GetTypeInfo().IsAssignableFrom(t) && t.GetTypeInfo().GetCustomAttribute<BLLNameAttribute>() != null);
            foreach (var type in types)
            {
                var module = type.GetTypeInfo().GetCustomAttribute<BLLNameAttribute>();
                //对ModuleName不为空的对象，找到第一个继承IServiceKey的接口并注入接口及实现
                var interfaceObj = type.GetInterfaces()
                    .FirstOrDefault(t => typeof(IBLL).GetTypeInfo().IsAssignableFrom(t));
                if (interfaceObj != null)
                {
                    builder.RegisterType(type).AsImplementedInterfaces().Named(module.BLLName, interfaceObj);
                    builder.RegisterType(type).Named(module.BLLName, type);
                }
            }
        }
        /// <summary>
        /// 获取程序集
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Assembly> GetAssemblies()
        {
            if (_referenceAssembly == null)
            {
                _referenceAssembly = new List<Assembly>();
                foreach (string assemblyString in GetFilterAssemblies((from p in DependencyContext.Default.GetDefaultAssemblyNames()
                                                                        select p.Name).ToArray()))
                {
                    _referenceAssembly.Add(Assembly.Load(assemblyString));
                }
            }
            return _referenceAssembly;
        }
        /// <summary>
        /// 过滤系统程序集
        /// </summary>
        /// <param name="assemblyNames">当前程序所依赖的程序集</param>
        /// <returns></returns>
        private static string[] GetFilterAssemblies(string[] assemblyNames)
        {
            string pattern = "^Microsoft.\\w*|^System.\\w*|^DotNetty.\\w*|^runtime.\\w*|^ZooKeeperNetEx\\w*|^StackExchange.Redis\\w*|^Consul\\w*|^Newtonsoft.Json.\\w*|^Newtonsoft.Json\\w*|^Autofac.\\w*|^Autofac\\w*|^SOS.NETCore\\w*|^System\\w*|^WindowsBase\\w*|^mscorlib\\w*|^netstandard\\w*";
            Regex notRelatedRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Singleline);
            return (from name in assemblyNames
                    where !notRelatedRegex.IsMatch(name)
                    select name).ToArray<string>();
        }
        /// <summary>
        /// 缓存程序集
        /// </summary>
        private static List<Assembly> _referenceAssembly;
    }
}
