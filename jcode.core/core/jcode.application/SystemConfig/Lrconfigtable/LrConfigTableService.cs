using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-10-10 09:51:29
    /// 描 述： 数据列表配置-高级查询、表格列数据库执行类
    /// </summary>
    public class LrConfigTableService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        private Expression<Func<LrConfigTableEntity, bool>> GetExpression(LrConfigTableEntity queryParams)
        {
            var exp = Expressionable.Create<LrConfigTableEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Code), t => t.F_Title.Contains(queryParams.F_Code));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Title), t => t.F_Title.Contains(queryParams.F_Title));
            if (queryParams.F_Type != null)
            {
                exp = exp.And(t => t.F_Type == queryParams.F_Type);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ObjectId), t => t.F_ObjectId.Contains(queryParams.F_ObjectId));
            if (queryParams.F_IsPublic != null)
            {
                exp = exp.And(t => t.F_IsPublic == queryParams.F_IsPublic);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Content), t => t.F_Content.Contains(queryParams.F_Content));
            if (queryParams.F_State != null)
            {
                exp = exp.And(t => t.F_State == queryParams.F_State);
            }
            if (queryParams.F_DeleteMark != null)
            {
                exp = exp.And(t => t.F_DeleteMark == queryParams.F_DeleteMark);
            }
            if (!string.IsNullOrEmpty(queryParams.F_CreateDateQRange))
            {
                var f_CreateDate_list = queryParams.F_CreateDateQRange.Split(" - ");
                DateTime f_CreateDate = Convert.ToDateTime(f_CreateDate_list[0]);
                DateTime f_CreateDate_end = Convert.ToDateTime(f_CreateDate_list[1]);
                exp = exp.And(t => t.F_CreateDate >= f_CreateDate && t.F_CreateDate <= f_CreateDate_end);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserId), t => t.F_CreateUserId.Contains(queryParams.F_CreateUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserName), t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            if (!string.IsNullOrEmpty(queryParams.F_ModifyDateQRange))
            {
                var f_ModifyDate_list = queryParams.F_ModifyDateQRange.Split(" - ");
                DateTime f_ModifyDate = Convert.ToDateTime(f_ModifyDate_list[0]);
                DateTime f_ModifyDate_end = Convert.ToDateTime(f_ModifyDate_list[1]);
                exp = exp.And(t => t.F_ModifyDate >= f_ModifyDate && t.F_ModifyDate <= f_ModifyDate_end);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ModifyUserId), t => t.F_ModifyUserId.Contains(queryParams.F_ModifyUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ModifyUserName), t => t.F_ModifyUserName.Contains(queryParams.F_ModifyUserName));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_TenantId), t => t.F_TenantId.Contains(queryParams.F_TenantId));
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigTableEntity>> GetList(LrConfigTableEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrConfigTableEntity>(expression);
        }
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigTableEntity>> GetPageList(Pagination pagination, LrConfigTableEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrConfigTableEntity>(expression, pagination);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrConfigTableEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<LrConfigTableEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await this.BaseRepository().Delete<LrConfigTableEntity>(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            string[] keyValuesArr = null;
            string[] arr = keyValues.Split(",");
            keyValuesArr = arr;
            await this.BaseRepository().Delete<LrConfigTableEntity>(keyValuesArr);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrConfigTableEntity entity)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(entity.F_Id))
                {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                entity.F_State = 0;
                entity.F_DeleteMark = 0;
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(entity, true);
            }
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取当前登录人员的个性化表格列
        /// </summary>
        /// <param name="userEntity">当前登录者用户信息</param>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        public async Task<LrConfigTableEntity> GetThCol1(UserEntity userEntity, string objectId)
        {
            var list = (List<LrConfigTableEntity>)(await this.BaseRepository().FindList<LrConfigTableEntity>(it => it.F_DeleteMark == 0 && it.F_State == 0 && it.F_Type == 2 && it.F_CreateUserId == userEntity.F_UserId&&it.F_ObjectId== objectId));
            if (list.Count > 0)
            {
                return list[0];
            }
            return null;
        }
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表[自己创建+公开创建]
        /// </summary>
        /// <param name="userEntity">当前登录者用户信息</param>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<LrConfigTableEntity>> GetQueryViews1(UserEntity userEntity, string objectId)
        {
            return (List<LrConfigTableEntity>)(await this.BaseRepository().FindList<LrConfigTableEntity>(it => it.F_DeleteMark == 0
            && it.F_State == 0
            && it.F_Type == 1
            && it.F_ObjectId == objectId
            && it.F_CreateUserId == userEntity.F_UserId
            || it.F_IsPublic == 1
            ));
        }


        /// <summary>
        /// 获取当前人员满足条件的查询表格查询配置
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<LrConfigTableEntity>> GetTableConfig(LrConfigTableEntity queryParams)
        {
            var exp = Expressionable.Create<LrConfigTableEntity>();
            exp = exp.And(t => t.F_DeleteMark == 0);//删除标记(0正常，1删除)
            exp = exp.And(t => t.F_State == 0);//使用状态(默认使用0，未使用1)
            if (queryParams.F_Type != null)//类型(查询视图1，表格显示列2)
            {
                exp = exp.And(t => t.F_Type == queryParams.F_Type);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ObjectId), t => t.F_ObjectId.Contains(queryParams.F_ObjectId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserId), t => t.F_CreateUserId.Contains(queryParams.F_CreateUserId));
            if (queryParams.F_IsPublic != null)//可见性(默认私有0，公有1)
            {
                exp = exp.And(t => t.F_IsPublic == queryParams.F_IsPublic);
            }

            return (List<LrConfigTableEntity>)(await this.BaseRepository().FindList<LrConfigTableEntity>(exp.ToExpression()));
        }
        #endregion
    }
}