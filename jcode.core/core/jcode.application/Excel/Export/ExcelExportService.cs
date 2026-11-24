using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导出设置
    /// </summary>
    public class ExcelExportService : ServiceBase
    {

        #region 获取数据 
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetPageList(Pagination pagination, string keyword, string moduleId)
        {
            var expression = LinqExtensions.True<ExcelExportEntity>();
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => t.F_Name.Contains(keyword));
            }
            if (!string.IsNullOrEmpty(moduleId))
            {
                expression = expression.And(t => t.F_ModuleId == moduleId);
            }
            expression = expression.And(t => t.F_IsEdit == null || t.F_IsEdit == 1);
            

            return this.BaseRepository().FindList(expression, pagination);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetList(string moduleId)
        {
            var expression = LinqExtensions.True<ExcelExportEntity>();
            expression = expression.And(t => t.F_ModuleId == moduleId && t.F_EnabledMark == 1);
            return this.BaseRepository().FindList(expression);
        }
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ExcelExportEntity>> GetList()
        {
            return this.BaseRepository().FindList<ExcelExportEntity>(t => t.F_EnabledMark == 1);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<ExcelExportEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<ExcelExportEntity>(keyValue);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await this.BaseRepository().Delete<ExcelExportEntity>(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, ExcelExportEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
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
