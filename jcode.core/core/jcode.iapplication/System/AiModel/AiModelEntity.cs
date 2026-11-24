using System;
using jcode.orm;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.09.20
    /// 描 述：模型管理
    /// </summary> 
    [MyTable("lr_base_aimodel")]
    public class AiModelEntity
    {
        #region 实体成员 
        /// <summary> 
        /// 主键 
        /// </summary> 
        /// <returns></returns> 
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary> 
        /// 提供商 
        /// </summary> 
        /// <returns></returns> 
        public string F_Provider { get; set; }
        /// <summary> 
        /// 类型 
        /// </summary> 
        /// <returns></returns> 
        public string F_Type { get; set; }
        /// <summary> 
        /// 模型 
        /// </summary> 
        /// <returns></returns> 
        public string F_Model { get; set; }
        /// <summary> 
        /// API密钥 
        /// </summary> 
        /// <returns></returns> 
        public string F_ApiKey { get; set; }
        /// <summary> 
        /// 接口地址（自定义 OpenAI 兼容网关时使用） 
        /// </summary> 
        /// <returns></returns> 
        public string F_Url { get; set; }
        /// <summary>
        /// 有效标志
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        /// 备注
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
        /// 创建用户
        /// </summary>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改日期
        /// </summary>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改用户主键
        /// </summary>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改用户
        /// </summary>
        public string F_ModifyUserName { get; set; }
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
