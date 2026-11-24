using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：角色管理
    /// </summary>
    public class RoleService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取角色数据列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<RoleEntity>> GetList()
        {
            return this.BaseRepository().FindList<RoleEntity>(t => t.F_DeleteMark == 0, "F_ModifyDate DESC,F_CreateDate DESC,F_FullName");
        }
        /// <summary>
        /// 获取角色数据列表（分页）
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<RoleEntity>> GetPageList(Pagination pagination, string keyword)
        {
            var exp = Expressionable.Create<RoleEntity>()
               .And(t => t.F_DeleteMark == 0)
               .And(t => t.F_EnabledMark == 1)
               .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_FullName.Contains(keyword) || t.F_EnCode.Contains(keyword))
               .ToExpression();

            return this.BaseRepository().FindList(exp, pagination);
        }
        /// <summary>
        /// 获取角色数据列表
        /// </summary>
        /// <param name="roleIds">主键串</param>
        /// <returns></returns>
        public Task<IEnumerable<RoleEntity>> GetListByRoleIds(string roleIds)
        {
            var roles = roleIds.Split(",");
            return this.BaseRepository().FindList<RoleEntity>(t=>t.F_EnabledMark == 1 && t.F_DeleteMark == 0 && roles.Contains(t.F_RoleId));
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<RoleEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<RoleEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<RoleEntity>(keyValue);
                await db.ExecuteSql(" Delete  From LR_BASE_USERRELATION where F_OBJECTID = @keyValue  ", new { keyValue });

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存角色（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="roleEntity">角色实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, RoleEntity roleEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                roleEntity.F_ModifyUserId = this.GetUserId();
                roleEntity.F_ModifyUserName = this.GetUserName();
                roleEntity.F_RoleId = keyValue;
                roleEntity.F_ModifyDate = DateTime.Now;

                await this.BaseRepository().Update(roleEntity);
            }
            else
            {
                roleEntity.F_CreateUserId = this.GetUserId();
                roleEntity.F_CreateUserName = this.GetUserName();

                roleEntity.F_RoleId = Guid.NewGuid().ToString();
                roleEntity.F_CreateDate = DateTime.Now;
                roleEntity.F_DeleteMark = 0;
                roleEntity.F_EnabledMark = 1;
                //roleEntity.F_Category = "1";
                await this.BaseRepository().Insert(roleEntity);
            }
        }
        #endregion
    }
}
