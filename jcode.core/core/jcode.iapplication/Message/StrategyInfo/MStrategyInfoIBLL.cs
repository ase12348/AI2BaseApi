using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：消息策略
    /// </summary>
    public interface MStrategyInfoIBLL : IBLL
    {
        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<MStrategyInfoEntity>> GetList();
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<MStrategyInfoEntity>> GetPageList(Pagination pagination, string keyword);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<MStrategyInfoEntity> GetEntity(string keyValue);
        /// <summary>
        /// 根据策略编码获取策略
        /// </summary>
        /// <param name="code">策略编码</param>
        /// <returns></returns>
        Task<MStrategyInfoEntity> GetEntityByCode(string code);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, MStrategyInfoEntity entity);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 消息处理，在此处处理好数据，然后调用消息发送方法
        /// </summary>
        /// <param name="code">消息策略编码</param>
        /// <param name="content">消息内容</param>
        /// <param name="userlist">用户列表信息</param>
        /// <returns></returns>
        Task<ResponseDto> SendMessage(string code, string content, string userlist);
        #endregion
    }
}
