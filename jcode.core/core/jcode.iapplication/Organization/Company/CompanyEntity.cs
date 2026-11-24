using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：公司管理
    /// </summary>
    [MyTable("lr_base_company")]
    public class CompanyEntity
    {
        #region 实体成员
        /// <summary>
        /// 公司主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_CompanyId { get; set; }
        /// <summary>
        /// 公司分类
        /// </summary>
        public int? F_Category { get; set; }
        /// <summary>
        /// 父级主键
        /// </summary>
        public string F_ParentId { get; set; }
        /// <summary>
        /// 公司代码
        /// </summary>
        public string F_EnCode { get; set; }
        /// <summary>
        /// 公司简称
        /// </summary>
        public string F_ShortName { get; set; }
        /// <summary>
        /// 公司名称
        /// </summary>
        public string F_FullName { get; set; }
        /// <summary>
        /// 外线电话
        /// </summary>
        public string F_OuterPhone { get; set; }
        /// <summary>
        /// 内线电话
        /// </summary>
        public string F_InnerPhone { get; set; }
        /// <summary>
        /// 负责人
        /// </summary>
        public string F_Manager { get; set; }
        /// <summary>
        /// 详细地址
        /// </summary>
        public string F_Address { get; set; }
        /// <summary>
        /// 经营范围
        /// </summary>
        public string F_BusinessScope { get; set; }
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
        /// 人事-部门标签-字典--ADD BY SSY 20230315
        /// </summary>
        public string F_DeptTag { get; set; }
        /// <summary>
        /// 层级编码
        /// </summary>
        public string F_DeepNum { get; set; }
        #endregion

        #region
        /// <summary>
        /// 钉钉应用的唯一标识key
        /// </summary>
        public string F_DingAppKey { get; set; }
        /// <summary>
        /// 钉钉应用的密钥
        /// </summary>
        public string F_DingAppSecret { get; set; }
        /// <summary>
        /// 钉钉钉钉应用ID
        /// </summary>
        public string F_DingAgentId { get; set; }
        /// <summary>
        /// 同步后的企业微信部门ID（根公司为1）
        /// </summary>
        public string F_WechatDepId { get; set; }
        #endregion
        
        /// <summary>
        /// 公司角色信息
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
