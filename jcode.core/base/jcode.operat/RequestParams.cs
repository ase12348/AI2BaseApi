using System;
using jcode.autofac;
using jcode.util;
using Microsoft.AspNetCore.Http;

namespace jcode.operat
{
    public class RequestParams
    {
        /// <summary>
        /// 获取头部请求参数值
        /// </summary>
        public static string GetHeadersParamValue(string name)
        {
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
            {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                if (accessor.HttpContext != null)
                {
                    string token = string.Empty;
                    if (!accessor.HttpContext.Request.Headers[name].IsEmpty())
                    {
                        token = accessor.HttpContext.Request.Headers[name].ToString();
                    }
                    return token;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        
        
        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public static string GetQuerysParamValue(string name)
        {
            if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
            {
                var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                if (accessor.HttpContext != null)
                {
                    string token = string.Empty;
                    if (!accessor.HttpContext.Request.Query[name].IsEmpty())
                    {
                        token = accessor.HttpContext.Request.Query[name].ToString();
                    }
                    return token;
                }
                else
                {
                    return string.Empty;
                }
            }
            else
            {
                return string.Empty;
            }
        }
        
        
        /// <summary>
        /// 获取请求参数值
        /// </summary>
        public static string GetParamValue(string name)
        {
            var value = GetHeadersParamValue(name);
            if (string.IsNullOrEmpty(value))
            {
                return   GetQuerysParamValue(name);
            }

            return value;
        }
    }
}