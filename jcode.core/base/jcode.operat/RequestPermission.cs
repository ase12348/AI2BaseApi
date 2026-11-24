using jcode.autofac;
using jcode.util;
using Microsoft.AspNetCore.Http;
using System;

namespace jcode.operat
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.02
    /// 描 述：请求授权
    /// </summary>
    public class RequestPermission
    {
        /// <summary>
        /// 获取请求密钥
        /// </summary>
        public static string GetRequestToken
        {
            get
            {
                if (IocManager.Instance.Container != null && IocManager.Instance.IsRegistered<IHttpContextAccessor>())
                {
                    var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                    if (accessor.HttpContext != null)
                    {
                        string token = string.Empty;
                        if (!accessor.HttpContext.Request.Headers["token"].IsEmpty())
                        {
                            token = accessor.HttpContext.Request.Headers["token"].ToString();
                        }
                        //若是开启密级，则无法使用除header以外的token传参----ADD BY SSY 20230419
                        //if (!ConfigHelper.GetConfig().IsSecretLevel)
                        //{
                            if (string.IsNullOrEmpty(token))
                            {
                                try
                                {
                                    token = accessor.HttpContext.Request.Query["token"];
                                }
                                catch (Exception)
                                {
                                    token = string.Empty;
                                }
                            }

                            if (string.IsNullOrEmpty(token))
                            {
                                try
                                {
                                    if (accessor.HttpContext.Request.HasFormContentType)
                                    {
                                        token = accessor.HttpContext.Request.Form["token"];
                                    }
                                    else
                                    {
                                        token = string.Empty;
                                    }
                                }
                                catch (Exception)
                                {
                                    token = string.Empty;
                                }
                            }
                        //}

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
        }
    }
}
