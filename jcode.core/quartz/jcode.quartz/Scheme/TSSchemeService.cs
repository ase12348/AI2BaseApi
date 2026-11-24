using jcode.iapplication;
using jcode.iquartz;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SqlSugar;

namespace jcode.quartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    public class TSSchemeService : ServiceBase
    {
        #region 获取数据 

        /// <summary> 
        /// 获取页面显示列表数据 
        /// </summary> 
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param> 
        /// <returns></returns> 
        public Task<IEnumerable<TSSchemeInfoNoSchemeEntity>> GetPageList(Pagination pagination, string keyword, string appId = null)
        {
            var appCodes = new List<string>();
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                appCodes = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
            }


            var exp = Expressionable.Create<TSSchemeInfoNoSchemeEntity>()
                .AndIF(!keyword.IsEmpty(), t =>  t.F_Name.Contains(keyword))
                .AndIF(!string.IsNullOrEmpty(appId), t => t.F_AppId == appId)
                .AndIF(ConfigHelper.GetConfig().OpenAppCodeFilter,t=>appCodes.Contains(t.F_AppCode))
                .ToExpression();

            return this.BaseRepository(false).FindList(exp, pagination);
        }
        /// <summary>
        /// 获取所有启用的任务
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<TSSchemeInfoEntity>> GetList()
        {
            var appCodes = new List<string>();
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                appCodes = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
            }

            var exp = Expressionable.Create<TSSchemeInfoEntity>()
                .And(t=>t.F_IsActive == 1)
                .AndIF(ConfigHelper.GetConfig().OpenAppCodeFilter,t=>appCodes.Contains(t.F_AppCode))
                .ToExpression();

            return this.BaseRepository(false).FindList(exp);
        }
        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="keyValue">主键</param> 
        /// <returns></returns> 
        public Task<TSSchemeInfoEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository(false).FindEntityByKey<TSSchemeInfoEntity>(keyValue);
        }

        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="runId">运行id</param> 
        /// <returns></returns> 
        public Task<TSSchemeInfoEntity> GetEntityByRunId(string runId)
        {
            return this.BaseRepository(false).FindEntity<TSSchemeInfoEntity>(t=>t.F_RunId == runId);
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
            await this.BaseRepository(false).Delete<TSSchemeInfoEntity>(keyValue);
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary> 
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体数据</param> 
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, TSSchemeInfoEntity entity)
        {

            TSSchemeModel model = entity.F_Scheme.ToObject<TSSchemeModel>();
            if (model.StartType == 1)
            {
                entity.F_BeginTime = DateTime.Now;
            }
            else
            {
                entity.F_BeginTime = model.StartTime;
            }

            entity.F_EndType = model.EndType;
            if (model.EndType == 1)
            {
                entity.F_EndTime = DateTime.MaxValue.AddDays(-1);
            }
            else
            {
                entity.F_EndTime = model.EndTime;
            }

            var userId = GetUserId();
            if (!string.IsNullOrEmpty(userId))
            {
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = userId;
            }


            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
                if (entity.F_IsActive == 3)
                {
                    entity.F_IsActive = 1;
                }

                await this.BaseRepository(false).Update(entity,true);
            }
            else
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_IsActive = 1;
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = GetUserId();
                await this.BaseRepository(false).Insert(entity);
            }
        }
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        /// <returns></returns>
        public async Task UpdateEntity(string keyValue, TSSchemeInfoEntity entity)
        {
            entity.F_Id = keyValue;
            await this.BaseRepository(false).Update(entity,true);
        }
        #endregion

        #region 执行sql语句和存储过程
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        public async Task ExecuteBySql(string code, string sql)
        {
            await this.BaseRepository(code, false).ExecuteSql(sql,300);
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task ExecuteProc(string code, string name)
        {
            await this.BaseRepository(code, false).ExecuteProc(name,300);
        }

        #endregion
    }
}
