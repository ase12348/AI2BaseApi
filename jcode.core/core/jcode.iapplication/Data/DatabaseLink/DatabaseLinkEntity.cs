using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据库连接
    /// </summary>
    [MyTable("lr_base_databaselink")]
    public class DatabaseLinkEntity
    {
        #region 实体成员
        /// <summary>
        /// 数据库连接主键
        /// </summary>
        [Column(IsPrimary = true)]
        public string F_Id { get; set; }
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string F_ServerAddress { get; set; }
        /// <summary>
        /// 数据库编码
        /// </summary>
        public string F_DBName { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string F_DBAlias { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string F_DbType { get; set; }
        /// <summary>
        /// 连接地址
        /// </summary>
        public string F_DbConnection { get; set; }
        /// <summary>
        /// 连接地址是否加密
        /// </summary>
        public int? F_DESEncrypt { get; set; }
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

        //数据库连接优化
        /// <summary>
        /// 数据库连接类型(0普通，1高级)----ADD BY SSY 20230306
        /// </summary>
        public int? F_Type { get; set; }
        /// <summary>
        /// 主机
        /// </summary>
        public string F_Server { get; set; }
        /// <summary>
        /// 端口
        /// </summary>
        public string F_Port { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string F_UserId { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string F_Password { get; set; }
        /// <summary>
        /// 初始数据库
        /// </summary>
        public string F_DataBase { get; set; }
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
        #endregion
    }
}
