using System.ComponentModel.DataAnnotations;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：修改密码
    /// </summary>
    public class UserUpdatePswInputDto
    {
        /// <summary>
        /// 新密码
        /// </summary>
        [Required(ErrorMessage = "不能为空")]
        public string password { get; set; }
        /// <summary>
        /// 旧密码
        /// </summary>
        [Required(ErrorMessage = "不能为空")]
        public string oldPassword { get; set; }
    }
}
