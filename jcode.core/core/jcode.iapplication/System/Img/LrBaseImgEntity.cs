using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-图片模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2022-07-07 11:19:40
    /// 描 述：lr_base_img(极简搭图片存储表)表的实体
    /// </summary>
    [MyTable("lr_base_img")]
    public class LrBaseImgEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string F_FileName { get; set; }
        /// <summary>
        /// 文件扩展名
        /// </summary>
        public string F_FileExtensions { get; set; }
        /// <summary>
        /// 文件分类
        /// </summary>
        public string F_FileType { get; set; }
        /// <summary>
        /// 是否公共
        /// </summary>
        public int? F_IsPublic { get; set; }
        /// <summary>
        /// 创建日期
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
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }

        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateDateQRange { get; set; }

        #endregion
    }
}