using jcode.autofac;
using jcode.iapplication;
using jcode.iquartz;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.quartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    public class TSSchemeBLL : BLLBase, ITSSchemeIBLL, BLL
    {
        private readonly TSSchemeService schemeService = new TSSchemeService();



        #region 获取数据

        /// <summary> 
        /// 获取页面显示列表数据 
        /// </summary> 
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param> 
        /// <returns></returns> 
        public Task<IEnumerable<TSSchemeInfoNoSchemeEntity>> GetPageList(Pagination pagination, string keyword, string appId = null)
        {
            return schemeService.GetPageList(pagination, keyword, appId);
        }

        /// <summary> 
        /// 获取列表数据 
        /// </summary> 
        /// <returns></returns> 
        public Task<IEnumerable<TSSchemeInfoEntity>> GetList()
        {
            return schemeService.GetList();
        }
        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="keyValue">主键</param> 
        /// <returns></returns> 
        public Task<TSSchemeInfoEntity> GetEntity(string keyValue)
        {
            return schemeService.GetEntity(keyValue);
        }


        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="runId">运行id</param> 
        /// <returns></returns> 
        public Task<TSSchemeInfoEntity> GetEntityByRunId(string runId) {
            return schemeService.GetEntityByRunId(runId);
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
            await schemeService.DeleteEntity(keyValue);
            // await QuartzHelper.DeleteJob(keyValue);
        }

        /// <summary> 
        /// 保存实体数据（新增、修改）
        /// </summary> 
        /// <param name="keyValue">主键</param>
        /// <param name="schemeInfoEntity">实体</param> 
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, TSSchemeInfoEntity schemeInfoEntity)
        {
            schemeInfoEntity.F_RunId = Guid.NewGuid().ToString();
            
            await schemeService.SaveEntity(keyValue, schemeInfoEntity);
            /*if (!string.IsNullOrEmpty(keyValue)) {
                // await QuartzHelper.DeleteJob(keyValue);
                schemeInfoEntity =await GetEntity(keyValue);
            }
                
            if (schemeInfoEntity.F_IsActive == 1 ) {
                var runId =await GetRunId(schemeInfoEntity.F_Id);
                await QuartzHelper.AddJob(schemeInfoEntity.F_Id, runId, schemeInfoEntity.F_Scheme.ToObject<TSSchemeModel>());
            }*/
        }

        /// <summary>
        /// 获取运行id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetRunId(string id) {
            TSSchemeInfoEntity schemeInfoEntity = new TSSchemeInfoEntity()
            {
                F_RunId = Guid.NewGuid().ToString()
            };
            await schemeService.UpdateEntity(id, schemeInfoEntity);
            return schemeInfoEntity.F_RunId;
        }

        #endregion

        #region 扩展应用
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="keyValue">任务主键</param>
        public async Task FinishJob(string keyValue)
        {
            TSSchemeInfoEntity schemeInfoEntity = new TSSchemeInfoEntity()
            {
                F_IsActive = 3
            };
            await schemeService.UpdateEntity(keyValue, schemeInfoEntity);
        }
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="keyValue">任务主键</param>
        public async Task PauseJob(string keyValue)
        {
            TSSchemeInfoEntity schemeInfoEntity = new TSSchemeInfoEntity()
            {
                F_IsActive = 2
            };
            await schemeService.UpdateEntity(keyValue, schemeInfoEntity);
            // await QuartzHelper.DeleteJob(keyValue);
        }
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="keyValue">任务进程主键</param>
        public async Task EnAbleJob(string keyValue)
        {
            TSSchemeInfoEntity schemeInfoEntity = new TSSchemeInfoEntity()
            {
                F_IsActive = 1,
                F_RunId = Guid.NewGuid().ToString()
            };
            // var entity = await GetEntity(keyValue);

            await schemeService.UpdateEntity(keyValue, schemeInfoEntity);
            // await QuartzHelper.AddJob(keyValue, schemeInfoEntity.F_RunId, entity.F_Scheme.ToObject<TSSchemeModel>());
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        public async Task UpdateEntity(TSSchemeInfoEntity entity)
        {
            await schemeService.UpdateEntity(entity.F_Id, entity);
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
            await schemeService.ExecuteBySql(code, sql);
        }
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        public async Task ExecuteProc(string code, string name)
        {
            await schemeService.ExecuteProc(code, name);
        }

        #endregion
    }
}
