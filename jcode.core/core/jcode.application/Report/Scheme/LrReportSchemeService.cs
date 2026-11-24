using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期： 2022-05-05 10:40:00
    /// 描 述： 简洁报表数据库执行类
    /// </summary>
    public class LrReportSchemeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<LrReportSchemeEntity, bool>> GetExpression(LrReportSchemeEntity queryParams) {
            var expression = LinqExtensions.True<LrReportSchemeEntity>();
            if(!string.IsNullOrEmpty(queryParams.F_Name))
            {
                expression = expression.And(t => t.F_Name.Contains(queryParams.F_Name));
            }
            if(!string.IsNullOrEmpty(queryParams.F_Code))
            {
                expression = expression.And(t => t.F_Code.Contains(queryParams.F_Code));
            }
            if(!string.IsNullOrEmpty(queryParams.F_TempCategory))
            {
                expression = expression.And(t => t.F_TempCategory == queryParams.F_TempCategory);
            }
            if(!string.IsNullOrEmpty(queryParams.F_TempStyle))
            {
                expression = expression.And(t => t.F_TempStyle == queryParams.F_TempStyle);
            }
            if(!string.IsNullOrEmpty(queryParams.F_TempType))
            {
                expression = expression.And(t => t.F_TempType == queryParams.F_TempType);
            }
            if(!string.IsNullOrEmpty(queryParams.F_Description))
            {
                expression = expression.And(t => t.F_Description.Contains(queryParams.F_Description));
            }
            if(queryParams.F_ParamJson != null)
            {
                expression = expression.And(t => t.F_ParamJson == queryParams.F_ParamJson);
            }
            if(queryParams.F_SortCode != null)
            {
                expression = expression.And(t => t.F_SortCode == queryParams.F_SortCode);
            }
            if(queryParams.F_EnabledMark != null)
            {
                expression = expression.And(t => t.F_EnabledMark == queryParams.F_EnabledMark);
            }
            if(!string.IsNullOrEmpty(queryParams.F_CreateDateQRange))
            {
                var f_CreateDate_list = queryParams.F_CreateDateQRange.Split(',');
                DateTime f_CreateDate = Convert.ToDateTime(f_CreateDate_list[0]);
                DateTime f_CreateDate_end = Convert.ToDateTime(f_CreateDate_list[1]);
                expression = expression.And(t => t.F_CreateDate >= f_CreateDate &&t.F_CreateDate <= f_CreateDate_end);
            }
            if(!string.IsNullOrEmpty(queryParams.F_CreateUserId))
            {
                expression = expression.And(t => t.F_CreateUserId == queryParams.F_CreateUserId);
            }
            if(!string.IsNullOrEmpty(queryParams.F_CreateUserName))
            {
                expression = expression.And(t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            }
            if(!string.IsNullOrEmpty(queryParams.F_ModifyDateQRange))
            {
                var f_ModifyDate_list = queryParams.F_ModifyDateQRange.Split(',');
                DateTime f_ModifyDate = Convert.ToDateTime(f_ModifyDate_list[0]);
                DateTime f_ModifyDate_end = Convert.ToDateTime(f_ModifyDate_list[1]);
                expression = expression.And(t => t.F_ModifyDate >= f_ModifyDate &&t.F_ModifyDate <= f_ModifyDate_end);
            }
            if(!string.IsNullOrEmpty(queryParams.F_ModifyUserId))
            {
                expression = expression.And(t => t.F_ModifyUserId == queryParams.F_ModifyUserId);
            }
            if(!string.IsNullOrEmpty(queryParams.F_ModifyUserName))
            {
                expression = expression.And(t => t.F_ModifyUserName.Contains(queryParams.F_ModifyUserName));
            }

            return expression;
        }

        /// <summary>
        /// 获取简洁报表的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrReportSchemeEntity>> GetList(LrReportSchemeEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrReportSchemeEntity>(expression);
        }

        /// <summary>
        /// 获取简洁报表的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrReportSchemeEntity>> GetPageList(Pagination pagination, LrReportSchemeEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrReportSchemeEntity>(expression,pagination);
        }
        
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrReportSchemeEntity> GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<LrReportSchemeEntity>(keyValue);
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
            await this.BaseRepository().Delete<LrReportSchemeEntity>(keyValue);
        }

        

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrReportSchemeEntity entity)
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
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
                entity.F_Id = keyValue;


                await this.BaseRepository().Update(entity);
            }
        }

        
        #endregion
    }
}
