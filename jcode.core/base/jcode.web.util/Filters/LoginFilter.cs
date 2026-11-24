using jcode.autofac;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.web.util.Filters
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.17
    /// 描 述：登录验证过滤器
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public class LoginFilter : Attribute, IAsyncAuthorizationFilter
    {
        private readonly IOperator _operator;
        private readonly UserIBLL _userIBLL;
        private readonly ISystemConfigBLL _iSystemConfigBLL;
  
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="ioperator">验证登录信息方法</param>
        /// <param name="userIBLL">获取用户方法</param>
        /// <param name="iSystemConfigBLL">系统配置</param>
        public LoginFilter(IOperator ioperator,UserIBLL userIBLL,ISystemConfigBLL iSystemConfigBLL)
        {
            _operator = ioperator;
            _userIBLL = userIBLL;
            _iSystemConfigBLL = iSystemConfigBLL;
        }

        /// <summary>
        /// 执行验证
        /// </summary>
        /// <param name="context"></param>
        public  async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
        
            // 获取ip地址
            var ip = "";
            if (context.HttpContext.Request.Headers["X-Real-IP"].IsEmpty())
            {
                ip = context.HttpContext.Connection.RemoteIpAddress.ToString();
            }
            else
            {
                ip = context.HttpContext.Request.Headers["X-Real-IP"].ToString();
            }
            if (!string.IsNullOrEmpty(ip))
            {
                ip = ip.Replace("::ffff:", "");
                ip = ip.Replace("::1", "");

                if (string.IsNullOrEmpty(ip))
                {
                    ip = "127.0.0.1";
                }
            }
            ContextHelper.SetItem("ip", ip);
            
            // 获取请求设备类型，目前只是对数据权限有效果，自定义表单数据权限可以做app和pc端分开
            if (!context.HttpContext.Request.Headers["dc"].IsEmpty() && context.HttpContext.Request.Headers["dc"].ToString() == "pc")
            {
                ContextHelper.SetItem("device", "pc");
            }
            else
            {
                ContextHelper.SetItem("device", "app");
            }

            // 允许匿名访问
            var endpoint = context.HttpContext.Features.Get<IEndpointFeature>()?.Endpoint;
            if (endpoint != null && endpoint.Metadata.GetMetadata<AllowAnonymousAttribute>() != null)
            {
                return;
            }

            {
                // Token验证
                // string token = RequestPermission.GetRequestToken;
                var res = _operator.DecodeToken();
                if (string.IsNullOrEmpty(res))
                {
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.nologin, info =LangHelper.GetLang("缺失密钥") });
                    return;
                }
                
                if (res == AuthTokenConstants.ErrorExpired)
                {
                    // 4011 仅表示访问令牌过期，前端凭刷新令牌换新令牌；不要在此清理登录态
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.tokenexpired, info = LangHelper.GetLang("登录信息过期") });
                    return;
                }
                
                if (res == AuthTokenConstants.ErrorSignature)
                {
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.nologin, info = LangHelper.GetLang("非法密钥") });
                    return;
                }

                if (res == AuthTokenConstants.ErrorTypeMismatch)
                {
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.nologin, info = LangHelper.GetLang("登录凭证类型错误") });
                    return;
                }
                
                var payload = res.ToObject<Payload>();
                var userEntity = await _userIBLL.GetEntityByAccount(payload.Account);
                if (userEntity == null)
                {
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.nologin, info = LangHelper.GetLang("用户不存在") });
                    return;
                }

                
                var loginConfig  = await _iSystemConfigBLL.GetLoginConfig();

                /*if (loginConfig.LoginType == 1 && payload.LoginRandomCode != userEntity.F_LoginRandomCode)
                {
                    context.Result = new ObjectResult(new ResponseDto() { code = ResponseCode.nologin, info = LangHelper.GetLang("账号已经在其它地方登录") });
                    return;
                }*/
                
                // 做接口校验处理
                /*var apiUrl = context.HttpContext.Request.Path.Value;
                if (!apiUrl.StartsWith("/data/")
                     &&!apiUrl.StartsWith("/login/pc") 
                     && !apiUrl.StartsWith("/custmerform/data") 
                     && !apiUrl.StartsWith("/system/annexesfile/") 
                     && !apiUrl.StartsWith("/system/img") 
                     && !apiUrl.StartsWith("/wordPrint/template/code/")
                     && !apiUrl.StartsWith("/message/")
                     && !apiUrl.StartsWith("/print/printTemplate/code/")
                   )
                {
                    string u = string.Empty;
                    if (!context.HttpContext.Request.Headers["u"].IsEmpty())
                    {
                        u = context.HttpContext.Request.Headers["u"].ToString();
                    }

                    if (string.IsNullOrEmpty(u))
                    {
                        context.Result = new ObjectResult(new { code = 403, info = LangHelper.GetLang("没有访问权限！") });
                        return;
                    }

                    var uValue = AESHelper.AesDecrypt(u, userEntity.F_UserId);

                    if ("/" + uValue != apiUrl)
                    {
                        context.Result = new ObjectResult(new { code = 403, info = LangHelper.GetLang("没有访问权限！") });
                        return;
                    }
                }*/
                


                ContextHelper.SetItem("userId", userEntity.F_UserId);
                ContextHelper.SetItem("account", userEntity.F_Account);
                ContextHelper.SetItem("userName", userEntity.F_RealName);
                ContextHelper.SetItem("SecurityLevel", userEntity.F_SecurityLevel?.ToString());//安全等级：1超级用户 2三员-系统管理员、3三员-安全管理员、4三员-审计员
                
                // 设置当前登录者岗位
                var postId = string.Empty;
                if (!context.HttpContext.Request.Headers["jcodepostId"].IsEmpty()) {
                    postId = context.HttpContext.Request.Headers["jcodepostId"].ToString();
                }
                if (string.IsNullOrEmpty(postId))
                {
                    try
                    {
                        postId = context.HttpContext.Request.Query["jcodepostId"];
                    }
                    catch (Exception)
                    {
                        postId = string.Empty;
                    }
                }
                ContextHelper.SetItem("postId", postId);
                
                OnlineUserHelper.AddUser(payload.LoginRandomCode,new OnlineUser()
                {
                    Account = payload.Account,
                    Name =  payload.UserName,
                    UserId = payload.UserId,
                    LoginRandomCode = payload.LoginRandomCode,
                    Time = DateTime.Now,
                    LoginTime = payload.Time,
                });
            }

            
        }
    }
}
