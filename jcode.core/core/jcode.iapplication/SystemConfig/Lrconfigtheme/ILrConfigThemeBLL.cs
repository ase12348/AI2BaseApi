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
    /// 日 期： 2023-10-10 09:59:11
    /// 描 述： 个性化主题配置
    /// </summary>
    public interface ILrConfigThemeBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取个性化主题配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrConfigThemeEntity>> GetList(LrConfigThemeEntity queryParams);
        /// <summary>
        /// 获取个性化主题配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrConfigThemeEntity>> GetPageList(Pagination pagination, LrConfigThemeEntity queryParams);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrConfigThemeEntity> GetEntity(string keyValue);
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
        Task SaveEntity(string keyValue, LrConfigThemeEntity entity);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        Task<LrConfigThemeEntity> GetTheme();
        /// <summary>
        /// 保存当前登录人员的个性化主题（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        Task SaveTheme(LrConfigThemeEntity entity);
        /// <summary>
        /// 清除当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        Task DeleteTheme();
        #endregion
    }
}