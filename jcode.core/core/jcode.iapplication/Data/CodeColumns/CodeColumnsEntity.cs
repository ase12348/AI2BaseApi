using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期：2021-09-18 10:13:07
    /// 描 述：lr_db_codecolumns(数据库表字段信息)表的实体
    /// </summary>
    [MyTable("lr_db_codecolumns")]
    public class CodeColumnsEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识码(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 表外键【lr_db_codetable】
        /// </summary>
        public string F_CodeTableId { get; set; }
        /// <summary>
        /// 列名
        /// </summary>
        public string F_DbColumnName { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 是否自增【1是，0否】
        /// </summary>
        public int? F_IsIdentity { get; set; }
        /// <summary>
        /// 是否主键【1是，0否】
        /// </summary>
        public int? F_IsPrimaryKey { get; set; }
        /// <summary>
        /// 是否为空【1是，0否】
        /// </summary>
        public int? F_IsNullable { get; set; }
        /// <summary>
        /// C#字段类型
        /// </summary>
        public string F_CsType { get; set; }
        /// <summary>
        /// 数据库字段类型
        /// </summary>
        public string F_DbType { get; set; }

        /// <summary>
        /// 字段长度
        /// </summary>
        public int? F_Length { get; set; }
        /// <summary>
        /// 小数位数
        /// </summary>
        public int? F_DecimalDigits { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public int? F_Sort { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
