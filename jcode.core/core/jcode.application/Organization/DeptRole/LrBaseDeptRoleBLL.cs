using jcode.autofac;
using jcode.util;
using jcode.iapplication;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-05-07 11:16:00
    /// 描 述： 部门角色
    /// </summary>
    public class LrBaseDeptRoleBLL: BLLBase, ILrBaseDeptRoleBLL, BLL {
        private readonly LrBaseDeptRoleService lrBaseDeptRoleService = new LrBaseDeptRoleService();
        private readonly ICache _iCache;
        private readonly string key = "jcode_dept_roles";
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        /// <param name="userRelationIBLL"></param>
        public LrBaseDeptRoleBLL(ICache iCache)
        {
            _iCache = iCache;
        }
        
        #region 获取数据
        /// <summary>
        /// 获取部门角色的列表数据
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LrBaseDeptRoleEntity>>GetList() {
            var list = await _iCache.ReadAsync<IEnumerable<LrBaseDeptRoleEntity>>(key);
            if (list == null)
            {
                list = await lrBaseDeptRoleService.GetList();
                await _iCache.WriteAsync(key,list);
            }

            return list;
        }
        /// <summary>
        /// 获取部门角色的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseDeptRoleEntity>>GetPageList(Pagination pagination, LrBaseDeptRoleEntity queryParams) {
            return lrBaseDeptRoleService.GetPageList(pagination, queryParams);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseDeptRoleEntity>GetEntity(string keyValue) {
            return lrBaseDeptRoleService.GetEntity(keyValue);
        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await lrBaseDeptRoleService.Delete(keyValue);
            _iCache.Remove(key);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues) {
            await lrBaseDeptRoleService.Deletes(keyValues);
            _iCache.Remove(key);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseDeptRoleEntity entity) {
            await lrBaseDeptRoleService.SaveEntity(keyValue, entity);
            _iCache.Remove(key);
        }
        #endregion
    }
}