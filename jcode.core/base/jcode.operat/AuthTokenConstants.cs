namespace jcode.operat
{
    /// <summary>
    /// 双令牌机制常量
    /// </summary>
    public static class AuthTokenConstants
    {
        /// <summary>
        /// 访问令牌类型
        /// </summary>
        public const string AccessType = "access";
        /// <summary>
        /// 刷新令牌类型
        /// </summary>
        public const string RefreshType = "refresh";
        /// <summary>
        /// 刷新令牌请求头（不使用 URL/Body，避免被日志记录；Sass 中间件也依赖它解析租户）
        /// </summary>
        public const string RefreshTokenHeader = "X-Refresh-Token";
        /// <summary>
        /// 访问令牌过期错误码
        /// </summary>
        public const string ErrorExpired = "TokenExpiredException";
        /// <summary>
        /// 签名错误码
        /// </summary>
        public const string ErrorSignature = "SignatureVerificationException";
        /// <summary>
        /// 令牌类型不匹配错误码
        /// </summary>
        public const string ErrorTypeMismatch = "TokenTypeException";
    }
}
