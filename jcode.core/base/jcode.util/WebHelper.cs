using jcode.autofac;
using Microsoft.AspNetCore.Http;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.02
    /// 描 述：web帮助类
    /// </summary>
    public static class WebHelper
    {
        /// <summary>
        /// 获取访问的客户客户端IP
        /// </summary>
        public static string GetClinetIP()
        {
            var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
            return accessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        /// <summary>
        /// 获取访问的客户客户端IP
        /// </summary>
        public static string GetClinetNginxIP()
        {
            string RemoteIpAddress = string.Empty;
            IHttpContextAccessor _iHttpContextAccessor = IocManager.Instance.GetService<IHttpContextAccessor>();
            if (_iHttpContextAccessor.HttpContext.Request.Headers["X-Real-IP"].IsEmpty())
            {
                RemoteIpAddress = _iHttpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                RemoteIpAddress = _iHttpContextAccessor.HttpContext.Request.Headers["X-Real-IP"].ToString();
            }
            return RemoteIpAddress;
        }
    }
}
