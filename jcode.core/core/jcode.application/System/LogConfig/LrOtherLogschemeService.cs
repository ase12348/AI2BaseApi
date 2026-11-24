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
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-03-28 16:23:13
    /// 描 述： 日志接口配置数据库执行类
    /// </summary>
    public class LrOtherLogschemeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<LrOtherLogschemeEntity, bool>> GetExpression(LrOtherLogschemeEntity queryParams) {
            var exp = Expressionable.Create<LrOtherLogschemeEntity>();
            if(!string.IsNullOrEmpty(queryParams.F_LogConfigId))
            {
                exp = exp.And(t => t.F_LogConfigId == queryParams.F_LogConfigId);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_EnCode),t => t.F_EnCode.Contains(queryParams.F_EnCode));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_FullName),t => t.F_FullName.Contains(queryParams.F_FullName));
            if(queryParams.F_RelationType != null)
            {
                exp = exp.And(t => t.F_RelationType == queryParams.F_RelationType);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_DataItemCode),t => t.F_DataItemCode.Contains(queryParams.F_DataItemCode));
            if(queryParams.F_SortCode != null)
            {
                exp = exp.And(t => t.F_SortCode == queryParams.F_SortCode);
            }
            if(!string.IsNullOrEmpty(queryParams.F_CreateDateQRange))
            {
                var f_CreateDate_list = queryParams.F_CreateDateQRange.Split(" - ");
                DateTime f_CreateDate = Convert.ToDateTime(f_CreateDate_list[0]);
                DateTime f_CreateDate_end = Convert.ToDateTime(f_CreateDate_list[1]);
                exp = exp.And(t => t.F_CreateDate >= f_CreateDate &&t.F_CreateDate <= f_CreateDate_end);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserId),t => t.F_CreateUserId.Contains(queryParams.F_CreateUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserName),t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_TenantId),t => t.F_TenantId.Contains(queryParams.F_TenantId));

            return exp.ToExpression();
        }

        /// <summary>
        /// 获取日志接口配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogschemeEntity>> GetList(LrOtherLogschemeEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrOtherLogschemeEntity>(expression,"F_SortCode ASC");
        }

        /// <summary>
        /// 获取日志接口配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogschemeEntity>> GetPageList(Pagination pagination, LrOtherLogschemeEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrOtherLogschemeEntity>(expression,pagination);
        }
        
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrOtherLogschemeEntity> GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<LrOtherLogschemeEntity>(keyValue);
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
            await this.BaseRepository().Delete<LrOtherLogschemeEntity>(keyValue);
        }

        /// <summary>
        /// 删除日志接口配置的数据根据外键
        /// <summary>
        /// <param name="key">关联键</param>
        /// <returns></returns>
        public async Task DeleteRelate(string key) {
            await this.BaseRepository().Delete<LrOtherLogschemeEntity>(t=>t.F_LogConfigId == key);
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrOtherLogschemeEntity entity)
        {
            if ( string.IsNullOrEmpty(keyValue) )
            {
                if(string.IsNullOrEmpty(entity.F_Id))
                {
                    entity.F_CreateDate = DateTime.Now;
                    entity.F_CreateUserId = this.GetUserId();
                    entity.F_CreateUserName = this.GetUserName();
                    entity.F_Id = Guid.NewGuid().ToString();
                }


                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;


                await this.BaseRepository().Update(entity);
            }
        }

        /// <summary>
        /// 保存子表数据
        /// <summary>
        /// <param name="key">外键值</param>
        /// <param name="list">数据集合</param>
        /// <returns></returns>
        public async Task SaveList(string key, IEnumerable<LrOtherLogschemeEntity> list) {
            var addList = new List<LrOtherLogschemeEntity>();
            var db = this.BaseRepository().BeginTrans();
            try{
                int num = 0;
                await db.Delete<LrOtherLogschemeEntity>(t => t.F_LogConfigId == key);
                foreach (var item in list)
                {
                    
                    if (string.IsNullOrEmpty(item.F_Id))
                    {
                        item.F_SortCode = num;
                        item.F_CreateDate = DateTime.Now;
                        item.F_CreateUserId = this.GetUserId();
                        item.F_CreateUserName = this.GetUserName();
                        item.F_Id = Guid.NewGuid().ToString();
                    }
                    item.F_LogConfigId = key;
                    addList.Add(item);
                    num++;
                }
                if (addList.Count > 0)
                {
                    await db.Inserts(addList);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        
        /// <summary>
        /// 批量删除实体
        /// <summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            string[] keyValuesArr = keyValues.Split(",");
            await this.BaseRepository().Delete<LrOtherLogschemeEntity>(keyValuesArr);
        }

        #endregion
    }
}
