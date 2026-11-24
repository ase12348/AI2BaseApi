using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期：2021-08-19 15:29:43
    /// 描 述：lr_desktop_info表的实体
    /// </summary>
    [MyTable("lr_desktop_info")]
    public class Lr_desktop_infoEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public String F_Id { get; set; }
        /// <summary>
        /// 菜单id
        /// </summary>
        public String F_ModuleId{ get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 分类
        /// </summary>
        public string F_Category { get; set; }

        /// <summary>
        /// 是否首屏打开 1 是 0否
        /// </summary>
        public int? F_IsFirst { get; set; }
        /// <summary>
        /// 状态 1 有效 0 无效
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 设计模板主键
        /// </summary>
        public string F_SchemeId { get; set; }
        /// <summary>
        /// 发布的版本
        /// </summary>
        public string F_ReleaseId { get; set; }
        /// <summary>
        /// 类型1pc 2移动端
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
        /// </summary>
        public string F_AppId { get; set; }
        #endregion

        #region 扩展属性
        /// <summary>
        /// 创建时间
        /// </summary>
        [Column(IsIgnore = true)]
        public DateTime? F_CreateTime { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        [Column(IsIgnore = true)]
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        [Column(IsIgnore = true)]
        public string Img { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}