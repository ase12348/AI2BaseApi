using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.09.21
    /// 描 述：智能体管理 
    /// </summary>
    public interface AgentIBLL : IBLL
    {
        #region 获取数据 
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        Task<IEnumerable<AgentEntity>> GetList(string keyword);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<AgentEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        Task DeleteEntity(string id);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        Task SaveEntity(string id, AgentEntity entity);
        /// <summary>
        /// 复制智能体
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>新实体主键</returns>
        Task<string> CopyEntity(string id);
        #endregion
    }
}
