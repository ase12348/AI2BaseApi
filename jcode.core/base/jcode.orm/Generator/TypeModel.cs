using System;

namespace jcode.orm
{
    public class TypeModel
    {
        /// <summary>
        /// 是否有大的数据
        /// </summary>
        public bool IsBigData { get; set; }
        /// <summary>
        /// 二进制列名
        /// </summary>
        public string ByteColName { get; set; }
        /// <summary>
        /// 实体类型
        /// </summary>
        public Type ClassType { get; set; }
    }
}