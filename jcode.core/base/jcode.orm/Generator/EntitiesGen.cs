using System.Collections.Generic;

namespace jcode.orm
{
    /// <summary>
    /// 生成实体结构
    /// </summary>
    public class EntitiesGen
    {
        /// <summary>
        /// 构造方法
        /// </summary>
        public EntitiesGen()
        {
            NameSpace = "DefaultModels";
        }
        /// <summary>
        /// 命名空间
        /// </summary>
        public string NameSpace { get; set; }
        /// <summary>
        /// 类名
        /// </summary>
        public string ClassName { get; set; }
        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 备注 
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 新的主键名
        /// </summary>
        public string NewPkName { get; set; }

        /// <summary>
        /// 列集合
        /// </summary>
        public List<PropertyGen> PropertyGens { get; set; }

    }
}
