using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯用户注册
    /// </summary>
    [MyTable("lr_im_sysuser")]
    public class IMSysUserEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 系统用户名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 系统用户编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 系统用户图标
        /// </summary>
        public string F_Icon { get; set; }
        /// <summary>
        /// 系统用户图标颜色
        /// </summary>
        public string F_Colour { get; set; }
        /// <summary>
        /// 点击消息跳转连接
        /// </summary>
        public string F_Url { get; set; }
        /// <summary>
        /// 点击消息跳转连接(移动端)
        /// </summary>
        public string F_AppUrl { get; set; }
        

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// F_Description
        /// </summary>
        public string F_Description { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否可编辑 0 表示不可编辑 其它值都可编辑
        /// </summary>
        public int? F_IsEdit { get; set; }
        #endregion
    }
}
