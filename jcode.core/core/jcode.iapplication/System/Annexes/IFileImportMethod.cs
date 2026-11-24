using jcode.autofac;
using System.Data;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2023.02.14
    /// 描 述：通用的文件导入接口
    /// </summary>
    public interface IFileImportMethod : IBLL
    {
        /// <summary>
        /// 导入执行方法
        /// </summary>
        /// <param name="data">文件数据</param>
        /// <returns></returns>
        Task<bool> Execute(byte[] data);
    }
}