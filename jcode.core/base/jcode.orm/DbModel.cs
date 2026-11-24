using System;
using System.Collections.Generic;
using SqlSugar;

namespace jcode.orm
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据库连接
    /// </summary>
    public class DbModel
    {
        /// <summary>
        /// 数据库编码
        /// </summary>
        /// <returns></returns>
        public string F_DBName { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        /// <returns></returns>
        public string F_DbType { get; set; }
        /// <summary>
        /// 连接地址
        /// </summary>
        /// <returns></returns>
        public string F_DbConnection { get; set; }
        /// <summary>
        /// 租户id
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 租户自己添加
        /// </summary>
        public int? F_IsEdit { get; set; }
        
        /// <summary>
        /// 是否读写分离,默认都不是1是
        /// </summary>
        public int? F_IsSlave{ get; set; }
        
        /// <summary>
        /// 读写分离配置
        /// </summary>
        public string F_SlaveJson{ get; set; }
        
        /// <summary>
        /// 是否加密存储
        /// </summary>
        public int? F_DESEncrypt{ get; set; }
        
    }

    
    /// <summary>
    /// 从库配置
    /// </summary>
    public class SlaveModel
    {   
        /// <summary>
        /// 链接串
        /// </summary>
        public string DbConnection { get; set; }
        /// <summary>
        /// 权重
        /// </summary>
        public int HitRate{ get; set; }
    }

    /// <summary>
    /// 数据库日志记录
    /// </summary>
    public class DbDiffLog
    {
        /// <summary>
        /// 
        /// </summary>
        public List<DiffLogTableInfo> AfterData { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public List<DiffLogTableInfo> BeforeData { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public TimeSpan? Time { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public DiffType DiffType { get; set; }
    }
}
