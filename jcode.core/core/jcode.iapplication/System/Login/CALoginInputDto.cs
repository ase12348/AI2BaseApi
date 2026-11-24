using System.ComponentModel.DataAnnotations;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2023.04.26
    /// 描 述：CA登录参数--暂定待联调
    /// </summary>
    public class CALoginInputDto
    {
        /// <summary>
        /// CA登录名
        /// </summary>
        [Required(ErrorMessage = "CA登录名不能为空")]
        public string CaUserSign { get; set; }
        /// <summary>
        /// usbkey的证书--参数暂定
        /// </summary>
        public string CaCer { get; set; }
        /// <summary>
        /// 服务器下送的随机串
        /// </summary>
        public string CaRandom { get; set; }
    }
}
