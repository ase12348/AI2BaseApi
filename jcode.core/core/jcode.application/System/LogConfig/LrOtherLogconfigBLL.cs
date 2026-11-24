using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-03-28 16:23:13
    /// 描 述： 日志接口配置
    /// </summary>
    public class LrOtherLogconfigBLL : BLLBase, ILrOtherLogconfigBLL,BLL
    {
        private readonly LrOtherLogconfigService lrOtherLogconfigService = new LrOtherLogconfigService();

        private readonly ILrOtherLogschemeBLL _iLrOtherLogschemeBLL;
        /// <summary>
        /// 构造方法
        /// <summary>
        /// <param name="iLrOtherLogschemeBLL">日志接口配置-详情【lr_other_logscheme】接口</param>
        public LrOtherLogconfigBLL(ILrOtherLogschemeBLL iLrOtherLogschemeBLL)
        {
            _iLrOtherLogschemeBLL = iLrOtherLogschemeBLL?? throw new ArgumentNullException(nameof(iLrOtherLogschemeBLL));
        }


        #region 获取数据
        /// <summary>
        /// 获取日志接口配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogconfigEntity>> GetList(LrOtherLogconfigEntity queryParams)
        {
            return lrOtherLogconfigService.GetList(queryParams);
        }

        /// <summary>
        /// 获取日志接口配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrOtherLogconfigEntity>> GetPageList(Pagination pagination, LrOtherLogconfigEntity queryParams)
        {
            return lrOtherLogconfigService.GetPageList(pagination, queryParams);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrOtherLogconfigEntity> GetEntity(string keyValue)
        {
            return lrOtherLogconfigService.GetEntity(keyValue);
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
            await lrOtherLogconfigService.Delete(keyValue);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteAll(string keyValue)
        {
            var res = new LogConfigDto();
            res.LrOtherLogconfigEntity = await GetEntity(keyValue);
            if(res.LrOtherLogconfigEntity != null)
            {
                res.LrOtherLogschemeList = await _iLrOtherLogschemeBLL.GetList(new LrOtherLogschemeEntity { F_LogConfigId = res.LrOtherLogconfigEntity.F_Id });
            }
            lrOtherLogconfigService.BeginTrans();
            try
            {
                await Delete(keyValue);
                if(res.LrOtherLogconfigEntity != null)
                {
                    await _iLrOtherLogschemeBLL.DeleteRelateEntity(res.LrOtherLogconfigEntity.F_Id);
                }
                lrOtherLogconfigService.Commit();
            }
            catch (Exception)
            {
                lrOtherLogconfigService.Rollback();
                throw;
            }
        }


        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrOtherLogconfigEntity entity)
        {
            
            await lrOtherLogconfigService.SaveEntity(keyValue, entity);
        }

        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        public async Task SaveAll(string keyValue, LogConfigDto dto)
        {
            lrOtherLogconfigService.BeginTrans();
            try
            {
                await SaveEntity(keyValue,dto.LrOtherLogconfigEntity);
                await _iLrOtherLogschemeBLL.SaveList(dto.LrOtherLogconfigEntity.F_Id,dto.LrOtherLogschemeList);
                lrOtherLogconfigService.Commit();
            }
            catch (Exception)
            {
                lrOtherLogconfigService.Rollback();
                throw;
            }
        }


        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            await lrOtherLogconfigService.Deletes(keyValues);
        }

        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task DeleteAlls(string keyValues)
        {
            var keyValuelist = keyValues.Split(",");
            foreach (var keyValue in keyValuelist)
            {
                var res = new LogConfigDto();
                res.LrOtherLogconfigEntity = await GetEntity(keyValue);
            if(res.LrOtherLogconfigEntity != null)
            {
                res.LrOtherLogschemeList = await _iLrOtherLogschemeBLL.GetList(new LrOtherLogschemeEntity { F_LogConfigId = res.LrOtherLogconfigEntity.F_Id });
            }
                lrOtherLogconfigService.BeginTrans();
                try
                {
                    await Delete(keyValue);
                if(res.LrOtherLogconfigEntity != null)
                {
                    await _iLrOtherLogschemeBLL.DeleteRelateEntity(res.LrOtherLogconfigEntity.F_Id);
                }
                    lrOtherLogconfigService.Commit();
                }
                catch (Exception)
                {
                    lrOtherLogconfigService.Rollback();
                    throw;
                }
            }
        }


        #endregion

        #region 扩展方法
        /// <summary>
        /// 根据请求路径以及请求方式获取日志配置实体数据
        /// </summary>
        /// <param name="address">请求路径</param>
        /// <param name="requestMethod">请求方式</param>
        /// <returns></returns>
        public async Task<LogConfigDto> GetEntityByConfig(string address, string requestMethod)
        {
            var exp = Expressionable.Create<LrOtherLogconfigEntity>()
                .And(t => t.F_DeleteMark == 0)
                .And(t => t.F_EnabledMark == 1)
                .AndIF(!string.IsNullOrEmpty(requestMethod), t => t.F_RequestMethod.ToUpper() == requestMethod.ToUpper())
                .AndIF(!string.IsNullOrEmpty(address), t => t.F_Address == address)
                .ToExpression();

            var configEntity = await lrOtherLogconfigService.BaseRepository().FindEntity<LrOtherLogconfigEntity>(exp);

            if (configEntity != null)
            {
                LogConfigDto dto = new LogConfigDto()
                {
                    LrOtherLogconfigEntity = configEntity,
                    LrOtherLogschemeList = await _iLrOtherLogschemeBLL.GetList(new LrOtherLogschemeEntity { F_LogConfigId = configEntity.F_Id })

                };
                return dto;
            }
            return null;
        }
        #endregion
    }
}
