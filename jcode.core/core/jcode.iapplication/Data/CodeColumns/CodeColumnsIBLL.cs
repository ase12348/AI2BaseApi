using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-09-18 10:13:07
    /// 描 述： CodeColumns
    /// </summary>
    public interface CodeColumnsIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取CodeColumns的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<CodeColumnsEntity>> GetList(CodeColumnsEntity queryParams);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableId">表格主键</param>
        Task Delete(string tableId);
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        Task Add(List<CodeColumnsEntity> list);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        Task Update(List<CodeColumnsEntity> list);
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        Task Deletes(List<CodeColumnsEntity> list);
        #endregion
    }
}
