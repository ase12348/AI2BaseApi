using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace jcode.iapplication
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-10-10 09:51:29
    /// 描 述： 数据列表配置-高级查询、表格列
    /// </summary>
    public interface ILrConfigTableBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrConfigTableEntity>> GetList(LrConfigTableEntity queryParams);
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrConfigTableEntity>> GetPageList(Pagination pagination, LrConfigTableEntity queryParams);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrConfigTableEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task Delete(string keyValue);
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        Task Deletes(string keyValues);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, LrConfigTableEntity entity);
        #endregion

        #region 扩展方法

        #region 表格列
        /// <summary>
        /// 获取当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        Task<LrConfigTableEntity> GetThCol(string objectId);
        /// <summary>
        /// 保存当前登录人员的个性化表格列（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        Task SaveThCol(LrConfigTableEntity entity);
        /// <summary>
        /// 清除当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        Task DeleteThCol(string objectId);
        #endregion

        #region 高级查询

        #endregion
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        Task<IEnumerable<LrConfigTableEntity>> GetQueryViews(string objectId);
        /// <summary>
        /// 删除查询视图
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteQueryView(string keyValue);
        /// <summary>
        /// 保存数据(新增、编辑)
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveQueryView(string keyValue, LrConfigTableEntity entity);

        #endregion
    }
}