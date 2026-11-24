using jcode.util;
using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq.Expressions;
using System.Linq;
namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-03-01 10:15:24
    /// 描 述： 图标管理数据库执行类
    /// </summary>
    public class LrBaseIconsService: ServiceBase {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        private Expression<Func<LrBaseIconsEntity, bool>>GetExpression(LrBaseIconsEntity queryParams) {
            var exp = Expressionable.Create<LrBaseIconsEntity>();
            if (queryParams.F_Height != null) {
                exp = exp.And(t => t.F_Height == queryParams.F_Height);
            }
            if (queryParams.F_Width != null) {
                exp = exp.And(t => t.F_Width == queryParams.F_Width);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Name), t => t.F_Name.Contains(queryParams.F_Name));
            
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.Keyword), t => t.F_Name.Contains(queryParams.Keyword) || t.F_Code.Contains(queryParams.Keyword));
            
            if (!string.IsNullOrEmpty(queryParams.F_Code)) {
                exp = exp.And(t => t.F_Code == queryParams.F_Code);
            }
            if (!string.IsNullOrEmpty(queryParams.F_Category)) {
                exp = exp.And(t => t.F_Category == queryParams.F_Category);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserId), t => t.F_CreateUserId.Contains(queryParams.F_CreateUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_CreateUserName), t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ModifyUserId), t => t.F_ModifyUserId.Contains(queryParams.F_ModifyUserId));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ModifyUserName), t => t.F_ModifyUserName.Contains(queryParams.F_ModifyUserName));
            if (queryParams.F_Body != null) {
                exp = exp.And(t => t.F_Body == queryParams.F_Body);
            }
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取图标管理的列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseIconsEntity>>GetALLList() {
            return this.BaseRepository().FindAll<LrBaseIconsEntity>();
        }
        /// <summary>
        /// 获取图标管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseIconsEntity>>GetPageList(Pagination pagination, LrBaseIconsEntity queryParams) {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrBaseIconsEntity>(expression, pagination);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseIconsEntity>GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<LrBaseIconsEntity>(keyValue);
        }
        #endregion
        
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await this.BaseRepository().Delete<LrBaseIconsEntity>(keyValue);
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
            await this.BaseRepository().Delete<LrBaseIconsEntity>(keyValuesArr);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseIconsEntity entity) {
            entity.F_ModifyDate = DateTime.Now;
            entity.F_ModifyUserId = this.GetUserId();
            entity.F_ModifyUserName = this.GetUserName();
            if (string.IsNullOrEmpty(keyValue)) {
                if (string.IsNullOrEmpty(entity.F_Id)) {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            } else {
                entity.F_Id = keyValue;
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion

        #region 版本

        public async Task<string> GetVer()
        {
            var entity = await  this.BaseRepository().FindEntity<LrBaseIconverEntity>(t=>t.F_Id == "id");
            return entity?.F_Ver;
        }
        
        /// <summary>
        /// 写入版本
        /// </summary>
        /// <returns></returns>
        public async Task SetNewVer()
        {
            var ver = await GetVer();
            var entity = new LrBaseIconverEntity()
            {
                F_Id = "id",
                F_Ver = Guid.NewGuid().ToString()
            };
            if(string.IsNullOrEmpty(ver))
            {
                await this.BaseRepository().Insert(entity);
            } else {
               
                await this.BaseRepository().Update(entity);
            }
        }
        
        #endregion
    }
}