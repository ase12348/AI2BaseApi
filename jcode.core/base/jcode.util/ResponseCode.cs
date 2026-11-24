namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.11
    /// 描 述：接口响应码 200 成功，400 失败，500 异常，401 没有登录信息
    /// </summary>
    public enum ResponseCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        success = 200,
        /// <summary>
        /// 失败
        /// </summary>
        fail = 400,
        /// <summary>
        /// 异常
        /// </summary>
        exception = 500,
        /// <summary>
        /// 没有登录信息
        /// </summary>
        nologin = 401,
        /// <summary>
        /// 访问令牌过期（前端凭刷新令牌换取新令牌）
        /// </summary>
        tokenexpired = 4011
    }
}
