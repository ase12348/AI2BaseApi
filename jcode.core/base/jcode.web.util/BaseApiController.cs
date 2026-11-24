using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using jcode.web.util.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using jcode.ilanguage;
using jcode.operat;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace jcode.web.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.11
    /// 描 述：基础控制器
    /// </summary>
    [TypeFilter(typeof(ExceptionFilter))]
    [TypeFilter(typeof(LoginFilter))]
    [TypeFilter(typeof(ResultFilter))]
    [TypeFilter(typeof(RequestFilter))]
    [ApiController]
    [ApiExplorerSettings(GroupName = "v2")]
    public class BaseApiController : ControllerBase
    {
        #region 获取基础信息
        private UserEntity userInfo;
        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        protected async Task<UserEntity> CurrentUser(string userId = null)
        {
            var userBLL = IocManager.Instance.GetService<UserIBLL>();
            if (string.IsNullOrEmpty(userId))
            {
                if (userInfo == null)
                {
                    userInfo = await userBLL.GetEntity();
                }
                return userInfo;
            }
            else
            {
                return await userBLL.GetEntity(userId);
            }

        }
        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="code">编码规则编码</param>
        /// <param name="formData">表单值</param>
        /// <returns></returns>
        protected Task<string> GetRuleCode(string code,string formData = "")
        {
            var codeRuleIBLL = IocManager.Instance.GetService<CodeRuleIBLL>();
            return codeRuleIBLL.GetBillCode(code,formData);
        }

        /// <summary>
        /// 获取登录者用户名称
        /// </summary>
        /// <returns></returns>
        protected string GetUserName()
        {
            return ContextHelper.GetItem("userName") as string;
        }
        /// <summary>
        /// 获取登录者用户Id
        /// </summary>
        /// <returns></returns>
        protected string GetUserId()
        {
            return ContextHelper.GetItem("userId") as string;
        }
        /// <summary>
        /// 获取登录者用户账号
        /// </summary>
        /// <returns></returns>
        protected string GetUserAccount()
        {
            return ContextHelper.GetItem("account") as string;
        }


        /// <summary>
        /// 获取租户号
        /// </summary>
        /// <returns></returns>
        protected string GetTenantNo()
        {
            return ContextHelper.GetItem("TenantNo") as string;
        }

        /// <summary>
        /// 获取租户名称
        /// </summary>
        /// <returns></returns>
        protected string GetTenantName()
        {
            return ContextHelper.GetItem("TenantName") as string;
        }


        /// <summary>
        /// 获取租户公司名
        /// </summary>
        /// <returns></returns>
        protected string GetTenantCompany()
        {
            return ContextHelper.GetItem("TenantCompany") as string;
        }
        #endregion

        #region 多语言
        
        /// <summary>
        /// 获取翻译
        /// </summary>
        /// <param name="label">当前语言</param>
        /// <returns></returns>
        protected string GetLang(string label)
        {
            return LangHelper.GetLang(label);
        }

        #endregion
        
        #region 请求响应
        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual IActionResult SuccessTable(object data)
        {
            return Content(new ResponseDto { code = ResponseCode.success, info = GetLang("获取成功"), data = data }.ToJson());
        }
        
        
        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual IActionResult Success(object data)
        {
            if (ConfigHelper.GetConfig().ApiDataAes)
            {
                var settings = new JsonSerializerSettings
                {
                    ContractResolver = new DefaultContractResolver
                    {
                        NamingStrategy = new CamelCaseNamingStrategy() // 使用 CamelCase 命名策略
                    },
                    DateFormatString = "yyyy-MM-dd HH:mm:ss"
                };
                var dataStr = JsonConvert.SerializeObject(data, settings);
                
                return new JsonResult(new { code = ResponseCode.success, info = GetLang("获取成功"),s = 1, data = AESHelper.AesEncrypt(dataStr, "jcodeapi") });
            }

            return new JsonResult(new ResponseDto { code = ResponseCode.success, info = GetLang("获取成功"), data = data });
        }
        /// <summary>
        /// 返回成功消息
        /// </summary>
        /// <param name="info">信息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        protected virtual IActionResult Success(string info, object data)
        {
            if (ConfigHelper.GetConfig().ApiDataAes)
            {
                return new JsonResult(new { code = ResponseCode.success, info = GetLang("获取成功"),s = 1, data = AESHelper.AesEncrypt(data.ToJson(), "jcodeapi") });
            }
            return new JsonResult(new ResponseDto { code = ResponseCode.success, info = GetLang(info), data = data });
        }
        /// <summary>
        /// 返回成功数据
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns></returns>
        protected virtual IActionResult SuccessInfo(string info)
        {
            return new JsonResult(new ResponseDto { code = ResponseCode.success, info = GetLang(info), data = new { } });
        }

        /// <summary>
        /// 返回失败消息
        /// </summary>
        /// <param name="info">消息</param>
        /// <returns></returns>
        protected virtual IActionResult Fail(string info)
        {
            return new JsonResult(new ResponseDto { code = ResponseCode.fail, info = GetLang(info) });
        }

        /// <summary>
        /// 返回信息
        /// </summary>
        /// <param name="responseDto">返回数据</param>
        /// <returns></returns>
        protected virtual IActionResult OK(object responseDto)
        {
            return new JsonResult(responseDto);
        }


        /// <summary>
        /// 没有权限
        /// </summary>
        /// <param name="info">消息</param>
        /// <returns></returns>
        protected virtual IActionResult Nologin(string info)
        {
            return new JsonResult(new ResponseDto { code = ResponseCode.nologin, info = GetLang(info) });
        }
        
        /// <summary>
        /// 没有权限
        /// </summary>
        /// <param name="info">消息</param>
        /// <returns></returns>
        protected virtual IActionResult NeedChangePwd(string info)
        {
            return new JsonResult(new { code = 402, info = GetLang(info) });
        }

        #endregion

        /// <summary>
        /// 获取cookies
        /// </summary>
        /// <param name="key">键</param>
        /// <returns>返回对应的值</returns>
        protected string GetCookies(string key)
        {
            HttpContext.Request.Cookies.TryGetValue(key, out string value);
            if (string.IsNullOrEmpty(value))
                value = string.Empty;
            return value;
        }
    }
}
