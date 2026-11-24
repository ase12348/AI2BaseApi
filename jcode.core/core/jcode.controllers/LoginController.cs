using jcode.cache;
using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.24
    /// 描 述：登录模块
    /// </summary>
    public class LoginController : BaseApiController
    {
        private readonly ILoginBLL _iLoginBLL;
        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly AuthorizeIBLL _authorizeIBLL;
        private readonly ICache _iCache;
        private readonly ISystemConfigBLL _iSystemConfigBLL;


        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="iLoginBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="authorizeIBLL"></param>
        /// <param name="iCache"></param>
        /// <param name="iSystemConfigBLL"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public LoginController(
            ILoginBLL iLoginBLL,
            UserRelationIBLL userRelationIBLL,
            AuthorizeIBLL authorizeIBLL, 
            ICache iCache,
            ISystemConfigBLL iSystemConfigBLL
            )
        {
            _iLoginBLL = iLoginBLL ?? throw new ArgumentNullException(nameof(iLoginBLL));
            _userRelationIBLL = userRelationIBLL ?? throw new ArgumentNullException(nameof(userRelationIBLL));
            _authorizeIBLL = authorizeIBLL ?? throw new ArgumentNullException(nameof(authorizeIBLL));
            
            _iCache = iCache ?? throw new ArgumentNullException(nameof(iCache));
            _iSystemConfigBLL = iSystemConfigBLL ?? throw new ArgumentNullException(nameof(iSystemConfigBLL));

           
        }

        /// <summary>
        /// 登录接口
        /// </summary>
        /// <param name="loginInputDto">输入参数</param>
        /// <returns></returns>
        [HttpPost("login")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto<LoginOutputDto>), 200)]
        public async Task<IActionResult> Login(LoginInputDto loginInputDto)
        {
            //throw new Exception("测试熔断");
            return OK(await _iLoginBLL.Login(loginInputDto));
        }

        /// <summary>
        /// 获取用户信息PC端
        /// </summary>
        /// <returns></returns>
        [HttpGet("login/pc")]
        [ProducesResponseType(typeof(ResponseDto<UserEntity>), 200)]
        public async Task<IActionResult> GetUserInfo()
        {
            var data = await this.CurrentUser();
            if (data == null)
            {
                return Nologin("没有此账号");
            }



            if (data.F_SecurityLevel != 1 && data.F_SecurityLevel != 5)
            {
                // 密码策略
                var pwdConfig  = await _iSystemConfigBLL.GetPwdConfig();
                if (pwdConfig != null && pwdConfig.PwdDay != null)
                {
                    DateTime LastChangePDate = Convert.ToDateTime(data.F_LastChangePDate);//最后一次修改密码时间
                    DateTime nowtime = DateTime.Now;//当前时间
                    TimeSpan ts = nowtime - LastChangePDate;//计算时间差
                    double days = ts.TotalDays;//将时间差转换为天数
                    if (days >= pwdConfig.PwdDay)
                    {
                        data.F_IsNeedChangePwd = 1;
                    }
                }
                
                
                data.PostIds = string.Join(',', await _userRelationIBLL.GetObjectIds(data.F_UserId, 2)); // 岗位
                data.RoleIds = string.Join(',', await _userRelationIBLL.GetRoleIds());
               
                
                string objectIds = data.F_UserId + (string.IsNullOrEmpty(data.RoleIds) ? "" : ("," + data.RoleIds));
                var moduleAuthIds = (List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(objectIds.Split(','), 1, "0");
                var notModuleAuthIds =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(data.F_UserId.Split(','), 10, "0");

                data.ModuleAuthIds = moduleAuthIds.FindAll(t => notModuleAuthIds.FindIndex(t2=>t2 == t) == -1);
            }
            data.F_Secretkey = "";
            data.F_Password = "";
            return Success(data);
        }
        
        /// <summary>
        /// 获取用户信息移动端
        /// </summary>
        /// <returns></returns>
        [HttpGet("login/app")]
        [ProducesResponseType(typeof(ResponseDto<UserEntity>), 200)]
        public async Task<IActionResult> GetUserAppInfo()
        {
            var data = await this.CurrentUser();
            if (data == null)
            {
                return Nologin("没有此账号");
            }
            
            if (data.F_SecurityLevel != 1 && data.F_SecurityLevel != 5)
            {
                // 密码策略
                var pwdConfig  = await _iSystemConfigBLL.GetPwdConfig();
                if (pwdConfig != null && pwdConfig.PwdDay != null)
                {
                    DateTime LastChangePDate = Convert.ToDateTime(data.F_LastChangePDate);//最后一次修改密码时间
                    DateTime nowtime = DateTime.Now;//当前时间
                    TimeSpan ts = nowtime - LastChangePDate;//计算时间差
                    double days = ts.TotalDays;//将时间差转换为天数
                    if (days >= pwdConfig.PwdDay)
                    {
                        return NeedChangePwd("请更新密码!");
                    }
                }
                
                data.PostIds = string.Join(',', await _userRelationIBLL.GetObjectIds(data.F_UserId, 2)); // 岗位
                data.RoleIds = string.Join(',', await _userRelationIBLL.GetRoleIds());
               
                
                string objectIds = data.F_UserId + (string.IsNullOrEmpty(data.RoleIds) ? "" : ("," + data.RoleIds));
    
                var moduleAuthIds = (List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(objectIds.Split(','), 5, "0");
                var notModuleAuthIds =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(data.F_UserId.Split(','), 50, "0");

                
                data.ModuleAuthIds = moduleAuthIds.FindAll(t => notModuleAuthIds.FindIndex(t2=>t2 == t) == -1);
            }
            

            data.F_Secretkey = "";
            data.F_Password = "";
            return Success(data);
        }
        
        /// <summary>
        /// 用刷新令牌换取新的访问令牌与刷新令牌
        /// </summary>
        /// <returns></returns>
        [HttpPost("login/refresh")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto<LoginOutputDto>), 200)]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Headers[jcode.operat.AuthTokenConstants.RefreshTokenHeader].ToString();
            return OK(await _iLoginBLL.Refresh(refreshToken));
        }

        /// <summary>
        /// 注销当前会话
        /// </summary>
        /// <returns></returns>
        [HttpPost("login/logout")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Headers[jcode.operat.AuthTokenConstants.RefreshTokenHeader].ToString();
            await _iLoginBLL.Logout(refreshToken);
            return SuccessInfo("退出成功");
        }

        /// <summary>
        /// 清空缓存
        /// </summary>
        /// <returns></returns>
        [HttpPost("login/cache")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public IActionResult RemvoeCache()
        {
            _iCache.RemoveAll();
            return SuccessInfo("清空成功");
        }

        #region 微信小程序登录

        #region 小程序绑定
        /// <summary>
        /// 小程序绑定
        /// </summary>
        /// <param name="code">获取openId编码</param>
        /// <param name="type">'weixin'/'alipay'/'dingtalk' 时表示使用小程序一键登录;暂时只支持微信</param>
        /// <returns></returns>
        [HttpPost("login/binding")]
        [ProducesResponseType(typeof(ResponseDto<LoginOutputDto>), 200)]
        public async Task<IActionResult> OpenIdBind(string code,string type)
        {
            await _iLoginBLL.OpenIdBind(code,type);
            return SuccessInfo("绑定成功");
        }
        #endregion

        #region 小程序解绑
        /// <summary>
        /// 小程序解绑
        /// </summary>
        /// <returns></returns>
        [HttpPost("login/unbinding")]
        [ProducesResponseType(typeof(ResponseDto<OpenidRes>), 200)]
        public async Task<IActionResult> OpenIdUnbind()
        {
            await _iLoginBLL.OpenIdUnbind();
            return SuccessInfo("解绑成功");
        }
        #endregion

        #region 获取小程序Opendid
        /// <summary>
        /// 获取小程序Opendid
        /// </summary>
        /// <param name="code">获取openId编码</param>
        /// <param name="type">'weixin'/'alipay'/'dingtalk' 时表示使用小程序一键登录;暂时只支持微信</param>
        /// <returns></returns>
        [HttpGet("login/openid")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto<OpenidRes>), 200)]
        public async Task<IActionResult> GetOpenid(string code,string type)
        {
            var res = await _iLoginBLL.GetOpenid(code, type);
            return Success(res);
        }
        #endregion
        
        #region 小程序登录
        /// <summary>
        /// 小程序登录
        /// </summary>
        /// <param name="code">获取openId编码</param>
        /// <param name="type">'weixin'/'alipay'/'dingtalk'/'feishu' 时表示使用小程序一键登录;暂时只支持微信</param>
        /// <returns></returns>
        [HttpPost("login/openid")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto<LoginOutputDto>), 200)]
        public async Task<IActionResult> OpenIdLogin(string code,string type)
        {
            return OK(await _iLoginBLL.OpenIdLogin(code,type));
        }
        #endregion

        #endregion
        
        

    }
}
