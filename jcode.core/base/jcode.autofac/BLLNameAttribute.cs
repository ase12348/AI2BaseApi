using System;

namespace jcode.autofac
{
    /// <summary>
    /// 定义bll名称，用于对同一接口的不同实现
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BLLNameAttribute : Attribute
    {
        /// <summary>
        /// 名字
        /// </summary>
        public string BLLName { get; set; }
        /// <summary>
        /// 构造方法
        /// </summary>
        public BLLNameAttribute()
        {

        }
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="bLLName">业务模块名字</param>
        public BLLNameAttribute(string bLLName)
        {
            BLLName = bLLName;
        }
    }
}
