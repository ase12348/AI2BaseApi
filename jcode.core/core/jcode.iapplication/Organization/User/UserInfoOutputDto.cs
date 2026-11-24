using System;
using System.Collections.Generic;
using System.Text;

namespace jcode.iapplication.Organization.User
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.26
    /// 描 述：用户信息
    /// </summary>
    public class UserInfoOutputDto
    {

    }
    /// <summary>
    /// 用户信息
    /// </summary>
    public class UserInfo {
        /// <summary>
        /// 账号
        /// </summary>
        public string account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string name { get; set; }
        /// <summary>
        /// 头像
        /// </summary>
        public string avatar { get; set; }
        /// <summary>
        /// 公司ID
        /// </summary>
        public string companyId { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string companyName { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public string departmentId { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string departmentName { get; set; }
    }
}
