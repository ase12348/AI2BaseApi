using System;
using System.Collections.Generic;
using System.Text;

namespace jcode.iapplication.SystemConfig.Lrconfigtable
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-10-27
    /// 描 述：高级查询
    /// </summary>
    public class QueryModel
    {
        /// <summary>
        /// 高级查询主键
        /// </summary>
        public string F_Id { get; set; }
        /// <summary>
        /// 功能Id
        /// </summary>
        public string F_ObjectId { get; set; }
        /// <summary>
        /// 配置内容
        /// </summary>
        public List<Content> Content { get; set; }
    }
    /// <summary>
    /// 配置内容
    /// </summary>
    public class Content
    {
        /// <summary>
        /// 字段名称
        /// </summary>
        public string ColName { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string ColValue { get; set; }
        /// <summary>
        /// 字段类型
        /// </summary>
        public string ColType { get; set; }
    }
}
