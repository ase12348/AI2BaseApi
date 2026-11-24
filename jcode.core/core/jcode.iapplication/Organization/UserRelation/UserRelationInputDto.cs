using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 成员关系
    /// </summary>
    public class UserRelationInputDto
    {
        /// <summary>
        /// 用户id字段
        /// </summary>
        public string UserIds { get; set; }
    }
    
    
    /// <summary>
    /// 成员关系
    /// </summary>
    public class UserRelationRoleDto
    {
        /// <summary>
        /// 当前岗位
        /// </summary>
        public string PostId { get; set; }

        /// <summary>
        /// 用户id字段
        /// </summary>
        public List<string> Roles { get; set; }
    }
}
