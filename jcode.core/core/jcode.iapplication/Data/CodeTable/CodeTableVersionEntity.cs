using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present
    /// 创建人：tobin
    /// 日 期：2022-05-09 10:38:26
    /// 描 述：lr_db_version(数据库表信息)表的版本实体
    /// </summary>
    [MyTable("lr_db_version")]
    public class CodeTableVersionEntity
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
        public string F_Version_No { get; set; }
        #endregion
    }
}
