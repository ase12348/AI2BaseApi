using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据字典分类
    /// </summary>
    [MyTable("lr_base_dataitem")]
    public class DataItemEntity
    {
        #region 实体成员
        /// <summary>
        /// 分类主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_ItemId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 分类编码
        /// </summary>
        public string F_ItemCode { get; set; }
        /// <summary>
        /// 分类名称
        /// </summary>
        public string F_ItemName { get; set; }
        /// <summary>
        /// 树型结构
        /// </summary>
        public int? F_IsTree { get; set; }
        /// <summary>
        /// 导航标记
        /// </summary>
        public int? F_IsNav { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        public int? F_DeleteMark { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
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
        /// 字典分类的种类(默认框架字典0，iWork系统常量维护1)--ADD BY SSY 20230315
        /// </summary>
        public int? F_Type { get; set; }
        #endregion

        #region 应用归属
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）：从应用工作台新建时自动写入，控制台数据为空
        /// </summary>
        public string F_AppId { get; set; }
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
