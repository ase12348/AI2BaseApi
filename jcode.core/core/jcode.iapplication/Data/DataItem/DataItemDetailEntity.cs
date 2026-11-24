using jcode.orm;
using System;
using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据字典明细
    /// </summary>
    [MyTable("lr_base_dataitemdetail")]
    public class DataItemDetailEntity
    {
        #region 实体成员
        /// <summary>
        /// 明细主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 分类主键
        /// </summary>
        public string F_ItemId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string F_ItemCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string F_ItemName { get; set; }
        /// <summary>
        /// 值
        /// </summary>
        public string F_ItemValue { get; set; }
        /// <summary>
        /// 快速查询
        /// </summary>
        public string F_QuickQuery { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>
        public string F_SimpleSpelling { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public int? F_IsDefault { get; set; }
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
        #endregion

        #region 应用归属
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）：应用工作台里新建时自动写入；
        /// 为空表示全局字典项，所有应用都可见
        /// </summary>
        public string F_AppId { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
    /// <summary>
    /// 数据字典明细集合
    /// </summary>
    public class DataItemDetailDto{
        /// <summary>
        /// 数据字典明细集合
        /// </summary>
        
        public List<DataItemDetailEntity> List { get; set; }
    }
}
