using System.Collections.Generic;

namespace jcode.iapplication.SystemConfig.Lrconfigtable
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期：2023-10-27
    /// 描 述：高级查询
    /// </summary>
    public class QueryConfigModel
    {
        /// <summary>
        /// 快捷查询内容
        /// </summary>
        public List<EasyQueryModel> QueryList { get; set; }
        /// <summary>
        /// 高级查询内容
        /// </summary>
        public List<AdvancedQueryModel> AdvancedQueryList { get; set; }
        /// <summary>
        /// 高级查询公式
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 查询视图名称
        /// </summary>
        public string F_Title { get; set; }
        /// <summary>
        /// 查询视图编码
        /// </summary>
        public string F_Code { get; set; }
    }
    /// <summary>
    /// 快捷查询配置
    /// </summary>
    public class EasyQueryModel
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
    /// <summary>
    /// 高级查询配置
    /// </summary>
    public class AdvancedQueryModel
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Index { get; set; }
        /// <summary>
        /// 字段
        /// </summary>
        public string F_FieldId { get; set; }
        /// <summary>
        /// 比较符1.等于2.大于3.大于等于4.小于5.小于等于6.包含7.包含于8.不等于9.不包含10.不包含于
        /// </summary>
        public string F_Symbol { get; set; }
        /// <summary>
        /// 字段值类型1.文本2.登录者ID3.登录者账号4.登录者公司5.登录者部门6.登录者岗位7.登录者角色
        /// </summary>
        public string F_FiledValueType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string F_FiledValue { get; set; }
    }
}
