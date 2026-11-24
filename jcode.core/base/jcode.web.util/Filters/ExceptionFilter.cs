using System;
using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace jcode.web.util.Filters
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.17
    /// 描 述：异常过滤器
    /// </summary>
    public class ExceptionFilter : IExceptionFilter
    {

        /// <summary>
        /// 发生异常时进入
        /// </summary>
        /// <param name="context"></param>
        public void OnException(ExceptionContext context)
        {

            string message = context.Exception.Message;
            try
            {
                if (context.Exception.Message.IndexOf("#NotLog#") == -1 && context.Exception.Message.IndexOf("#字段重复#") == -1)
                {
                    var logIBLL = IocManager.Instance.GetService<LogIBLL>();

                    LogEntity logEntity = new LogEntity();
                    logEntity.F_CategoryId = 4;
                    logEntity.F_ExecuteResult = -1;
                    logEntity.F_ExecuteResultJson = logIBLL.ExceptionFormat(context.Exception);
                    logIBLL.Write(logEntity,null);
                }
                else
                {
                    message = context.Exception.Message.Replace("#NotLog#", "");
                }
            }
            catch
            {
            }
            
            if (context.ExceptionHandled == false)
            {
                context.Result = new ContentResult
                {
                    Content = new ResponseDto { code = ResponseCode.exception, info = LangHelper.GetLang(message), data = "" }.ToJson(),//这里是把异常抛出。也可以不抛出。
                    StatusCode = StatusCodes.Status200OK,
                    ContentType = "text/html;charset=utf-8"
                };
            }
            context.ExceptionHandled = true;
        }
        
        
        
    }
}
