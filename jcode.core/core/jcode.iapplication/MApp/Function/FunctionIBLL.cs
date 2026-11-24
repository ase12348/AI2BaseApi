using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：移动端功能管理
    /// </summary>
    public interface FunctionIBLL:IBLL
    {

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FunctionEntity>> GetList();
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<FunctionEntity> GetEntity(string keyValue);
        /// <summary>
        /// 获取移动功能模板
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<FunctionSchemeEntity> GetScheme(string keyValue);


        /// <summary>
        /// 获取移动功能子集项
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<IEnumerable<ModuleItemEntity>> GetModuleItemList(string keyValue);


        /// <summary>
        /// 获取移动功能所有子集
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ModuleItemEntity>> GetModuleItemList();
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="functionEntity">功能信息</param>
        /// <param name="moduleItemEntitys">功能子集信息</param>
        Task SaveEntity(string keyValue, FunctionEntity functionEntity, List<ModuleItemEntity> moduleItemEntitys);
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">模板信息主键</param>
        /// <param name="state">状态1启用0禁用</param>
        Task UpdateState(string keyValue, int state);
        #endregion
    }
}
