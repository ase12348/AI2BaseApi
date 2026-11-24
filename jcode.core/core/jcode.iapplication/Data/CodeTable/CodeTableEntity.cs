using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present
    /// 创建人：施赛一
    /// 日 期：2021-09-18 10:38:26
    /// 描 述：lr_db_codetable(数据库表信息)表的实体
    /// </summary>
    [MyTable("lr_db_codetable")]
    public class CodeTableEntity
    {
        #region 实体成员
        /// <summary>
        /// 唯一标识码(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string F_ClassName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 数据库连接外键【lr_base_databaselink】
        /// </summary>
        public string F_DbId { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string F_Description { get; set; }
        /// <summary>
        /// 是否锁表【1是，0否】
        /// </summary>
        public int? F_IsLock { get; set; }
        /// <summary>
        /// 是否删除【1是，0否】
        /// </summary>
        public int? F_DeleteMark { get; set; }
        /// <summary>
        /// 模型与数据库间的同步关系 1 同步 0 未同步
        /// </summary>
        public int? F_State { get; set; }

        #endregion

        #region 应用归属
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）：从应用工作台新建/导入时自动写入，控制台数据为空
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
}