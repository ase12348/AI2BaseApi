namespace jcode.iapplication
{
    /// <summary>
    /// 双令牌签发结果
    /// </summary>
    public class AuthTokenResult
    {
        /// <summary>
        /// 访问令牌
        /// </summary>
        public string AccessToken { get; set; }
        /// <summary>
        /// 刷新令牌
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// 访问令牌有效期（秒）
        /// </summary>
        public int ExpiresIn { get; set; }
        /// <summary>
        /// 刷新令牌剩余有效期（秒，会话绝对过期时间）
        /// </summary>
        public int RefreshExpiresIn { get; set; }
    }

    /// <summary>
    /// 账号级吊销标记（仅 Redis 会话模式使用）
    /// </summary>
    public class AuthRevokeMarker
    {
        /// <summary>
        /// 吊销时间：早于该时间创建的会话全部失效
        /// </summary>
        public System.DateTime Time { get; set; }
    }
}
