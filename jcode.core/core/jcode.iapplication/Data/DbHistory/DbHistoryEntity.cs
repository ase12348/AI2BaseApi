using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.03.20
    /// 描 述：数据库操作记录
    /// </summary>
    [MyTable("lr_db_history")]
    public class DbHistoryEntity
    {
        /// <summary>
        /// 唯一标识码(GUID)
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 操作动作1建表 2 更新表
        /// </summary>
        public int? F_Action { get; set; }
        /// <summary>
        /// 数据库id
        /// </summary>
        public string F_DbId { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string F_DbName { get; set; }
        /// <summary>
        /// 表格名称
        /// </summary>
        public string F_TableName { get; set; }
        /// <summary>
        /// 具体描述
        /// </summary>
        public string F_Des { get; set; }
        /// <summary>
        /// 创建人id
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }



        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
