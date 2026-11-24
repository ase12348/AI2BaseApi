using jcode.autofac;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：双令牌签发、刷新、注销
    /// </summary>
    public interface IAuthTokenBLL : IBLL
    {
        /// <summary>
        /// 登录成功后签发访问令牌与刷新令牌，并创建服务端会话
        /// </summary>
        /// <param name="user">登录用户</param>
        /// <param name="sessionHours">会话绝对有效小时数（登录配置 TokenTimeout，0 时取 JwtExp）</param>
        /// <returns></returns>
        Task<AuthTokenResult> Issue(UserEntity user, int sessionHours);

        /// <summary>
        /// 用刷新令牌换取新的令牌对（轮换刷新令牌）
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns>失败返回 null</returns>
        Task<AuthTokenResult> Refresh(string refreshToken);

        /// <summary>
        /// 注销会话（刷新令牌可解析时按会话注销，否则静默忽略）
        /// </summary>
        /// <param name="refreshToken">刷新令牌</param>
        /// <returns></returns>
        Task Logout(string refreshToken);

        /// <summary>
        /// 吊销指定账号的全部会话
        /// </summary>
        /// <param name="account">账号</param>
        /// <returns></returns>
        Task RevokeAll(string account);
    }
}
