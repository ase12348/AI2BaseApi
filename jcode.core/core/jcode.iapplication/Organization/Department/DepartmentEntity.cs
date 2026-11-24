using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：部门管理
    /// </summary>
    [MyTable("lr_base_dept")]
    public class DepartmentEntity
    {
        #region 实体成员
        /// <summary>
        /// 部门主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_DepartmentId { get; set; }
        /// <summary>
        /// 公司主键
        /// </summary>
        public string F_CompanyId { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 部门代码
        /// </summary>
        public string F_EnCode { get; set; }
        /// <summary>
        /// 部门名称
        /// </summary>
        public string F_FullName { get; set; }
        /// <summary>
        /// 部门简称
        /// </summary>
        public string F_ShortName { get; set; }
        /// <summary>
        /// 部门类型
        /// </summary>
        public string F_Nature { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string F_Manager { get; set; }
        /// <summary>
        /// 外线电话
        /// </summary>
        public string F_OuterPhone { get; set; }
        /// <summary>
        /// 内线电话
        /// </summary>
        public string F_InnerPhone { get; set; }
        /// <summary>
        /// 电子邮件
        /// </summary>
        public string F_Email { get; set; }
        /// <summary>
        /// 部门传真
        /// </summary>
        public string F_Fax { get; set; }
        /// <summary>
        /// 排序码
        /// </summary>
        public int? F_SortCode { get; set; }
        /// <summary>
        /// 删除标记
        /// </summary>
        public int? F_DeleteMark { get; set; }
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

        #region 第三方对接扩展
        /// <summary>
        /// 机构类型(0院本部，1厂所，2部门，3处室)--ADD BY SSY 20230317
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 父级名称--ADD BY SSY 20230317
        /// </summary>
        public string F_ParentName { get; set; }
        /// <summary>
        /// 人事-行政正职领导-单选--ADD BY SSY 20230315
        /// </summary>
        public string F_ManagerId { get; set; }
        /// <summary>
        /// 人事党委正职领导(党委书记)-单选--ADD BY SSY 20230315
        /// </summary>
        public string F_PartyManagerId { get; set; }
        /// <summary>
        /// 人事-部门领导(含正职)-多选--ADD BY SSY 20230315
        /// </summary>
        public string F_ManagerIds { get; set; }
        /// <summary>
        /// 人事-上级分管领导-多选--ADD BY SSY 20230315
        /// </summary>
        public string F_LeaderIds { get; set; }
        /// <summary>
        /// 体系熟悉
        /// </summary>
        public string F_OrdId { get; set; }
        #endregion

        #region 扩展
        /// <summary>
        /// 人事-部门类型-字典
        /// </summary>
        public string F_DeptTag { get; set; }
        /// <summary>
        /// 所属基地
        /// </summary>
        public string F_Base { get; set; }
        /// <summary>
        /// 所属事业线
        /// </summary>
        public string F_BusinessLine { get; set; }
        /// <summary>
        /// 部门属性
        /// </summary>
        public string F_Attribute { get; set; }
        /// <summary>
        /// 机构体系归属
        /// </summary>
        public string F_System { get; set; }
        
        /// <summary>
        /// 机构类型
        /// </summary>
        public string f_OrganType{ get; set; }
        /// <summary>
        /// 机构子类
        /// </summary>
        public string f_OrganChild{ get; set; }
        /// <summary>
        /// 机构等级
        /// </summary>
        public string f_OrganGrade{ get; set; }
        /// <summary>
        /// 机构级别
        /// </summary>
        public string f_OrganLevel{ get; set; }
        
        /// <summary>
        /// 层级编码
        /// </summary>
        public string F_DeepNum { get; set; }
        #endregion

        #region
        /// <summary>
        /// 同步后的钉钉部门ID
        /// </summary>
        public string F_DingDepId { get; set; }
        /// <summary>
        /// 同步后的企业微信部门ID
        /// </summary>
        public string F_WechatDepId { get; set; }
        /// <summary>
        /// 企业微信同步状态 -1未知 0未同步 1已同步 2绑定失效 3名称不一致
        /// </summary>
        [Column(IsIgnore = true)]
        public int F_WechatSyn { get; set; }
        #endregion
        
        /// <summary>
        /// 部门角色数据
        /// </summary>
        [Column(IsIgnore = true)]
        public string DeptRoleList { get; set; }

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        #endregion
    }
}
