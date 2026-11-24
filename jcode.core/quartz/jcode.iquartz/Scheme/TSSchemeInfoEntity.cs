using jcode.orm;
using System;

namespace jcode.iquartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    [MyTable("lr_ts_schemeinfo")]
    public class TSSchemeInfoEntity
    {
        #region 实体成员 
        /// <summary> 
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 任务运行ID
        /// </summary>
        public string F_RunId { get; set; }
        /// <summary> 
        /// 任务名称
        /// </summary> 
        public string F_Name { get; set; }

        /// <summary> 
        /// 标志1.启用2.未启用3.执行结束
        /// </summary> 
        public int? F_IsActive { get; set; }
        /// <summary> 
        /// 开始时间 
        /// </summary> 
        public DateTime? F_BeginTime { get; set; }
        /// <summary> 
        /// 结束类型1.无限期 2.有限期 
        /// </summary> 
        public int? F_EndType { get; set; }
        /// <summary> 
        /// 结束时间 
        /// </summary> 
        public DateTime? F_EndTime { get; set; }
        /// <summary> 
        /// 看板说明
        /// </summary> 
        public string F_Description { get; set; }
        /// <summary> 
        /// 模板 
        /// </summary> 
        public string F_Scheme { get; set; }
        
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>	
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 运行状态 1正常 2异常
        /// </summary>
        public int? F_RunActive{ get; set; }
        /// <summary>
        /// 运行检测时间
        /// </summary>
        public DateTime? F_RunTestTime { get; set; }
        
        /// <summary>
        /// 应用编码
        /// </summary>
        public string F_AppCode { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
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
    
    [MyTable("lr_ts_schemeinfo")]
    public class TSSchemeInfoNoSchemeEntity
    {
        #region 实体成员 
        /// <summary> 
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 任务运行ID
        /// </summary>
        public string F_RunId { get; set; }
        /// <summary> 
        /// 任务名称
        /// </summary> 
        public string F_Name { get; set; }

        /// <summary> 
        /// 标志1.启用2.未启用3.执行结束
        /// </summary> 
        public int? F_IsActive { get; set; }
        /// <summary> 
        /// 开始时间 
        /// </summary> 
        public DateTime? F_BeginTime { get; set; }
        /// <summary> 
        /// 结束类型1.无限期 2.有限期 
        /// </summary> 
        public int? F_EndType { get; set; }
        /// <summary> 
        /// 结束时间 
        /// </summary> 
        public DateTime? F_EndTime { get; set; }
        /// <summary> 
        /// 看板说明
        /// </summary> 
        public string F_Description { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建用户主键
        /// </summary>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>	
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 运行状态 1正常 2异常
        /// </summary>
        public int? F_RunActive{ get; set; }
        /// <summary>
        /// 运行检测时间
        /// </summary>
        public DateTime? F_RunTestTime { get; set; }
        
        /// <summary>
        /// 应用编码
        /// </summary>
        public string F_AppCode { get; set; }
        /// <summary>
        /// 所属应用主键（lr_plugin_config.F_Id）
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
