using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：登录会话（双令牌）；Redis 可用时存缓存，否则落本表
    /// </summary>
    [MyTable("lr_base_auth_session")]
    public class AuthSessionEntity
    {
        /// <summary>
        /// 会话标识（sid），同时作为访问令牌与刷新令牌的会话键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }

        /// <summary>
        /// 用户账号
        /// </summary>
        [Column(IsNullable = true)]
        public string F_Account { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        [Column(IsNullable = true)]
        public string F_UserId { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        [Column(IsNullable = true)]
        public string F_UserName { get; set; }
        /// <summary>
        /// 租户编号（管理租户为空）
        /// </summary>
        [Column(IsNullable = true)]
        public string F_TenantNo { get; set; }
        /// <summary>
        /// 登录随机代码
        /// </summary>
        [Column(IsNullable = true)]
        public string F_LoginRandomCode { get; set; }

        /// <summary>
        /// 当前有效的刷新令牌 jti
        /// </summary>
        [Column(IsNullable = true)]
        public string F_RefreshJti { get; set; }
        /// <summary>
        /// 上一个刷新令牌 jti（并发宽限期内使用）
        /// </summary>
        [Column(IsNullable = true)]
        public string F_PrevRefreshJti { get; set; }
        /// <summary>
        /// 上一个刷新令牌的宽限截止时间
        /// </summary>
        [Column(IsNullable = true)]
        public DateTime? F_PrevExpireTime { get; set; }

        /// <summary>
        /// 会话创建（登录）时间
        /// </summary>
        [Column(IsNullable = true)]
        public DateTime? F_CreateTime { get; set; }
        /// <summary>
        /// 最近一次刷新时间
        /// </summary>
        [Column(IsNullable = true)]
        public DateTime? F_LastRefreshTime { get; set; }
        /// <summary>
        /// 会话绝对过期时间（不随刷新延长）
        /// </summary>
        [Column(IsNullable = true)]
        public DateTime? F_ExpireTime { get; set; }

        /// <summary>
        /// 是否已注销/吊销：1 是，0 否
        /// </summary>
        [Column(IsNullable = true)]
        public int? F_Revoked { get; set; }
        /// <summary>
        /// 注销/吊销时间
        /// </summary>
        [Column(IsNullable = true)]
        public DateTime? F_RevokedTime { get; set; }

        /// <summary>
        /// 登录IP
        /// </summary>
        [Column(IsNullable = true)]
        public string F_CreateIp { get; set; }
        /// <summary>
        /// 登录设备（pc/app）
        /// </summary>
        [Column(IsNullable = true)]
        public string F_Device { get; set; }
    }
}
