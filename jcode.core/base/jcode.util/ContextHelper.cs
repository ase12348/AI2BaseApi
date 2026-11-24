using jcode.autofac;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading;

namespace jcode.util
{

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.11
    /// 描 述：上下文上下文帮助类
    /// </summary>
    public static class ContextHelper {
        //private static readonly ConcurrentDictionary<string, object> _list =new ConcurrentDictionary<string, object>();
        private static readonly AsyncLocal<Dictionary<string, object>> asyncLocal = new AsyncLocal<Dictionary<string, object>>();

        /// <summary>
        /// 不在webapi中执行,目前使用于任务调度
        /// </summary>
        public static void SetAsyncLocal() {
            asyncLocal.Value = new Dictionary<string, object>();
        }


        /// <summary>
        /// 获取上下文值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static object GetItem(string key)
        {
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
            {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                if (accessor.HttpContext != null && accessor.HttpContext.Items != null)
                {
                    accessor.HttpContext.Items.TryGetValue(key, out object res);
                    return res;
                }
                else if (asyncLocal.Value != null)
                {
                    asyncLocal.Value.TryGetValue(key, out object result);
                    return result;
                }
            }

            return string.Empty;

        }
        /// <summary>
        /// 设置上下文值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="value">值</param>
        public static void SetItem(string key,string value)
        {
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>()) {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();

                if (accessor.HttpContext != null && accessor.HttpContext.Items != null)
                {
                    if (accessor.HttpContext.Items.ContainsKey(key))
                    {
                        accessor.HttpContext.Items.Remove(key);
                    }
                    accessor.HttpContext.Items.Add(key, value);
                }
                else if (asyncLocal.Value != null) { 
                    asyncLocal.Value.TryAdd(key, value);
                }
            }
            
        }
        /// <summary>
        /// 移除上下文值
        /// </summary>
        /// <param name="key"></param>
        public static void RemoveItem(string key)
        {
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
            {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                if (accessor.HttpContext != null && accessor.HttpContext.Items != null)
                {
                    if (accessor.HttpContext.Items.ContainsKey(key))
                    {
                        accessor.HttpContext.Items.Remove(key);
                    }
                }
                else if (asyncLocal.Value != null)
                {
                    asyncLocal.Value.Remove(key);
                }
            }
        }
    }
}
