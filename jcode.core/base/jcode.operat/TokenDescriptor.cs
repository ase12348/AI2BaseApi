using System;

namespace jcode.operat
{
    /// <summary>
    /// 签发 JWT 所需的声明信息
    /// </summary>
    public class TokenDescriptor
    {
        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 租户编号
        /// </summary>
        public string TenantNo { get; set; }
        /// <summary>
        /// 登录随机代码
        /// </summary>
        public string LoginRandomCode { get; set; }
        /// <summary>
        /// 会话标识
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// 令牌唯一标识
        /// </summary>
        public string Jti { get; set; }
        /// <summary>
        /// 令牌类型：access / refresh
        /// </summary>
        public string Typ { get; set; }
        /// <summary>
        /// 签发时间
        /// </summary>
        public DateTime IssuedAt { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpiresAt { get; set; }
        /// <summary>
        /// 登录时间（在线用户列表展示用；刷新签发的访问令牌沿用登录时间）
        /// </summary>
        public DateTime? LoginTime { get; set; }
    }
}
