using System;

namespace jcode.operat
{
    /// <summary>
    /// 令牌中的信息
    /// </summary>
    public class Payload
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// 用户账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 租户编号
        /// </summary>
        public string TenantNo { get; set; }
        /// <summary>
        /// 安全等级：1超级用户 2三员-系统管理员、3三员-安全管理员、4三员-审计员
        /// </summary>
        public string SecurityLevel { get; set; }
        /// <summary>
        /// 用户密级：0公开，1内部，2秘密，3机密，4绝密
        /// </summary>
        public string SecretLevel { get; set; }
        /// <summary>
        /// 登录随机代码
        /// </summary>
        public string LoginRandomCode { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public DateTime Time{ get; set; }
        /// <summary>
        /// 令牌类型：access 访问令牌，refresh 刷新令牌
        /// </summary>
        public string Typ { get; set; }
        /// <summary>
        /// 会话标识（访问令牌与刷新令牌共用）
        /// </summary>
        public string Sid { get; set; }
        /// <summary>
        /// 令牌唯一标识
        /// </summary>
        public string Jti { get; set; }
        /// <summary>
        /// 过期时间（Unix 秒）
        /// </summary>
        public long Exp { get; set; }
    }
}
