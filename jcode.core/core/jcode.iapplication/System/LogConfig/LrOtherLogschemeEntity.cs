using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-03-28 16:23:13
    /// 描 述：lr_other_logscheme(日志接口配置-详情【lr_other_logscheme】)表的实体
    /// </summary>
    [MyTable("lr_other_logscheme")]
    public class LrOtherLogschemeEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 日志接口配置外键(lr_other_logconfig)
        /// </summary>
        public string F_LogConfigId { get; set; }
        /// <summary>
        /// 编码
        /// </summary>
        public string F_EnCode { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        public string F_FullName { get; set; }
        /// <summary>
        /// 关联类型0:无关联;1:数据字典;2:数据视图;3:公司信息;4:部门信息;5:用户信息;
        /// </summary>
        public int? F_RelationType { get; set; }
        /// <summary>
        /// 数据字典编号
        /// </summary>
        public string F_DataItemCode { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? F_SortCode { get; set; }
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
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 数据视图编码--ADD BY SSY 20230711
        /// </summary>
        public string F_DSourceCode { get; set; }
        /// <summary>
        /// 关联字段--ADD BY SSY 20230711
        /// </summary>
        public string F_DSourceRelevance { get; set; }
        /// <summary>
        /// 显示字段--ADD BY SSY 20230711
        /// </summary>
        public string F_DSourceReveal { get; set; }

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