using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导入设置字段
    /// </summary>
    [MyTable("lr_excel_importfileds")]
    public class ExcelImportFieldEntity
    {
        #region 实体成员
        /// <summary>
        /// 导入模板Id
        /// </summary>
        public string F_ImportId { get; set; }
        /// <summary>
        /// 字段名字
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// excel名字
        /// </summary>
        public string F_ColName { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string F_CsType { get; set; }
        /// <summary>
        /// 唯一性验证:0要,1需要
        /// </summary>
        public int? F_OnlyOne { get; set; }
        /// <summary>
        /// 关联类型0:无关联,1:GUID,2:数据字典3:数据表;4:固定数值;5:操作人ID;6:操作人名字;7:操作时间;8:单据编码;
        /// </summary>
        public int? F_RelationType { get; set; }
        /// <summary>
        /// 数据字典编号
        /// </summary>
        public string F_DataItemCode { get; set; }
        /// <summary>
        /// 固定数据
        /// </summary>
        public string F_Value { get; set; }
        /// <summary>
        /// 关联数据源
        /// </summary>
        public string F_DSourceId { get; set; }
        /// <summary>
        /// 关联数据源导入对应字段
        /// </summary>
        public string F_DSourceExcelId { get; set; }
        /// <summary>
        /// 关联数据源保存字段
        /// </summary>
        public string F_DSourceDBId { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 单据编码编号
        /// </summary>
        public string F_EnCode { get; set; }
        #endregion


        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
