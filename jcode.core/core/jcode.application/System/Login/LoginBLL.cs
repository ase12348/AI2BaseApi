using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using TencentCloud.Ess.V20201111.Models;
using HttpMethods = jcode.util.HttpMethods;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.25
    /// 描 述：登录模块
    /// </summary>
    public class LoginBLL : BLLBase, ILoginBLL, BLL
    {
        private readonly UserIBLL _userIBLL;
        private readonly LogIBLL _logIBLL;
        private readonly ISystemConfigBLL _iSystemConfigBLL;
        private readonly IAuthTokenBLL _authTokenBLL;
        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="userIBLL"></param>
        /// <param name="logIBLL"></param>
        /// <param name="iSystemConfigBLL"></param>
        /// <param name="authTokenBLL"></param>
        public LoginBLL(
            UserIBLL userIBLL,
            LogIBLL logIBLL,
            ISystemConfigBLL iSystemConfigBLL,
            IAuthTokenBLL authTokenBLL

        )
        {
            _userIBLL = userIBLL ?? throw new ArgumentNullException(nameof(userIBLL));
            _logIBLL = logIBLL ?? throw new ArgumentNullException(nameof(logIBLL));
            _iSystemConfigBLL = iSystemConfigBLL ?? throw new ArgumentNullException(nameof(iSystemConfigBLL));
            _authTokenBLL = authTokenBLL ?? throw new ArgumentNullException(nameof(authTokenBLL));
        }

        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="loginInputDto">输入参数</param>
        /// <returns></returns>
        public async Task<ResponseDto<LoginOutputDto>> Login(LoginInputDto loginInputDto)
        {
            Stopwatch stopwatch = CommonHelper.TimerStart();
            
            #region 写入日志
            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 1;
            logEntity.F_OperateAccount = loginInputDto.Account;
            #endregion
            
            // 加载登录配置
            var loginConfig  = await _iSystemConfigBLL.GetLoginConfig();

            // 登录时长
            var tokenTimeout = 0;
            if (loginConfig != null)
            {
                tokenTimeout = loginConfig.TokenTimeout;
                
                
            
                //限制IP登录
                if (loginConfig.IsIpAuth == 1)
                {
                    if (!IsIpAuth(loginConfig.IpAuthType, loginConfig.IpAuth))
                    {
                        logEntity.F_ExecuteResult = 0;
                        logEntity.F_ExecuteResultJson = "限制IP登录!";
                        _logIBLL.Write(logEntity,stopwatch);
                        return Fail<LoginOutputDto>("当前IP无法登录，请联系管理员！");
                    }
                }
            }
            
            



            
            

            #region 内部账户验证
            UserEntity userEntity = await _userIBLL.GetEntityByAccount(loginInputDto.Account);


            
            if (userEntity == null)
            {
                logEntity.F_ExecuteResult = 0;
                logEntity.F_ExecuteResultJson = "没有此账号!";
                _logIBLL.Write(logEntity,stopwatch);
                return Fail<LoginOutputDto>("没有此账号!");
            }
            logEntity.F_OperateUserId = userEntity.F_UserId;
            logEntity.F_OperateName = userEntity.F_RealName;

            // 密码错误次数
            if (userEntity.F_EnabledMark == 2)
            {
                logEntity.F_ExecuteResultJson = "账户被系统锁定,请联系管理员!";
                _logIBLL.Write(logEntity,stopwatch);
                return Fail<LoginOutputDto>(logEntity.F_ExecuteResultJson);
            }

            if (userEntity.F_SecurityLevel != 1 && loginConfig != null)
            {
                // 限制时间登录
                if (loginConfig.IsLoginTime == 1)
                {
                    if (!IsTimeAuth(loginConfig.LoginTimes))
                    {
                        logEntity.F_ExecuteResult = 0;
                        logEntity.F_ExecuteResultJson = "限制时间登录!";
                        _logIBLL.Write(logEntity,stopwatch);
                        return Fail<LoginOutputDto>("当前时间不允许登录，请联系管理员！");
                    }
                }
            
                if ( loginConfig.IsPwdVerify == 1 && loginConfig.ErrorPwdType != 1 && userEntity.F_LockTime != null &&  userEntity.F_LockTime > DateTime.Now)
                {
                    logEntity.F_ExecuteResult = 0;
                    logEntity.F_ExecuteResultJson = "请" + userEntity.F_LockTime?.ToString("yyyy-MM-dd HH:mm:ss") +"后来登录！";
                    _logIBLL.Write(logEntity,stopwatch);
                    return Fail<LoginOutputDto>(logEntity.F_ExecuteResultJson);
                }
            }

            



            // 校验密码
            bool isOk = _userIBLL.IsPasswordOk(userEntity.F_Password, loginInputDto.Password, userEntity.F_Secretkey);
            if (!isOk)//登录失败
            {
                logEntity.F_ExecuteResult = 0;
                logEntity.F_ExecuteResultJson = "账号密码不匹配";
                _logIBLL.Write(logEntity,stopwatch);
                
                userEntity.F_LockCount = (userEntity.F_LockCount ?? 0) +1;
                
                // 密码错误次数,超级用户不做锁定操作
                if (loginConfig != null && loginConfig.IsPwdVerify == 1 && userEntity.F_LockCount >= loginConfig.ErrorPwdCount && userEntity.F_SecurityLevel != 1)
                {
                    if (loginConfig.ErrorPwdType == 1)
                    {
                        // 锁定账号
                        userEntity.F_EnabledMark = 2; 
                        logEntity.F_ExecuteResultJson = "账号密码不匹配,账号被锁！";
                    }
                    else
                    {
                        // 延时登录
                        userEntity.F_LockTime = DateTime.Now.AddMinutes(30);
                        logEntity.F_ExecuteResultJson = "账号密码不匹配,账号延时30分钟登录！";
                    }
                }
                
                await _userIBLL.UpdateEntity(userEntity);//更新用户错误登录的次数
                return Fail<LoginOutputDto>("账号密码不匹配");
            }
            
            
            // 禁用账号,黑名单账号,离职账号不能登录
            if (userEntity.F_EnabledMark != 1 || userEntity.F_Type == 3 || userEntity.F_IsBlack == 1)
            {
                logEntity.F_ExecuteResult = 0;
                if (userEntity.F_EnabledMark != 1)
                {
                    logEntity.F_ExecuteResultJson = "账户被系统锁定,请联系管理员!";
                }
                if (userEntity.F_Type == 3)
                {
                    logEntity.F_ExecuteResultJson = "离职账号无法登录!";
                }
                if (userEntity.F_IsBlack == 1)
                {
                    logEntity.F_ExecuteResultJson = "黑名单账号无法登录!";
                }

                _logIBLL.Write(logEntity,stopwatch);
                return Fail<LoginOutputDto>("账户被系统锁定,请联系管理员!");
            }
            

            logEntity.F_OperateUserId = userEntity.F_UserId;
            logEntity.F_ExecuteResult = 1;
            logEntity.F_ExecuteResultJson = "登录成功";
            _logIBLL.Write(logEntity,stopwatch);
            
            var loginrandomcode = Guid.NewGuid().ToString();//生成登录成功随机码
            
            userEntity.F_LoginRandomCode = loginrandomcode;
            userEntity.F_LockCount = 0;
            userEntity.F_LockTime = null;
            userEntity.F_LastLoginTime = DateTime.Now;//最后登录时间
            await _userIBLL.UpdateEntity(userEntity);//更新用户信息
            
            var authToken = await _authTokenBLL.Issue(userEntity, tokenTimeout);
            userEntity.F_Password = string.Empty;
            userEntity.F_Secretkey = string.Empty;
            return Success("登录成功", new LoginOutputDto
            {
                token = authToken.AccessToken,
                refreshToken = authToken.RefreshToken,
                expiresIn = authToken.ExpiresIn,
                refreshExpiresIn = authToken.RefreshExpiresIn,
                user = userEntity
            });
            #endregion
        }
        
        /// <summary>
        /// 用刷新令牌换取新的令牌对
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns></returns>
        public async Task<ResponseDto<LoginOutputDto>> Refresh(string refreshToken)
        {
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Fail<LoginOutputDto>("缺少刷新令牌");
            }

            var authToken = await _authTokenBLL.Refresh(refreshToken);
            if (authToken == null)
            {
                return Fail<LoginOutputDto>("登录信息已失效，请重新登录");
            }

            return Success("刷新成功", new LoginOutputDto
            {
                token = authToken.AccessToken,
                refreshToken = authToken.RefreshToken,
                expiresIn = authToken.ExpiresIn,
                refreshExpiresIn = authToken.RefreshExpiresIn
            });
        }

        /// <summary>
        /// 注销当前会话
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns></returns>
        public async Task Logout(string refreshToken)
        {
            await _authTokenBLL.Logout(refreshToken);
        }
        
        /// <summary>
        /// 限制时间登录
        /// </summary>
        /// <param name="loginTimeList">配置的可登录时间</param>
        /// <returns></returns>
        private bool IsTimeAuth(List<ConfigLoginTimeModel> loginTimeList)
        {
            DateTime now = DateTime.Now;
            if (loginTimeList != null)
            {
                if (loginTimeList.Count>0)
                {
                    foreach (var item in loginTimeList)
                    {
                        //如果登录时间小于开始时间，或者大于结束时间，则不允许登录
                        if (now< item.startTime || now>item.endTime)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        /// <summary>
        /// 限定ip权限
        /// </summary>
        /// <param name="IpAuthType">限定类型(1白名单，2黑名单)</param>
        /// <param name="IpAuthStr">限定IP</param>
        /// <returns></returns>
        private bool IsIpAuth(int IpAuthType, string IpAuthStr)
        {
            if (!string.IsNullOrEmpty(IpAuthStr))
            {
                string Ip = WebHelper.GetClinetIP().ToString();
                List<string> IpAuthList = new List<string>(IpAuthStr.Split(','));//IP清单
                if (IpAuthList.Count > 0)
                {
                    var res = IpAuthList.Contains(Ip);
                    if (IpAuthType == 1) //白名单-只有在ip清单中的才可以访问
                    {
                        if (!res)
                        {
                            return false;
                        }
                    }
                    else //黑名单-在IP清单中的不可以访问
                    {
                        if (res)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        
        #region 微信小程序登录

        #region 小程序绑定
        /// <summary>
        /// 小程序绑定
        /// </summary>
        /// <param name="code">小程序打开时生成的</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        public async Task OpenIdBind(string code, string type = "weixin")
        {
            Stopwatch stopwatch = CommonHelper.TimerStart();
            
            #region 写入日志
            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 1;
            #endregion
            
            var openid = await GetOpenid(code, type);
            
            var userentity = await CurrentUser();
            if (!string.IsNullOrEmpty(userentity.F_WxOpenId))
            {
                logEntity.F_ExecuteResult = 0;
                logEntity.F_ExecuteResultJson = "该帐号已经绑定!";
                _logIBLL.Write(logEntity,stopwatch);
                
                throw new Exception("#NotLog#该帐号已经绑定");
            }
            else {
                userentity.F_WxOpenId = openid;
                await _userIBLL.SaveEntity(userentity.F_UserId, userentity);

                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = "绑定成功!";
                _logIBLL.Write(logEntity,stopwatch);
            }
        }
        #endregion

        #region 小程序解绑
        /// <summary>
        /// 小程序绑定
        /// </summary>
        /// <returns></returns>
        public async Task OpenIdUnbind()
        {
            Stopwatch stopwatch = CommonHelper.TimerStart();
            
            #region 写入日志
            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 1;
            #endregion
            
            var userentity = await CurrentUser();
            userentity.F_WxOpenId = string.Empty;
            await _userIBLL.SaveEntity(userentity.F_UserId, userentity);

            logEntity.F_ExecuteResult = 1;
            logEntity.F_ExecuteResultJson = "解绑成功!";
            _logIBLL.Write(logEntity,stopwatch);
        }
        #endregion

        #region 小程序一键登录
        /// <summary>
        /// 获取小程序openid
        /// </summary>
        /// <param name="code">小程序打开时生成的</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        public async Task<ResponseDto<LoginOutputDto>> OpenIdLogin(string code, string type = "weixin")
        {
            Stopwatch stopwatch = CommonHelper.TimerStart();
            #region 写入日志
            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 1;
            #endregion
            
            // 加载登录配置
            var loginConfig  = await _iSystemConfigBLL.GetLoginConfig();

            // 登录时长
            var tokenTimeout = 0;
            if (loginConfig != null)
            {
                tokenTimeout = loginConfig.TokenTimeout;
                
                // 限制时间登录
                if (loginConfig.IsLoginTime == 1)
                {
                    if (!IsTimeAuth(loginConfig.LoginTimes))
                    {
                        logEntity.F_ExecuteResult = 0;
                        logEntity.F_ExecuteResultJson = "限制时间登录!";
                        _logIBLL.Write(logEntity,stopwatch);
                        return Fail<LoginOutputDto>("当前时间不允许登录，请联系管理员！");
                    }
                }
            
                //限制IP登录
                if (loginConfig.IsIpAuth == 1)
                {
                    if (!IsIpAuth(loginConfig.IpAuthType, loginConfig.IpAuth))
                    {
                        logEntity.F_ExecuteResult = 0;
                        logEntity.F_ExecuteResultJson = "限制IP登录!";
                        _logIBLL.Write(logEntity,stopwatch);
                        return Fail<LoginOutputDto>("当前IP无法登录，请联系管理员！");
                    }
                }
            }

            var openid = await GetOpenid(code, type);

            var userEntity = await _userIBLL.GetEntityByOpenId(openid,type);
            if (userEntity != null)
            {
                logEntity.F_OperateAccount = userEntity.F_Account;
                logEntity.F_OperateUserId = userEntity.F_UserId;
                logEntity.F_OperateName = userEntity.F_RealName;
            }
            
            if (userEntity == null)
            {
                logEntity.F_ExecuteResult = 0;
                logEntity.F_ExecuteResultJson = "请先登录绑定帐号!";
                _logIBLL.Write(logEntity,stopwatch);
                return Fail<LoginOutputDto>("请先登录绑定帐号");
            }

            // 禁用账号,黑名单账号,离职账号不能登录
            if (userEntity.F_EnabledMark != 1 || userEntity.F_Type == 3 || userEntity.F_IsBlack == 1)
            {
                logEntity.F_ExecuteResult = 0;
                if (userEntity.F_EnabledMark != 1)
                {
                    logEntity.F_ExecuteResultJson = "账户被系统锁定,请联系管理员!";
                }
                if (userEntity.F_Type == 3)
                {
                    logEntity.F_ExecuteResultJson = "离职账号无法登录!";
                }
                if (userEntity.F_IsBlack == 1)
                {
                    logEntity.F_ExecuteResultJson = "黑名单账号无法登录!";
                }

                
                _logIBLL.Write(logEntity,stopwatch);
                return Fail<LoginOutputDto>("账户被系统锁定,请联系管理员!");
            }
            else
            {
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = "登录成功";
                _logIBLL.Write(logEntity,stopwatch);
                
                var loginrandomcode = Guid.NewGuid().ToString();//生成登录成功随机码

                userEntity.F_LoginRandomCode = loginrandomcode;
                userEntity.F_LockCount = 0;
                userEntity.F_LockTime = null;
                userEntity.F_LastLoginTime = DateTime.Now;//最后登录时间
                await _userIBLL.UpdateEntity(userEntity);//更新用户信息
                
                var authToken = await _authTokenBLL.Issue(userEntity, tokenTimeout);
                userEntity.F_Password = string.Empty;
                userEntity.F_Secretkey = string.Empty;
                
                return Success("登录成功", new LoginOutputDto
                {
                    token = authToken.AccessToken,
                    refreshToken = authToken.RefreshToken,
                    expiresIn = authToken.ExpiresIn,
                    refreshExpiresIn = authToken.RefreshExpiresIn,
                    user = userEntity
                });
            }
        }
        #endregion

        #region 获取小程序openid
        /// <summary>
        /// 获取小程序openid
        /// </summary>
        /// <param name="code">小程序打开时生成的</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        public async Task<string> GetOpenid(string code, string type = "weixin")
        {
            switch (type)
            {
                case "weixin":
                    var wxAppId = ConfigHelper.GetConfig().WeixinAppId;//微信小程序 AppId
                    var wxAppSecret = ConfigHelper.GetConfig().WeixinAppSecret;//微信小程序 AppSecret

                    if (string.IsNullOrEmpty(wxAppId) || string.IsNullOrEmpty(wxAppSecret))
                    {
                        throw new Exception("#NotLog#后端未配置微信小程序 AppId 或密钥");
                    }

                    var wxurl = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid={0}&secret={1}&js_code={2}&grant_type=authorization_code", wxAppId, wxAppSecret, code);

                    var wxReqResult = (await HttpMethods.Post(wxurl)).ToJObject();
                    if (wxReqResult == null)
                    {
                        throw new Exception("#NotLog#第三方登录后台验证请求发起失败");
                    }
                    return wxReqResult["openid"].ToString();
                case "feishu":
                    var feishuUrl = "https://open.feishu.cn/open-apis/authen/v2/oauth/token";
                    var feishuPostData = new
                    {
                        grant_type = "authorization_code",
                        client_id = "",
                        client_secret = "",
                        code = code,
                        redirect_uri = "http://192.168.3.5:5173",
                    };
                    var feishuRes = (await  HttpMethods.Post(feishuUrl, feishuPostData.ToJson())).ToJObject();
                    if (feishuRes == null || feishuRes["code"].ToString() != "0")
                    {
                        throw new Exception("#NotLog#第三方登录后台验证请求发起失败" + feishuRes["code"].ToString());
                    }
                     // feishuRes["access_token"];
                    var feishuHeader = new Dictionary<string, string>();
                    feishuHeader.Add("Authorization","Bearer " + feishuRes["access_token"].ToString());
                    // feishuHeader.Add("Content-Type","application/json");
                    
                    
  
                    var feishuUserRes =( await HttpMethods.Get("https://open.feishu.cn/open-apis/authen/v1/user_info",feishuHeader)).ToJObject();
                    if (feishuUserRes == null || feishuUserRes["code"].ToString() != "0")
                    {
                        throw new Exception("#NotLog#第三方登录后台验证请求发起失败" + feishuRes["code"].ToString());
                    }

                    return feishuUserRes["data"]["open_id"].ToString(); 
                default:
                    throw new Exception("#NotLog#平台类别错误（后端暂未支持此平台登录）");
            }
        }
        #endregion
        #endregion
    }
}
