using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：公司管理
    /// </summary>
    public interface CompanyIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取公司列表数据
        /// </summary>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        Task<IEnumerable<CompanyEntity>> GetList(string keyWord = "");
        /// <summary>
        /// 获取子公司列表信息
        /// </summary>
        /// <param name="pId">父级Id</param>
        /// <returns></returns>
        Task<IEnumerable<CompanyEntity>> GetListByPId(string pId);
        /// <summary>
        /// 获取公司信息实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<CompanyEntity> GetEntity(string keyValue);
        /// <summary>
        /// 获取树形数据
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <returns></returns>
        Task<IEnumerable<TreeModel>> GetTree(string parentId);
        /// <summary>
        /// 获取公司本身和子公司的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetSubNodes(string parentId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公司信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存公司信息（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="companyEntity">公司实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, CompanyEntity companyEntity);
        /// <summary>
        /// 保存公司对应的企业微信部门ID
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="wechatDepId">企业微信部门ID</param>
        /// <returns></returns>
        Task SaveWechatDepId(string companyId, string wechatDepId);
        #endregion

        /// <summary>
        /// 给老数据添加层级编码
        /// </summary>
        Task InitDeepNum();
    }
}
