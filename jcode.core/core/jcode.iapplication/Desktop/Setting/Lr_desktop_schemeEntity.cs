using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期：2021-08-19 15:29:43
    /// 描 述：lr_desktop_scheme表的实体
    /// </summary>
    [MyTable("lr_desktop_scheme")]
    public class Lr_desktop_schemeEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public String F_Id { get; set; }
        /// <summary>
        /// 设计内容
        /// </summary>
        public String F_Content { get; set; }
        /// <summary>
        /// 1.正式2.草稿
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? F_CreateTime { get; set; }
        /// <summary>
        /// 创建人员
        /// </summary>
        public String F_CreateUserId { get; set; }
        /// <summary>
        /// 桌面信息主键
        /// </summary>
        public String F_InfoId { get; set; }
        
                
        /// <summary>
        /// 版本号
        /// </summary>
        public string F_Ver { get; set; }
        /// <summary>
        /// 版本描述
        /// </summary>
        public string F_Des { get; set; }

        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}