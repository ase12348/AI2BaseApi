using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.09.21
    /// 描 述：智能体管理数据库执行类 
    /// </summary>
    public class AgentService : ServiceBase
    {
        #region 获取数据 
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        private Expression<Func<AgentEntity, bool>> GetExpression(string keyword)
        {
            var exp = Expressionable.Create<AgentEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(keyword),
                t => t.F_Name.Contains(keyword) || t.F_Description.Contains(keyword));
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<AgentEntity>> GetList(string keyword)
        {
            var expression = GetExpression(keyword);
            return this.BaseRepository().FindList<AgentEntity>(expression, "F_CreateDate DESC");
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<AgentEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<AgentEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteEntity(string id)
        {
            await this.BaseRepository().Delete<AgentEntity>(t => t.F_Id == id);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string id, AgentEntity entity)
        {
            entity.F_ModifyDate = DateTime.Now;
            entity.F_ModifyUserId = this.GetUserId();
            entity.F_ModifyUserName = this.GetUserName();
            if (!string.IsNullOrEmpty(id))
            {
                entity.F_Id = id;
                await this.BaseRepository().Update(entity);
            }
            else
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            }
        }
        /// <summary>
        /// 复制智能体，生成一个非内置、未发布的副本
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>新实体主键</returns>
        public async Task<string> CopyEntity(string id)
        {
            var source = await this.BaseRepository().FindEntityByKey<AgentEntity>(id);
            if (source == null)
            {
                return null;
            }
            var entity = new AgentEntity
            {
                F_Id = Guid.NewGuid().ToString(),
                F_Name = $"{source.F_Name}-副本",
                F_Icon = source.F_Icon,
                F_Description = source.F_Description,
                F_IsBuiltIn = 0,
                F_Published = 0,
                F_CreateDate = DateTime.Now,
                F_CreateUserId = this.GetUserId(),
                F_CreateUserName = this.GetUserName(),
                F_ModifyDate = DateTime.Now,
                F_ModifyUserId = this.GetUserId(),
                F_ModifyUserName = this.GetUserName(),
            };
            await this.BaseRepository().Insert(entity);
            return entity.F_Id;
        }
        #endregion
    }
}
