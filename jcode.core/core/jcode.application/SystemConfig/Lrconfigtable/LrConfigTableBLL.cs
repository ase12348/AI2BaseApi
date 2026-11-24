using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.util;
using Org.BouncyCastle.Crypto.Operators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TencentCloud.Ccc.V20200210.Models;
using TencentCloud.Cme.V20191029.Models;

namespace jcode.application
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-10-10 09:51:29
    /// 描 述： 数据列表配置-高级查询、表格列
    /// </summary>
    public class LrConfigTableBLL : BLLBase, ILrConfigTableBLL, BLL
    {
        private readonly LrConfigTableService lrConfigTableService = new LrConfigTableService();
        private readonly ICache _iCache;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public LrConfigTableBLL(ICache iCache)
        {
            _iCache = iCache;
        }
        #region 获取数据
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigTableEntity>> GetList(LrConfigTableEntity queryParams)
        {
            return lrConfigTableService.GetList(queryParams);
        }
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigTableEntity>> GetPageList(Pagination pagination, LrConfigTableEntity queryParams)
        {
            return lrConfigTableService.GetPageList(pagination, queryParams);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<LrConfigTableEntity> GetEntity(string keyValue)
        {
            string key = $"config_queryviewinfo_{keyValue}";
            var entity = _iCache.Read<LrConfigTableEntity>(key);
            if (entity==null)
            {
                entity= await lrConfigTableService.GetEntity(keyValue);
                _iCache.Write<LrConfigTableEntity>(key, entity);
            }
            return entity;
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
            await lrConfigTableService.Delete(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            await lrConfigTableService.Deletes(keyValues);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrConfigTableEntity entity)
        {
            await lrConfigTableService.SaveEntity(keyValue, entity);
        }
        #endregion

        #region 扩展方法

        #region 表格列
        /// <summary>
        /// 获取当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        public async Task<LrConfigTableEntity> GetThCol(string objectId)
        {
            var user = await CurrentUser();
            string key = $"config_thcol_{objectId}_{user.F_UserId}";
            var entity = _iCache.Read<LrConfigTableEntity>(key);
            if (entity == null)
            {
                var entitys = (List<LrConfigTableEntity>)(await lrConfigTableService.GetTableConfig(new LrConfigTableEntity { F_Type=2, F_ObjectId= objectId, F_CreateUserId= user.F_UserId}));
                if (entitys.Count > 0)
                {
                    entity = entitys[0];
                }
                _iCache.Write(key, entity);
            }
            return entity;
        }
        /// <summary>
        /// 保存当前登录人员的个性化表格列（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        public async Task SaveThCol(LrConfigTableEntity entity)
        {
            if (string.IsNullOrEmpty(entity.F_ObjectId))
            {
                throw new Exception("功能对象不明确！");
            }
            var theme = await GetThCol(entity.F_ObjectId);
            if (theme != null)//编辑
            {
                await lrConfigTableService.SaveEntity(theme.F_Id, entity);
                _iCache.Remove($"config_thcol_{entity.F_ObjectId}_{theme.F_CreateUserId}");
            }
            else//新增
            {
                await lrConfigTableService.SaveEntity(null, entity);
            }
        }
        /// <summary>
        /// 清除当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        public async Task DeleteThCol(string objectId)
        {
            var theme = await GetThCol(objectId);
            if (theme != null)
            {
                await lrConfigTableService.Delete(theme.F_Id);
                _iCache.Remove($"config_thcol_{objectId}_{theme.F_CreateUserId}");
            }
        }
        #endregion

        #region 高级查询
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LrConfigTableEntity>> GetQueryViews(string objectId)
        {
            var user = await CurrentUser();
            List<LrConfigTableEntity> views = new List<LrConfigTableEntity>();
            //私有视图
            var queryviewspr=await GetQueryViewsPr(objectId);
            //公有视图
            var queryviewspu = await GetQueryViewsPu(objectId);
            if (queryviewspr.Count>0)
            {
                views.AddRange(queryviewspr);
            }
            if (queryviewspu.Count > 0)
            {
                views.AddRange(queryviewspu);
            }
            return views;
        }
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表-私有数据
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<List<LrConfigTableEntity>> GetQueryViewsPr(string objectId)
        {
            var user = await CurrentUser();
            string key = $"config_queryviewspr_{objectId}_{user.F_UserId}";
            var entitys = _iCache.Read<List<LrConfigTableEntity>>(key);
            if (entitys == null)
            {
                entitys = (List<LrConfigTableEntity>)(await lrConfigTableService.GetTableConfig(new LrConfigTableEntity { F_Type = 1, F_ObjectId = objectId, F_CreateUserId = user.F_UserId, F_IsPublic = 0 }));
                if (entitys.Count>0)
                {
                    _iCache.Write(key, entitys);
                }
            }
            return entitys;
        }
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表-公有数据
        /// </summary>
        /// <param name="objectId"></param>
        /// <returns></returns>
        public async Task<List<LrConfigTableEntity>> GetQueryViewsPu(string objectId)
        {
            string key = $"config_queryviewspu_{objectId}";
            var entitys = _iCache.Read<List<LrConfigTableEntity>>(key);
            if (entitys == null)
            {
                entitys = (List<LrConfigTableEntity>)(await lrConfigTableService.GetTableConfig(new LrConfigTableEntity { F_Type = 1, F_ObjectId = objectId, F_IsPublic = 1 }));
                _iCache.Write(key, entitys);
            }
            return entitys;
        }
        /// <summary>
        /// 保存数据(新增、编辑)
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveQueryView(string keyValue, LrConfigTableEntity entity)
        {
            if (string.IsNullOrEmpty(entity.F_ObjectId))
            {
                throw new Exception("功能对象不明确！");
            }
            if (string.IsNullOrEmpty(keyValue))//新增
            {
                await lrConfigTableService.SaveEntity(null, entity);
            }
            else//编辑
            {
                await lrConfigTableService.SaveEntity(keyValue, entity);
            }
            _iCache.Remove($"config_queryviewspr_{entity.F_ObjectId}_{GetUserId()}");//私有
            _iCache.Remove($"config_queryviewspu_{entity.F_ObjectId}");//公有
            _iCache.Remove($"config_queryviewinfo_{entity.F_Id}");//配置信息
        }
        /// <summary>
        /// 删除查询视图-只能删除私有视图,且创建人是自己
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteQueryView(string keyValue)
        {
            var entity = await GetEntity(keyValue);
            if (entity != null)
            {
                if (entity.F_CreateUserId!=GetUserId())//创建人是自己方可删除
                {
                    throw new Exception("不可删除非本人创建的查询视图！");
                }
                await lrConfigTableService.Delete(keyValue);
                _iCache.Remove($"config_queryviewspr_{entity.F_ObjectId}_{GetUserId()}");//私有
                _iCache.Remove($"config_queryviewspu_{entity.F_ObjectId}");//公有
                _iCache.Remove($"config_queryviewinfo_{entity.F_Id}");//配置信息
            }
        }
        #endregion

        #endregion
    }
}