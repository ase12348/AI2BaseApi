using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：岗位管理
    /// </summary>
    [MyTable("lr_base_post")]
    public class PostEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_PostId { get; set; }
        /// <summary>
        /// 上级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 岗位名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 岗位编号
        /// </summary>
        public string F_EnCode { get; set; }
        /// <summary>
        /// 公司主键
        /// </summary>
        public string F_CompanyId { get; set; }
        /// <summary>
        /// 部门主键
        /// </summary>
        public string F_DepartmentId { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        public int? F_DeleteMark { get; set; }
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

        /// <summary>
        /// 部门名称
        /// </summary>
        [Column(IsIgnore = true)]
        public string DeptName { get; set; }
        
        /// <summary>
        /// 公司名称
        /// </summary>
        [Column(IsIgnore = true)]
        public string CompanyName { get; set; }
        
        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
