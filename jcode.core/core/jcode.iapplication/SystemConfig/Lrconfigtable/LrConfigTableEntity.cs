using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-10-10 09:51:29
    /// 描 述：lr_config_table(数据列表配置【lr_config_table】)表的实体
    /// </summary>
    [MyTable("lr_config_table")]
    public class LrConfigTableEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 视图编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 视图名称
        /// </summary>
        public string F_Title { get; set; }
        /// <summary>
        /// 类型(查询视图1，表格显示列2)
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 功能Id
        /// </summary>
        public string F_ObjectId { get; set; }
        /// <summary>
        /// 可见性(默认私有0，公有1)
        /// </summary>
        public int? F_IsPublic { get; set; }
        /// <summary>
        /// 配置信息json
        /// </summary>
        public string F_Content { get; set; }
        /// <summary>
        /// 使用状态(默认使用0，未使用1)
        /// </summary>
        public int? F_State { get; set; }
        /// <summary>
        /// 删除标记(0正常，1删除)
        /// </summary>
        public int? F_DeleteMark { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户id
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建用户名称
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户id
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户名称
        /// </summary>
        public string F_ModifyUserName { get; set; }
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
        /// <summary>
        /// 修改日期(时间查询范围)
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_ModifyDateQRange { get; set; }

        #endregion
    }
}