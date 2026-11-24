using jcode.autofac;
using jcode.util;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.25
    /// 描 述：登录模块
    /// </summary>
    public interface ILoginBLL : IBLL
    {
        /// <summary>
        /// 登录操作
        /// </summary>
        /// <param name="loginInputDto">登录输入参数</param>
        /// <returns></returns>
        Task<ResponseDto<LoginOutputDto>> Login(LoginInputDto loginInputDto);

        /// <summary>
        /// 用刷新令牌换取新的令牌对
        /// </summary>
        /// <param name="refreshToken">刷新令牌（请求头 X-Refresh-Token）</param>
        /// <returns></returns>
        Task<ResponseDto<LoginOutputDto>> Refresh(string refreshToken);

        /// <summary>
        /// 注销当前会话
        /// </summary>
        /// <param name="refreshToken">刷新令牌（请求头 X-Refresh-Token）</param>
        /// <returns></returns>
        Task Logout(string refreshToken);

        #region 小程序登录

        /// <summary>
        /// 小程序绑定
        /// </summary>
        /// <param name="code">小程序打开时生成的</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        Task OpenIdBind(string code, string type = "weixin");
        /// <summary>
        /// 小程序解绑
        /// </summary>
        /// <returns></returns>
        Task OpenIdUnbind();
        /// <summary>
        /// 获取小程序openid ，小程序不能将secretid暴露出来，所以要通过后端获取openid
        /// </summary>
        /// <param name="code">小程序打开时生成的，有且只能调用一次</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        Task<string> GetOpenid(string code, string type = "weixin");
        
        /// <summary>
        /// 获取小程序openid
        /// </summary>
        /// <param name="code">小程序打开时生成的</param>
        /// <param name="type">类型，默认 weixin</param>
        /// <returns></returns>
        Task<ResponseDto<LoginOutputDto>> OpenIdLogin(string code, string type = "weixin");
        #endregion
    }
}
