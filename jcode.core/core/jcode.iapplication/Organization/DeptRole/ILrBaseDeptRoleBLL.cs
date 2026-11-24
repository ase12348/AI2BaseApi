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
    /// 日 期： 2024-05-07 11:16:00
    /// 描 述： 部门角色
    /// </summary>
    public interface ILrBaseDeptRoleBLL: IBLL {
        #region 获取数据
        /// <summary>
        /// 获取部门角色的列表数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<LrBaseDeptRoleEntity>>GetList();
        /// <summary>
        /// 获取部门角色的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrBaseDeptRoleEntity>>GetPageList(Pagination pagination, LrBaseDeptRoleEntity queryParams);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrBaseDeptRoleEntity>GetEntity(string keyValue);
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
        Task SaveEntity(string keyValue, LrBaseDeptRoleEntity entity);
        #endregion
    }
}