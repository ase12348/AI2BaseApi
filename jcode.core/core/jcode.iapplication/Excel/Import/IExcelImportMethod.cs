using jcode.autofac;
using jcode.office;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.03.31
    /// 描 述：通用的接口导入接口
    /// </summary>
    public interface IExcelImportMethod : IBLL
    {
        /// <summary>
        /// 导入执行方法
        /// </summary>
        /// <param name="dt">excel数据</param>
        /// <returns></returns>
        Task<bool> Execute(DataTable dt);
    }

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.03.31
    /// 描 述：通用的接口导入接口
    /// </summary>
    public interface IExcelImportMethod2 : IBLL
    {
        /// <summary>
        /// 导入执行方法
        /// </summary>
        /// <param name="sheets">excel数据</param>
        /// <returns></returns>
        Task<bool> Execute(List<ExcelSheet> sheets);
    }
}
