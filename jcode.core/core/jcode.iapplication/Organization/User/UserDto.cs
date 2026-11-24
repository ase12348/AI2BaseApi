using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.08.02
    /// 描 述：用户输入参数
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// 账号信息
        /// </summary>
        public UserEntity User { get;set;}
        /// <summary>
        /// 岗位信息
        /// </summary>
        public IEnumerable<UserRelationEntity> Posts { get; set; }
        /// <summary>
        /// 角色信息
        /// </summary>
        public IEnumerable<UserRelationEntity> Roles { get; set; }
        /// <summary>
        /// 负责部门信息
        /// </summary>
        public IEnumerable<UserRelationEntity> Departments { get; set; }
        /// <summary>
        /// 负责公司信息
        /// </summary>
        public IEnumerable<UserRelationEntity> Companys { get; set; }

    }
}
