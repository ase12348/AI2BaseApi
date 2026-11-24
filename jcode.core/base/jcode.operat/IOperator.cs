using System.Threading.Tasks;

namespace jcode.operat
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：当前连接用户信息处理类
    /// </summary>
    public interface IOperator
    {
        /// <summary>
        /// 生成jwt令牌（访问令牌/刷新令牌共用）
        /// </summary>
        /// <param name="descriptor">令牌声明信息</param>
        /// <returns></returns>
        string EncodeToken(TokenDescriptor descriptor);

        /// <summary>
        /// 解密jwt令牌（要求 typ=access）
        /// </summary>
        /// <param name="token">令牌；为空时从请求头/Query/Form中读取</param>
        /// <returns>TokenExpiredException 时间过期, SignatureVerificationException 签证不正确, TokenTypeException 令牌类型错误</returns>
        string DecodeToken(string token = "");

        /// <summary>
        /// 解析jwt令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="expectedTyp">期望的令牌类型（access/refresh），为空不校验</param>
        /// <param name="ignoreExpiration">是否忽略过期时间（仅用于解析租户号等不授权场景）</param>
        /// <returns></returns>
        TokenDecodeResult Decode(string token, string expectedTyp = null, bool ignoreExpiration = false);
    }
}
