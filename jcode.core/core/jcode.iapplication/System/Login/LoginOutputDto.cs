namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.25
    /// 描 述：登录返回数据
    /// </summary>
    public class LoginOutputDto
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string refreshToken { get; set; }
        /// <summary>
        /// 访问令牌有效期（秒）
        /// </summary>
        public int expiresIn { get; set; }
        /// <summary>
        /// 刷新令牌剩余有效期（秒，会话绝对过期时间，不随刷新延长）
        /// </summary>
        public int refreshExpiresIn { get; set; }
        /// <summary>
        /// 用户数据
        /// </summary>
        public UserEntity user { get; set; }
    }

    /// <summary>
    /// 获取openid返回结果
    /// </summary>
    public class OpenidRes
    {
        /// <summary>
        /// 返回的openid值
        /// </summary>
        public string openid { get; set; }
        /// <summary>
        /// openid类型【微信、支付宝、钉钉】
        /// </summary>
        public string type { get; set; }
    }
}
