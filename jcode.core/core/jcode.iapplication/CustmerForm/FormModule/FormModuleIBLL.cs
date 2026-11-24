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
    /// 日 期：2021.05.07
    /// 描 述：表单关联功能
    /// </summary>
    public interface FormModuleIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<FormModuleEntity2>> GetPageList(Pagination pagination, FormModuleEntity2 queryParams);
        /// <summary>
        /// 获取列表（发布表单功能）
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FormModuleEntity2>> GetList();
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<FormModuleEntity> GetEntity(string keyValue);

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="code">主键</param>
        /// <returns></returns>
        Task<FormModuleEntity> GetEntityByCode(string code);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="moduleId">菜单主键</param>
        /// <returns></returns>
        Task<FormModuleEntity> GetEntityByModuleId(string moduleId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除模板信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <param name="moduleEntity">模块实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, FormModuleEntity entity, ModuleEntity moduleEntity);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task UpdateState(string keyValue, int state);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task UpdateStateByModuleId(string keyValue, int state);

        /// <summary>
        /// 虚拟删除模板信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteEntityByModuleId(string keyValue);
        #endregion
    }
}
