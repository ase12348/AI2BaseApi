using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：数据表模型与物理表之间的列差异，供「更新到表」提前确认用
    /// </summary>
    public class CodeTableColumnDiffDto
    {
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 物理表当前是否存在
        /// </summary>
        public bool TableExists { get; set; }

        /// <summary>
        /// 是否宿主自有表：为真时不允许由模型改结构，也不允许回收列
        /// </summary>
        public bool IsFrameworkTable { get; set; }

        /// <summary>
        /// 模型有、库里没有：本次会新建的列
        /// </summary>
        public List<CodeTableDiffColumnDto> Add { get; set; } = new List<CodeTableDiffColumnDto>();

        /// <summary>
        /// 库里有、模型没有：可以被回收的列，必须逐条确认后才执行
        /// </summary>
        public List<CodeTableDiffColumnDto> Drop { get; set; } = new List<CodeTableDiffColumnDto>();
    }

    /// <summary>
    /// 差异列
    /// </summary>
    public class CodeTableDiffColumnDto
    {
        /// <summary>
        /// 列名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 列说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 数据库类型
        /// </summary>
        public string DataType { get; set; }

        /// <summary>
        /// 是否主键
        /// </summary>
        public bool IsPrimaryKey { get; set; }

        /// <summary>
        /// 是否自增
        /// </summary>
        public bool IsIdentity { get; set; }

        /// <summary>
        /// 受保护：主键或自增列，提交回收时必须被拒
        /// </summary>
        public bool IsProtected { get; set; }
    }

    /// <summary>
    /// 数据表模型里已经不该存在的登记行
    /// </summary>
    public class CodeTableInvalidDto
    {
        /// <summary>
        /// 登记行主键（lr_db_codetable.F_Id）
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 表说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 失效原因：missing=库里已经没有这张表，framework=宿主自有表不该进模型
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 物理表是否还在：为真时清掉登记并不会删除表本身
        /// </summary>
        public bool PhysicalExists { get; set; }
    }
}
