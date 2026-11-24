using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.ilanguage
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言映射
    /// </summary>
    public interface LGMapIBLL : IBLL
    {

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        Task<DataTable> GetPageList(Pagination pagination, string keyword);
        /// <summary>
        /// 获取语言包映射关系数据集合
        /// </summary>
        /// <param name="code">语言包编码</param>
        /// <param name="mainCode">主语言编码</param>
        /// <returns></returns>
        Task<Dictionary<string, string>> GetMap(string code, string mainCode);
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="TypeCode">编码</param>
        /// <returns></returns>
        Task<IEnumerable<LGMapEntity>> GetListByTypeCode(string TypeCode);

        /// <summary>
        /// 获取数据字典翻译数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<DataTable> GetDataItemList(string code);

        /// <summary>
        /// 获取菜单翻译数据
        /// </summary>
        /// <param name="moduleId">模块编码</param>
        /// <param name="isParent">是否是父级 1是 0不是</param>
        /// <returns></returns>
        Task<DataTable> GetModuleList(string moduleId, int isParent);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        Task DeleteEntity(string code);

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="list">翻译集合</param>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        Task SaveEntity(List<LGMapEntity> list, string code);
        #endregion
    }
}
