using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present
    /// 创建人：tobin
    /// 日 期：2022-05-09 10:38:26
    /// 描 述：lr_db_data(数据库表信息)表的数据实体
    /// </summary>
    [MyTable("lr_db_data")]
    public class CodeTableDataEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识码(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 主键
        /// </summary>
        public string F_MainKey { get; set; }
        /// <summary>
        /// 主键值
        /// </summary>
        public string F_MainKeyValue { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int F_Sort { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public string F_Data { get; set; }
        #endregion
    }
}
