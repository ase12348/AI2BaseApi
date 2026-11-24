using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using TencentCloud.Cme.V20191029.Models;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板数据库执行类
    /// </summary>
    public class LrPrintTemplateService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<LrPrintTemplateEntity, bool>> GetExpression(LrPrintTemplateEntity queryParams)
        {
            var exp = Expressionable.Create<LrPrintTemplateEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_TemplateInfoId), t => t.F_TemplateInfoId.Contains(queryParams.F_TemplateInfoId));
            if (queryParams.F_Type != null)
            {
                exp = exp.And(t => t.F_Type == queryParams.F_Type);
            }
            if (queryParams.F_Component != null)
            {
                exp = exp.And(t => t.F_Component == queryParams.F_Component);
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
            if (queryParams.F_TenantId != null)
            {
                exp = exp.And(t => t.F_TenantId == queryParams.F_TenantId);
            }

            return exp.ToExpression();
        }
        /// <summary>
        /// 获取打印设计-打印模板的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintTemplateEntity>> GetPageList(Pagination pagination, LrPrintTemplateEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrPrintTemplateEntity>(expression, pagination);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintTemplateEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<LrPrintTemplateEntity>(keyValue);
        }
        
        /// <summary>
        /// 获取最新的模版版本（排除草稿）
        /// </summary>
        /// <param name="templateInfoId"></param>
        /// <returns></returns>
        public async Task<LrPrintTemplateEntity> GetLastEntity(string templateInfoId)
        {
            var list =(List<LrPrintTemplateEntity>)await this.BaseRepository()
                .FindList<LrPrintTemplateEntity>(t => t.F_TemplateInfoId == templateInfoId && t.F_Type == 1 , "F_CreateDate DESC");

            if (list.Count > 0)
            {
                return list[0];
            }

            return null;
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
            await this.BaseRepository().Delete<LrPrintTemplateEntity>(keyValue);
        }
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="templateInfoId">模版信息主键</param>
        /// <returns></returns>
        public async Task DeleteByInfo(string templateInfoId)
        {
            
            await this.BaseRepository().Delete<LrPrintTemplateEntity>(t=>t.F_TemplateInfoId == templateInfoId);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintTemplateEntity entity)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(entity.F_Id))
                {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion
    }
}
