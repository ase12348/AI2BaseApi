using jcode.util;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using System.Linq.Expressions;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期： 2022-05-05 11:14:02
    /// 描 述： 数据表模型常用字段维护数据库执行类
    /// </summary>
    public class DbFieldService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<DbFieldEntity, bool>> GetExpression(DbFieldEntity queryParams)
        {
            var expression = LinqExtensions.True<DbFieldEntity>();
            if (!string.IsNullOrEmpty(queryParams.F_Name))
            {
                expression = expression.And(t => t.F_Name.Contains(queryParams.F_Name));
            }
            if (!string.IsNullOrEmpty(queryParams.F_DataType))
            {
                expression = expression.And(t => t.F_DataType.Contains(queryParams.F_DataType));
            }
            if (queryParams.F_Remark != null)
            {
                expression = expression.And(t => t.F_Remark == queryParams.F_Remark);
            }
            if (!string.IsNullOrEmpty(queryParams.F_CreateDateQRange))
            {
                var f_CreateDate_list = queryParams.F_CreateDateQRange.Split(',');
                DateTime f_CreateDate = Convert.ToDateTime(f_CreateDate_list[0]);
                DateTime f_CreateDate_end = Convert.ToDateTime(f_CreateDate_list[1]);
                expression = expression.And(t => t.F_CreateDate >= f_CreateDate && t.F_CreateDate <= f_CreateDate_end);
            }
            if (!string.IsNullOrEmpty(queryParams.F_CreateUserId))
            {
                expression = expression.And(t => t.F_CreateUserId.Contains(queryParams.F_CreateUserId));
            }
            if (!string.IsNullOrEmpty(queryParams.F_CreateUserName))
            {
                expression = expression.And(t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            }
            if (queryParams.F_Length != null)
            {
                expression = expression.And(t => t.F_Length == queryParams.F_Length);
            }

            return expression;
        }

        /// <summary>
        /// 获取数据表模型常用字段维护的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbFieldEntity>> GetList(DbFieldEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<DbFieldEntity>(expression);
        }

        /// <summary>
        /// 获取数据表模型常用字段维护的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbFieldEntity>> GetPageList(Pagination pagination, DbFieldEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<DbFieldEntity>(expression, pagination);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DbFieldEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<DbFieldEntity>(keyValue);
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
            await this.BaseRepository().Delete<DbFieldEntity>(keyValue);
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DbFieldEntity entity)
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
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Update(entity);
            }
        }


        #endregion
    }
}
