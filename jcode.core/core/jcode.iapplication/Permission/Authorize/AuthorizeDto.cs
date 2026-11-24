using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 移动端权限配置提交信息
    /// </summary>
    public class AuthorizeDto
    {
        /// <summary>
        /// 父级Id
        /// </summary>
        public string ParentId { get; set; }
        /// <summary>
        /// 类型 1 模块 2 按钮 3 列 4 表单 5 移动端功能 6 移动功能按钮 7 移动功能列表 8 移动功能表单字段
        /// </summary>
        public int ItemType { get; set; }
        /// <summary>
        /// 权限分类-1角色2用户
        /// </summary>
        public int ObjectType { get; set; }
        /// <summary>
        /// 权限信息
        /// </summary>
        public List<string> Ids { get; set; }
    }


    /// <summary>
    /// 页面权限信息
    /// </summary>
    public class AuthorizeSettingDto
    {
        /// <summary>
        /// 设置的
        /// </summary>
        public IEnumerable<AuthorizeEntity> SettingList { get; set; }
        /// <summary>
        /// 我拥有的
        /// </summary>
        public IEnumerable<string> MyList { get; set; }

    }

    /// <summary>
    /// 页面权限信息
    /// </summary>
    public class AuthorizeModuleDto {
        /// <summary>
        /// 按钮
        /// </summary>
        public IEnumerable<string> Buttons { get; set; }
        /// <summary>
        /// 列表
        /// </summary>
        public IEnumerable<string> Columns { get; set; }
        /// <summary>
        /// 表单
        /// </summary>
        public IEnumerable<string> Forms { get; set; }

    }
}
