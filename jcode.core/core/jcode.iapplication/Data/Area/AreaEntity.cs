using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.18
    /// 描 述：行政区域
    /// </summary>
    [MyTable("lr_base_area")]
    public class AreaEntity
    {
        #region 实体成员
        /// <summary>
        /// 区域主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_AreaId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 区域编码
        /// </summary>
        public string F_AreaCode { get; set; }
        /// <summary>
        /// 区域名称
        /// </summary>
        public string F_AreaName { get; set; }
        /// <summary>
        /// 快速查询
        /// </summary>
        public string F_QuickQuery { get; set; }
        /// <summary>
        /// 简拼
        /// </summary>
        public string F_SimpleSpelling { get; set; }
        /// <summary>
        /// 层次
        /// </summary>
        public int? F_Layer { get; set; }
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
    }
}
