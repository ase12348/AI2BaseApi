using System;

namespace jcode.operat
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class LogRecordAttribute : Attribute
    {
        /// <summary>记录名称</summary>
        public string Name { get; set; }

        /// <summary>构造方法</summary>
        public LogRecordAttribute()
        {
        }

        /// <summary>构造方法</summary>
        /// <param name="name">记录名称</param>
        public LogRecordAttribute(string name) => this.Name = name;
    }
}