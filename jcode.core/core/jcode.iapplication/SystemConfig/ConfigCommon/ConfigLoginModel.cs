using System;
using System.Collections.Generic;
using System.Text;

namespace jcode.iapplication
{
    /// <summary>
    /// 登录配置信息
    /// </summary>
    public class ConfigLoginModel
    {
        /// <summary>
        /// 登录方式(1单一登录，2同时登录)
        /// </summary>
        public int LoginType { get; set; }
        /// <summary>
        /// 登录时长(小时)
        /// </summary>
        public int TokenTimeout { get; set; }
        
        /// <summary>
        /// 密码错误验证次数(1是，0否)
        /// </summary>
        public int IsPwdVerify { get; set; }
        /// <summary>
        /// 密码错误次数
        /// </summary>
        public int ErrorPwdCount { get; set; }
        /// <summary>
        /// 密码多次错误账号状态(1账号锁定，2延时登录,30分钟)
        /// </summary>
        public int ErrorPwdType { get; set; }
        
        /// <summary>
        /// 是否限制登录时间(1是，0否)
        /// </summary>
        public int? IsLoginTime { get; set; }
        /// <summary>
        /// 登录时间
        /// </summary>
        public List<ConfigLoginTimeModel> LoginTimes { get; set; }
        
        /// <summary>
        /// 是否限定IP(1是，0否)
        /// </summary>
        public int IsIpAuth { get; set; }
        /// <summary>
        /// 限定类型(1白名单，2黑名单)
        /// </summary>
        public int IpAuthType { get; set; }
        /// <summary>
        /// 限定IP
        /// </summary>
        public string IpAuth { get; set; }
    }
    
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-05-18 09:37:38
    /// 描 述：登录策略-登录时间
    /// </summary>
    public class ConfigLoginTimeModel
    {
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? startTime { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? endTime { get; set; }
    }
    
    
}
