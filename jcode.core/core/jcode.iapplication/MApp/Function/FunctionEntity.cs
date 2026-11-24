using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：移动端功能管理
    /// </summary>
    [MyTable("lr_app_function")]
    public class FunctionEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        /// <returns></returns>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 功能分类
        /// </summary>
        /// <returns></returns>
        public string F_Type { get; set; }
        /// <summary>
        /// 绑定表单
        /// </summary>
        /// <returns></returns>
        public string F_FormCode { get; set; }
        /// <summary>
        /// 表单版本
        /// </summary>
        /// <returns></returns>
        public string F_FormVerison { get; set; }

        /// <summary>
        /// 图标
        /// </summary>
        /// <returns></returns>
        public string F_Icon { get; set; }
        /// <summary>
        /// 图标颜色
        /// </summary>
        public string F_Color { get; set; }
        /// <summary>
        /// 功能名称
        /// </summary>
        public string F_Name { get; set; }
        /// <summary>
        /// 模板ID
        /// </summary>
        public string F_SchemeId { get; set; }
        /// <summary>
        ///  1 启用 0 停用
        /// </summary>
        public int? F_EnabledMark { get; set; }
        /// <summary>
        ///  排序
        /// </summary>
        public int? F_SortCode { get; set; }


        /// <summary>
        /// 是否是代码开发的1是2不是（自定义表单功能）3 4 桌面设计
        /// </summary>
        /// <returns></returns>
        public int? F_IsSystem { get; set; }

        /// <summary>
        /// 功能地址
        /// </summary>
        /// <returns></returns>
        public string F_Url { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_CreateDate { get; set; }
        /// <summary>
        /// 创建人ID
        /// </summary>
        /// <returns></returns>
        public string F_CreateUserId { get; set; }
        /// <summary>
        /// 创建人姓名
        /// </summary>
        /// <returns></returns>
        public string F_CreateUserName { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        /// <returns></returns>
        public DateTime? F_ModifyDate { get; set; }
        /// <summary>
        /// 修改人ID
        /// </summary>
        /// <returns></returns>
        public string F_ModifyUserId { get; set; }
        /// <summary>
        /// 修改人名字
        /// </summary>
        /// <returns></returns>
        public string F_ModifyUserName { get; set; }
        
        /// <summary>
        /// 应用编码
        /// </summary>
        public string F_AppCode{ get; set; }

        /// <summary>
        /// 模板名称
        /// </summary>
        [Column(IsIgnore = true)]
        public string Scheme { get; set; }

        
       
        #endregion

        #region 多租户
        /// <summary>
        /// 租户ID
        /// </summary>
        public string F_TenantId { get; set; }
        /// <summary>
        /// 是否可编辑 0 表示不可编辑 其它值都可编辑
        /// </summary>
        public int? F_IsEdit { get; set; }
        /// <summary>
        /// 租户同步id
        /// </summary>
        public string F_OldId { get; set; }
        #endregion
    }
}