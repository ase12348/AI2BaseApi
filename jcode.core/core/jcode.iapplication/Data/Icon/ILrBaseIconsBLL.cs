using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;
namespace jcode.iapplication {
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-03-01 10:15:24
    /// 描 述： 图标管理
    /// </summary>
    public interface ILrBaseIconsBLL: IBLL {
        #region 获取数据
        /// <summary>
        /// 获取图标管理的列表数据
        /// </summary>
        /// <param name="ver">版本号</param>
        /// <returns></returns>
        Task<IconDto>GetALLList(string ver);
        /// <summary>
        /// 获取图标管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrBaseIconsEntity>>GetPageList(Pagination pagination, LrBaseIconsEntity queryParams);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrBaseIconsEntity>GetEntity(string keyValue);
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
        Task SaveEntity(string keyValue, LrBaseIconsEntity entity);
        #endregion
    }
}