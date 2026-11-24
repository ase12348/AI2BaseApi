using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-03-28 16:23:13
    /// 描 述：lr_other_logconfig(日志接口配置【lr_other_logconfig】)表的实体
    /// </summary>
    [MyTable("lr_other_logconfig")]
    public class LrOtherLogconfigEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 接口名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 接口地址
        /// </summary>
        public string F_Address { get; set; }
        /// <summary>
        /// 请求方式【常量维护】
        /// </summary>
        public string F_RequestMethod { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 删除标记(0正常，1删除)
        /// </summary>
        public int? F_DeleteMark { get; set; }
        /// <summary>
        /// 有效标志(1正常，0禁用)
        /// </summary>
        public int? F_EnabledMark { get; set; }
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
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_ModifyUserName { get; set; }
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否启用密级过滤(0否，1是)
        /// </summary>
        public int? F_IsSecretFilter { get; set; }
        /// <summary>
        /// 日志描述保存类型(1全部，2仅操作前，3仅操作后)--ADD BY SSY 20230718
        /// </summary>
        public int? F_MsgType { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateDateQRange { get; set; }
        /// <summary>
        /// 修改日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_ModifyDateQRange { get; set; }

        #endregion
    }
}