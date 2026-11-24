using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导入设置
    /// </summary>
    public class ExcelImportService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetPageList(Pagination pagination, string keyword,string moduleId)
        {
            var expression = LinqExtensions.True<ExcelImportEntity>();
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
        /// 获取导入配置列表根据模块ID
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetList(string moduleId)
        {
            var expression = LinqExtensions.True<ExcelImportEntity>();
            expression = expression.And(t => t.F_ModuleId == moduleId && t.F_EnabledMark == 1);
            return this.BaseRepository().FindList(expression);
        }


        /// <summary>
        /// 获取所有导入配置信息
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetList()
        {
            return this.BaseRepository().FindList<ExcelImportEntity>(t => t.F_EnabledMark == 1);
        }

        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<ExcelImportEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<ExcelImportEntity>(keyValue);
        }
        
        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<ExcelImportEntity> GetEntityByCode(string code)
        {
            return this.BaseRepository().FindEntity<ExcelImportEntity>(t=>t.F_ModuleId == code);
        }
        /// <summary>
        /// 获取配置字段列表
        /// </summary>
        /// <param name="importId">配置信息主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportFieldEntity>> GetFieldList(string importId)
        {
            var expression = LinqExtensions.True<ExcelImportFieldEntity>();
            expression = expression.And(t => t.F_ImportId == importId);
            return this.BaseRepository().FindList(expression, "F_SortCode");
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteEntity(string keyValue)
        {

            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<ExcelImportEntity>(keyValue);
                await db.Delete<ExcelImportFieldEntity>(t => t.F_ImportId == keyValue );
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体数据</param>
        /// <param name="filedList">字段列表</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, ExcelImportEntity entity, List<ExcelImportFieldEntity> filedList)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                if (!string.IsNullOrEmpty(keyValue))
                {
                    entity.F_Id = keyValue;
                    entity.F_ModifyDate = DateTime.Now;
                    entity.F_ModifyUserId = this.GetUserId();
                    entity.F_ModifyUserName = this.GetUserName();
                    await db.Update(entity);
                }
                else
                {
                    entity.F_Id = Guid.NewGuid().ToString();
                    entity.F_CreateDate = DateTime.Now;
                    entity.F_CreateUserId = this.GetUserId();
                    entity.F_CreateUserName = this.GetUserName();
                    entity.F_EnabledMark = 1;
                    await db.Insert(entity);
                }
                await db.Delete<ExcelImportFieldEntity>(t => t.F_ImportId == entity.F_Id );
                foreach (var item in filedList)
                {
                    item.F_ImportId = entity.F_Id;
                    await db.Insert(item);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 更新配置主表
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public async Task UpdateEntity(string keyValue, ExcelImportEntity entity)
        {
            entity.F_Id = keyValue;
            entity.F_ModifyDate = DateTime.Now;
            entity.F_ModifyUserId = this.GetUserId();
            entity.F_ModifyUserName = this.GetUserName();
            await this.BaseRepository().Update(entity,true);
        }
        #endregion
    }
}
