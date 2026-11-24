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
    /// 极简搭AI应用开发基座项目-图片模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2022-07-07 11:19:40
    /// 描 述： 图片管理数据库执行类
    /// </summary>
    public class LrBaseImgService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取查询表达式
        /// </summary>
        /// <returns></returns>
        private Expression<Func<LrBaseImgEntity, bool>> GetExpression(LrBaseImgEntity queryParams)
        {
            var exp = Expressionable.Create<LrBaseImgEntity>();
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_FileName), t => t.F_FileName.Contains(queryParams.F_FileName));
            exp = exp.AndIF(!string.IsNullOrEmpty(queryParams.F_FileExtensions), t => t.F_FileExtensions.Contains(queryParams.F_FileExtensions));
            if (!string.IsNullOrEmpty(queryParams.F_FileType))
            {
                exp = exp.And(t => t.F_FileType == queryParams.F_FileType);
            }
            if (!string.IsNullOrEmpty(queryParams.F_CreateDateQRange))
            {
                var f_CreateDate_list = queryParams.F_CreateDateQRange.Split(',');
                DateTime f_CreateDate = Convert.ToDateTime(f_CreateDate_list[0]);
                DateTime f_CreateDate_end = Convert.ToDateTime(f_CreateDate_list[1]);
                exp = exp.And(t => t.F_CreateDate >= f_CreateDate && t.F_CreateDate <= f_CreateDate_end);
            }


            exp = exp.And(t => t.F_CreateUserId == this.GetUserId() || t.F_IsPublic == 1);



            return exp.ToExpression();
        }

        /// <summary>
        /// 获取图片管理的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseImgEntity>> GetList(LrBaseImgEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrBaseImgEntity>(expression);
        }

        /// <summary>
        /// 获取图片管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseImgEntity>> GetPageList(Pagination pagination, LrBaseImgEntity queryParams)
        {
            var expression = GetExpression(queryParams);
            return this.BaseRepository().FindList<LrBaseImgEntity>(expression, pagination);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseImgEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<LrBaseImgEntity>(keyValue);
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
            await this.BaseRepository().Delete<LrBaseImgEntity>(keyValue);
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseImgEntity entity)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
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
