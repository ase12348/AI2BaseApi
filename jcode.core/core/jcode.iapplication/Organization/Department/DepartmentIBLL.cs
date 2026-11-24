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
    /// 描 述：部门管理
    /// </summary>
    public interface DepartmentIBLL : IBLL
    {
        #region 获取数据

        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <param name="parentId">上级部门</param>
        /// <param name="type">部门类型</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        Task<IEnumerable<DepartmentEntity>> GetList(string companyId,string parentId, string type, string keyWord = "");

        /// <summary>
        /// 获取部门列表信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <returns></returns>
        Task<IEnumerable<DepartmentEntity>> GetAllList(string keyWord);
        
        /// <summary>
        /// 获取部门列表信息(根据公司Id 和 父级 id)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="pid">父级 id</param>
        /// <returns></returns>
        Task<IEnumerable<DepartmentEntity>> GetListByPid(string companyId, string pid);
        /// <summary>
        /// 获取部门列表信息(根据部门ID集合)
        /// </summary>
        /// <param name="keyValues">部门ID集合</param>
        /// <returns></returns>
        Task<IEnumerable<DepartmentEntity>> GetListByKeys(string[] keyValues);
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<DepartmentEntity> GetEntity(string keyValue);
        /// <summary>
        /// 获取部门本身和子部门的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetSubNodes(string parentId);

        /// <summary>
        /// 获取部门本身和父级部门的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetParentNodes(string parentId);
        /// <summary>
        /// 获取部门本身和子部门的id
        /// </summary>
        /// <param name="WechatDeptId">同步后的企微部门ID</param>
        /// <returns></returns>
        Task<DepartmentEntity> GetEntityByWechatDeptId(string WechatDeptId);

        /// <summary>
        /// 获取完整部门链名称
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        Task<string> GetFullName(string id);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存部门信息（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">部门实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, DepartmentEntity departmentEntity);

        /// <summary>
        /// 保存部门信息-批量（新增、修改）----ADD BY SSY 20220929
        /// </summary>
        /// <param name="departmentEntitys">部门实体</param>
        /// <returns></returns>
        Task SaveEntitys(List<DepartmentEntity> departmentEntitys);
        #endregion

        /// <summary>
        /// 给老数据添加层级编码
        /// </summary>
        Task InitDeepNum();
    }
}
