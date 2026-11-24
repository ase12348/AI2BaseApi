using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：功能模块
    /// </summary>
    public interface ModuleIBLL : IBLL
    {
        #region 功能模块
        /// <summary>
        /// 获取所有模块数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ModuleEntity>> GetModuleList();
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        Task<ModuleEntity> GetModuleByUrl(string url);
        /// <summary>
        ///  功能列表(code)
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<ModuleEntity> GetEntityByCode(string code);
        /// <summary>
        /// 功能实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task<ModuleEntity> GetModuleEntity(string keyValue);

        /// <summary>
        /// 根据父级主键获取数据
        /// </summary>
        /// <param name="parentId">父级主键</param>
        /// <returns></returns>
        Task<IEnumerable<ModuleEntity>> GetModuleListByParentId(string parentId);
        #endregion


        #region 提交数据
        /// <summary>
        /// 删除模块功能
        /// </summary>
        /// <param name="keyValue">主键值</param>
        Task<bool> Delete(string keyValue);

        /// <summary>
        /// 保存模块功能实体（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moduleEntity">实体</param>
        Task<string> SaveEntity(string keyValue, ModuleEntity moduleEntity);
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="state"></param>
        /// <returns></returns>
        Task UpdateState(string keyValue, int state);
        #endregion

    }
}
