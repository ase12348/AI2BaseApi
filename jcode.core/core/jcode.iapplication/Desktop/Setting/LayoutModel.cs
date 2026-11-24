using System.Collections.Generic;

namespace jcode.iapplication
{
    /// <summary>
    /// 桌面配置信息
    /// </summary>
    public class LayoutModel
    {
        /// <summary>
        /// id数据
        /// </summary>
        public string I { get; set; }
        /// <summary>
        /// 配置信息
        /// </summary>
        public ConfigModel Config { get; set; }
    }
    /// <summary>
    /// 配置信息
    /// </summary>
    public class ConfigModel
    {
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }
        /// <summary>
        /// 条件信息
        /// </summary>
        public ConditionInfoModel ConditionInfo { get; set; }
    }


    /// <summary>
    /// 条件信息模型
    /// </summary>
    public class ConditionInfoModel { 
        /// <summary>
        /// 计算公式
        /// </summary>
        public string Formula { get; set; }
        /// <summary>
        /// 条件信息
        /// </summary>
        public List<ConditionModel> Conditions { get; set; }
    }
    /// <summary>
    /// 条件模型
    /// </summary>
    public class ConditionModel {
        /// <summary>
        /// 字段
        /// </summary>
        public string Prop { get; set; }
        /// <summary>
        /// 比较符
        /// </summary>
        public int Symbol { get; set; }
        /// <summary>
        /// 值类型
        /// </summary>
        public int ValueType { get; set; }
        /// <summary>
        /// 字段值
        /// </summary>
        public string FiledValue { get; set; }

        /// <summary>
        /// 类型 glbd关联表单数据类型，需要特殊处理
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 表名称
        /// </summary>
        public string Table { get; set; }
        /// <summary>
        /// 比较字段
        /// </summary>
        public string Cfield { get; set; }
        /// <summary>
        /// 外键
        /// </summary>
        public string Field { get; set; }
        /// <summary>
        /// 关联表字段
        /// </summary>
        public string RelationField { get; set; }

    }
}
