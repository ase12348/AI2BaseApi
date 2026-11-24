using jcode.util;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace jcode.web.util.Filters
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.17
    /// 描 述：返回数据过滤器
    /// </summary>
    public class ResultFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 返回结果处理
        /// </summary>
        /// <param name="context"></param>
        public override void OnResultExecuting(ResultExecutingContext context)
        {

            if (!context.ModelState.IsValid)
            {
                ResponseDto result = new ResponseDto();

                foreach (var item in context.ModelState.Values)
                {
                    foreach (var error in item.Errors)
                    {
                        result.info += error.ErrorMessage + ",";
                    }
                }
                result.code = ResponseCode.fail;
                result.info = result.info.TrimEnd(',');
                context.Result = new JsonResult(result);
            }
            else
            {
                base.OnResultExecuting(context);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public override void OnActionExecuting(ActionExecutingContext context)
        {
        }
    }
}
