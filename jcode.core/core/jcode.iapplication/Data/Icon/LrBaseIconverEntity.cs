using jcode.orm;
using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座-单位管理
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期：2024-03-01 10:33:52
    /// 描 述：lr_base_iconver(图标库版本)表的实体
    /// </summary>
    [MyTable("lr_base_iconver")]
    public class LrBaseIconverEntity
    {
        #region 实体成员
        /// <summary>
        /// 主键
        /// </summary>
        [Column(IsPrimary = true,ColumnName = "F_Id")]
        public string F_Id { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        [Column(ColumnName = "F_Ver")]
        public string F_Ver { get; set; }

        #endregion

        #region 扩展属性
        
        #endregion
    }
}