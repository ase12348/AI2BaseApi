using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;
using System.IO;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.02
    /// 描 述：对HtmlHelper类进行扩展
    /// </summary>
    public static class ConfigHelper
    {
        /// <summary>
        /// 缓存数据（配置信息）
        /// </summary>
        private static readonly ConcurrentDictionary<string, object> _setting =
           new ConcurrentDictionary<string, object>();

        /// <summary>
        /// 缓存数据
        /// </summary>
        private static readonly ConcurrentDictionary<string, string> _cache =
          new ConcurrentDictionary<string, string>();

        private static IConfiguration configuration;

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static T GetAppSettings<T>(string key) where T : class, new()
        {
            if (configuration == null)
            {
                var env = GetValue<string>("env");
                string fileName = "appsettings.json";
                if (env == "dev")
                {
                    fileName = "appsettings.Development.json";
                }
                else if (env == "pro")
                {
                    fileName = "appsettings.json";
                }
                else if(!string.IsNullOrEmpty(env))
                {
                    fileName = $"appsettings.{env}.json";
                }

                var directory = ConfigHelper.GetValue<string>("baseDir");
                var filePath = $"{directory}/{fileName}";

                if (File.Exists(filePath)){
                    var builder = new ConfigurationBuilder();
                    builder.AddJsonFile(filePath, false, true);
                    configuration = builder.Build();
                }
            }
            // 获取bin目录路径
            //if (!File.Exists(filePath))
            //{
            //    var length = directory.IndexOf("/bin", StringComparison.Ordinal);
            //    filePath = $"{directory.Substring(0, length)}/{fileName}";
            //}
            //IConfiguration configuration;

            if (configuration == null) {
                return new T();
            }
            
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(configuration.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            
            return appconfig;
        }

        /// <summary>
        /// 获取配置信息
        /// </summary>
        /// <returns></returns>
        public static ServerOp GetConfig()
        {
            return GetAppSettings<ServerOp>("ServerOp");
        }

        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetValue(string key, object value)
        {
            _setting.GetOrAdd(key, value);
        }
        /// <summary>
        /// 获取数据值
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static T GetValue<T>(string key) where T : class
        {
            _setting.TryGetValue(key, out object result);
            return result as T;
        }


        /// <summary>
        /// 设置信息
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetCache(string key, string value)
        {
            _cache.GetOrAdd(key, value);
        }
        /// <summary>
        /// 获取数据值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string GetCache(string key)
        {
            _cache.TryGetValue(key, out string result);
            return result;
        }

        /// <summary>
        /// 移除数据值
        /// </summary>
        /// <param name="key">键值</param>
        /// <returns></returns>
        public static string RemoveCache(string key)
        {
            _cache.TryRemove(key, out string result);
            return result;
        }

        /// <summary>
        /// 清空缓存数据
        /// </summary>
        /// <returns></returns>
        public static void ClearCache()
        {
            _cache.Clear();
        }
    }
}
