using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板数据库执行类
    /// </summary>
    public class LrPrintTemplateinfoService : ServiceBase
    {
        #region 获取数据
                /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="isEnabled">是否获取正式表单</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintTemplateinfoEntity>> GetPageList(Pagination pagination, LrPrintTemplateinfoEntity queryParams, bool isEnabled = false, string tags = null)
        {
            return this.BaseRepository().FindListByQueryable<LrPrintTemplateinfoEntity>(q => {
                var queryable = q.LeftJoin<LrPrintTemplateEntity>((t, t1) => t.F_TemplateId == t1.F_Id);
                var exp = Expressionable.Create<LrPrintTemplateinfoEntity, LrPrintTemplateEntity>();

                if (!string.IsNullOrEmpty(queryParams.Keyword))
                {
                    exp = exp.And((t, t1) => t.F_Name.Contains(queryParams.Keyword));
                }
                if (!string.IsNullOrEmpty(queryParams.F_Category))
                {
                    exp = exp.And((t, t1) => t.F_Category == queryParams.F_Category);
                }
                // 标签过滤：F_Category 存逗号分隔的标签串，包含匹配即可命中其中任一标签
                if (!string.IsNullOrEmpty(tags))
                {
                    exp = exp.And((t, t1) => t.F_Category.Contains(tags));
                }
                if (isEnabled)
                {
                    exp = exp.And((t, t1) => t.F_EnabledMark == 1 && t1.F_Type == 1);
                }
                if (!string.IsNullOrEmpty(queryParams.F_AppId))
                {
                    exp = exp.And((t, t1) => t.F_AppId == queryParams.F_AppId);
                }

                return queryable
                .Where(exp.ToExpression())
                .Select((t, t1) => new LrPrintTemplateinfoEntity()
                {
                    F_Id = t.F_Id,
                    F_Name = t.F_Name,
                    F_Code = t.F_Code,
                    F_Category = t.F_Category,
                    F_TemplateId = t.F_TemplateId,
                    F_EnabledMark = t.F_EnabledMark,
                    F_Description = t.F_Description,
                    F_AppId = t.F_AppId,

                    F_Type = t1.F_Type,
                    F_CreateDate = t1.F_CreateDate,
                    F_CreateUserId = t1.F_CreateUserId,
                    F_CreateUserName = t1.F_CreateUserName
                });
            }, pagination);
        }
                /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintTemplateinfoEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<LrPrintTemplateinfoEntity>(keyValue);
        }
        /// <summary>
        /// 获取实体数据--根据模板编码
        /// </summary>
        /// <param name="code">模板编码</param>
        /// <returns></returns>
        public Task<LrPrintTemplateinfoEntity> GetEntityByCode(string code)
        {
            return this.BaseRepository().FindEntity<LrPrintTemplateinfoEntity>(t => t.F_Code == code);
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
            await this.BaseRepository().Delete<LrPrintTemplateinfoEntity>(keyValue);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintTemplateinfoEntity entity)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                if (string.IsNullOrEmpty(entity.F_Id))
                {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;
                // 更新是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(entity.F_AppId))
                {
                    var old = await this.BaseRepository().FindEntityByKey<LrPrintTemplateinfoEntity>(keyValue);
                    entity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(entity);
            }
        }
        
        /// <summary>
        /// 启用/停用模板
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        public async Task UpdateState(string id, int state)
        {
            LrPrintTemplateinfoEntity entity = new LrPrintTemplateinfoEntity
            {
                F_Id = id,
                F_EnabledMark = state
            };
            await this.BaseRepository().Update(entity, true);
        }
        #endregion
    }
}
