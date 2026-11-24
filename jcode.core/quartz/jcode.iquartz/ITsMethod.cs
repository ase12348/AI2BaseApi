using jcode.autofac;
using System.Threading.Tasks;

namespace jcode.iquartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务调度器执行的方法需要继承的接口
    /// </summary>
    public interface ITsMethod:IBLL
    {
        /// <summary>
        /// 任务调度器执行的方法
        /// </summary>
        Task<string> Execute();
    }
}
