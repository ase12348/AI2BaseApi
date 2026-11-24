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
    /// 日 期：2026.09.20
    /// 描 述：模型管理数据库执行类 
    /// </summary>
    public class AiModelService : ServiceBase
    {
        #region 获取数据 
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        private Expression<Func<AiModelEntity, bool>> GetExpression(string keyword)
        {
            var exp = Expressionable.Create<AiModelEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(keyword),
                t => t.F_Model.Contains(keyword) || t.F_Provider.Contains(keyword) || t.F_Type.Contains(keyword));
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<AiModelEntity>> GetList(string keyword)
        {
            var expression = GetExpression(keyword);
            return this.BaseRepository().FindList<AiModelEntity>(expression, "F_ModifyDate DESC");
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<AiModelEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<AiModelEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteEntity(string id)
        {
            await this.BaseRepository().Delete<AiModelEntity>(t => t.F_Id == id);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string id, AiModelEntity entity)
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
                entity.F_EnabledMark = 1;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            }
        }
        #endregion
    }
}
