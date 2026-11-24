using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：岗位管理
    /// </summary>
    public class PostService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetList(string companyId)
        {
            return this.BaseRepository().FindList<PostEntity>(t=>t.F_DeleteMark == 0 && t.F_CompanyId == companyId, "F_DepartmentId,F_ParentId,F_EnCode");
        }

        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门Id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetList(string companyId, string departmentId, string keyword)
        {
            var exp = Expressionable.Create<PostEntity>()
                .And(t => t.F_DeleteMark == 0)
                .AndIF(!string.IsNullOrEmpty(companyId),t => t.F_CompanyId == companyId)
                .AndIF(!string.IsNullOrEmpty(departmentId), t => t.F_DepartmentId == departmentId)
                .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Name.Contains(keyword))
                .ToExpression();
            return this.BaseRepository().FindList(exp, "F_DepartmentId,F_ParentId,F_EnCode");
        }
        /// <summary>
        /// 获取岗位数据列表(根据主键串)
        /// </summary>
        /// <param name="postIds">根据主键串</param>
        /// <returns></returns>
        public Task<IEnumerable<PostEntity>> GetListByPostIds(string postIds)
        {
            return this.BaseRepository().FindListByQueryable<PostEntity>(q =>
            {
                var queryable = q.LeftJoin<DepartmentEntity>((t, t1) => t.F_DepartmentId == t1.F_DepartmentId)
                    .LeftJoin<CompanyEntity>((t,t1, t2) => t.F_CompanyId == t2.F_CompanyId);
                
                var expression = Expressionable.Create<PostEntity>()
                    .And((t) => t.F_DeleteMark == 0 && postIds.Contains(t.F_PostId)).ToExpression();
                queryable = queryable.Where(expression);
                return queryable.Select((t, t1,t2) => new PostEntity
                {
                    F_PostId = t.F_PostId,
                    F_CompanyId = t.F_CompanyId,
                    F_Description = t.F_Description,
                    F_DepartmentId = t.F_DepartmentId,
                    F_DeleteMark = t.F_DeleteMark,
                    F_CreateUserId = t.F_CreateUserId,
                    F_Name = t.F_Name,
                    F_CreateDate = t.F_CreateDate,
                    F_ModifyDate = t.F_ModifyDate,
                    F_ParentId = t.F_ParentId,
                    F_ModifyUserId = t.F_ModifyUserId,
                    F_CreateUserName = t.F_CreateUserName,
                    F_ModifyUserName = t.F_ModifyUserName,
                    F_TenantId = t.F_TenantId,
                    F_EnCode = t.F_EnCode,
                    DeptName = t1.F_FullName,
                    CompanyName = t2.F_FullName
                    
                });
            });
        }
        /// <summary>
        /// 获取岗位的实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<PostEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<PostEntity>(keyValue);
        }
        /// <summary>
        /// 获取下级岗位id集合
        /// </summary>
        /// <param name="parentIds">父级Id集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetIdList(List<string> parentIds)
        {
            List<string> res = new List<string>();
            var list = await this.BaseRepository().FindList<PostEntity>(t=>t.F_DeleteMark == 0 && parentIds.Contains(t.F_ParentId));
            foreach (var item in list)
            {
                res.Add(item.F_PostId);
            }
            return res;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<PostEntity>(keyValue);
                await db.Delete<UserRelationEntity>(t => t.F_ObjectId == keyValue);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存岗位（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="postEntity">岗位实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, PostEntity postEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                postEntity.F_PostId = keyValue;
                postEntity.F_ModifyDate = DateTime.Now;
                postEntity.F_ModifyUserId = this.GetUserId();
                postEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(postEntity);
            }
            else
            {
                postEntity.F_PostId = Guid.NewGuid().ToString();
                postEntity.F_CreateDate = DateTime.Now;
                postEntity.F_DeleteMark = 0;
                postEntity.F_CreateUserId = this.GetUserId();
                postEntity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(postEntity);
            }
        }
        #endregion
    }
}
