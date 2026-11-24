using jcode.util;
using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq.Expressions;
namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-05-07 11:16:00
    /// 描 述： 部门角色数据库执行类
    /// </summary>
    public class LrBaseDeptRoleService: ServiceBase {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        private Expression<Func<LrBaseDeptRoleEntity, bool>>GetExpression(LrBaseDeptRoleEntity queryParams) {
            var exp = Expressionable.Create<LrBaseDeptRoleEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FCode), t => t.FCode.Contains(queryParams.FCode));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FName), t => t.FName.Contains(queryParams.FName));
            if (queryParams.FEnabledMark != null) {
                exp = exp.And(t => t.FEnabledMark == queryParams.FEnabledMark);
            }
            if (queryParams.FSortCode != null) {
                exp = exp.And(t => t.FSortCode == queryParams.FSortCode);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FDes), t => t.FDes.Contains(queryParams.FDes));

            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FCreateUserId), t => t.FCreateUserId.Contains(queryParams.FCreateUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FCreateUserName), t => t.FCreateUserName.Contains(queryParams.FCreateUserName));

            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FModifyUserId), t => t.FModifyUserId.Contains(queryParams.FModifyUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FModifyUserName), t => t.FModifyUserName.Contains(queryParams.FModifyUserName));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.FTenantId), t => t.FTenantId.Contains(queryParams.FTenantId));
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取部门角色的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseDeptRoleEntity>>GetList() {
            return this.BaseRepository().FindAll<LrBaseDeptRoleEntity>("F_SortCode ASC");
        }
        /// <summary>
        /// 获取部门角色的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseDeptRoleEntity>>GetPageList(Pagination pagination, LrBaseDeptRoleEntity queryParams) {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrBaseDeptRoleEntity>(expression, pagination);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseDeptRoleEntity>GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<LrBaseDeptRoleEntity>(keyValue);
        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await this.BaseRepository().Delete<LrBaseDeptRoleEntity>(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues) {
            string[] keyValuesArr = null;
            string[] arr = keyValues.Split(",");
            keyValuesArr = arr;
            await this.BaseRepository().Delete<LrBaseDeptRoleEntity>(keyValuesArr);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseDeptRoleEntity entity) {
            entity.FModifyDate = DateTime.Now;
            entity.FModifyUserId = this.GetUserId();
            entity.FModifyUserName = this.GetUserName();
            
            if (string.IsNullOrEmpty(keyValue)) {
                if (string.IsNullOrEmpty(entity.FId)) {
                    entity.FId = Guid.NewGuid().ToString();
                }
                entity.FCreateDate = DateTime.Now;
                entity.FCreateUserId = this.GetUserId();
                entity.FCreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            } else {
                entity.FId = keyValue;
             
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion
    }
}