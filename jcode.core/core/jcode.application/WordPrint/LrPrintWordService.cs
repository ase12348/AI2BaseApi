using jcode.util;
using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq.Expressions;
namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2023-05-06 14:06:35
    /// 描 述： word打印模版设置数据库执行类
    /// </summary>
    public class LrPrintWordService: ServiceBase {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        private Expression<Func<LrPrintWordEntity, bool>>GetExpression(LrPrintWordEntity queryParams, string tags = null) {
            var exp = Expressionable.Create<LrPrintWordEntity>();
            // 标签过滤：F_Category 存逗号分隔的标签串，包含匹配即可命中其中任一标签
            exp = exp.AndIF(!string.IsNullOrEmpty(tags), t => t.F_Category.Contains(tags));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Code), t => t.F_Code.Contains(queryParams.F_Code));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_Name), t => t.F_Name.Contains(queryParams.F_Name));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.Keyword), t => t.F_Name.Contains(queryParams.Keyword) || t.F_Code.Contains(queryParams.Keyword));
            if (queryParams.F_EnabledMark != null) {
                exp = exp.And(t => t.F_EnabledMark == queryParams.F_EnabledMark);
            }
            if (!string.IsNullOrEmpty(queryParams.F_ModifyUserId)) {
                exp = exp.And(t => t.F_ModifyUserId == queryParams.F_ModifyUserId);
            }
            if (!string.IsNullOrEmpty(queryParams.F_Category)) {
                exp = exp.And(t => t.F_Category == queryParams.F_Category);
            }
            if (!string.IsNullOrEmpty(queryParams.F_AppId)) {
                exp = exp.And(t => t.F_AppId == queryParams.F_AppId);
            }
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_ModifyUserName), t => t.F_ModifyUserName.Contains(queryParams.F_ModifyUserName));
            if (!string.IsNullOrEmpty(queryParams.F_ModifyDateTimeQRange)) {
                var f_ModifyDateTime_list = queryParams.F_ModifyDateTimeQRange.Split(" - ");
                DateTime f_ModifyDateTime = Convert.ToDateTime(f_ModifyDateTime_list[0]);
                DateTime f_ModifyDateTime_end = Convert.ToDateTime(f_ModifyDateTime_list[1]);
                exp = exp.And(t => t.F_ModifyDateTime >= f_ModifyDateTime && t.F_ModifyDateTime <= f_ModifyDateTime_end);
            }
            return exp.ToExpression();
        }
        /// <summary>
        /// 获取word打印模版设置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintWordEntity>>GetList(LrPrintWordEntity queryParams) {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrPrintWordEntity>(expression);
        }
        /// <summary>
        /// 获取word打印模版设置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintWordEntity>>GetPageList(Pagination pagination, LrPrintWordEntity queryParams, string tags = null) {
            var expression = GetExpression(queryParams, tags);
            return this.BaseRepository().FindList<LrPrintWordEntity>(expression, pagination);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintWordEntity>GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<LrPrintWordEntity>(keyValue);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<LrPrintWordEntity>GetEntityByCode(string code) {
            return this.BaseRepository().FindEntity<LrPrintWordEntity>(t=>t.F_Code == code);
        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await this.BaseRepository().Delete<LrPrintWordEntity>(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues) {
            string[] keyValuesArr = keyValues.Split(",");
            await this.BaseRepository().Delete<LrPrintWordEntity>(keyValuesArr);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintWordEntity entity) {
            entity.F_ModifyUserId = this.GetUserId();
            entity.F_ModifyUserName = this.GetUserName();
            entity.F_ModifyDateTime = DateTime.Now;
            if (string.IsNullOrEmpty(keyValue)) {
                if (string.IsNullOrEmpty(entity.F_Id)) {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                entity.F_CreateDateTime = DateTime.Now;
                await this.BaseRepository().Insert(entity);
            } else {
                entity.F_Id = keyValue;
                // 更新是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(entity.F_AppId)) {
                    var old = await this.BaseRepository().FindEntityByKey<LrPrintWordEntity>(keyValue);
                    entity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion
    }
}