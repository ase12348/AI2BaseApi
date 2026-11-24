using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022-05-05 11:14:02
    /// 描 述：lr_base_dbfield(建表常用字段)表的实体
    /// </summary>
    [MyTable("lr_base_dbfield")]
    public class DbFieldEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string F_DataType { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string F_Remark { get; set; }
        /// <summary>
        /// 字段长度
        /// </summary>
        public int? F_Length { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人名字
        /// </summary>
        public string F_CreateUserName { get; set; }

        #endregion


        #region 多租户
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
