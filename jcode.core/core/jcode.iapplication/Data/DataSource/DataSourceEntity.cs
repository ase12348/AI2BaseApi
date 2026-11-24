using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据源
    /// </summary>
    [MyTable("lr_base_datasource")]
    public class DataSourceEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string F_Code { get; set; }
        /// <summary>
        /// 名字
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 数据库主键
        /// </summary>
        public string F_DbId { get; set; }
        /// <summary>
        /// sql语句
        /// </summary>
        public string F_Sql { get; set; }
        /// <summary>
        /// 1.表格 2.代码 3.关联 0或者其他sql语句
        /// </summary>
        public int F_Type { get; set; }
        /// <summary>
        /// 分类字段
        /// </summary>
        public string F_Category { get; set; }
        /// <summary>
        /// 备注
        /// </summary>		
        public string F_Description { get; set; }
        /// <summary>
        /// 创建人主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人名字
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 修改人主键
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改人名字
        /// </summary>
        public string F_ModifyUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
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
