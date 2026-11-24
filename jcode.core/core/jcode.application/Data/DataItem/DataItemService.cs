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
    /// 描 述：数据字典管理服务类
    /// </summary>
    public class DataItemService : ServiceBase
    {
        #region 数据字典分类管理
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DataItemEntity>> GetClassifyList()
        {
            return this.BaseRepository().FindList<DataItemEntity>(t=>t.F_DeleteMark == 0, "F_ParentId,F_SortCode");
        }
        
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DataItemEntity>> GetClassifyListByPid(string pid)
        {
            return this.BaseRepository().FindList<DataItemEntity>(t=>t.F_ParentId == pid, "F_SortCode");
        }

        /// <summary>
        /// 通过编号获取字典分类实体
        /// </summary>
        /// <param name="itemCode">编码</param>
        /// <returns></returns>
        public Task<DataItemEntity> GetClassifyEntityByCode(string itemCode)
        {
            return this.BaseRepository().FindEntity<DataItemEntity>(t => t.F_DeleteMark == 0 && t.F_ItemCode == itemCode);
        }

        /// <summary>
        /// 通过编号获取字典分类实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DataItemEntity> GetClassifyEntityByKey(string keyValue)
        {
            return this.BaseRepository().FindEntity<DataItemEntity>(t => t.F_DeleteMark == 0 && t.F_ItemId == keyValue);
        }

        /// <summary>
        /// 删除分类数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteClassify(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<DataItemEntity>(keyValue);
                await db.Delete<DataItemDetailEntity>(t => t.F_ItemId == keyValue );
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存分类数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public async Task SaveClassifyEntity(string keyValue, DataItemEntity entity)
        {
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_ItemId = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                if (string.IsNullOrEmpty(entity.F_ParentId)) {
                    entity.F_ParentId = "0";
                }
                entity.F_DeleteMark = 0;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_ItemId = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
                // 更新实体是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(entity.F_AppId))
                {
                    var old = await this.BaseRepository().FindEntity<DataItemEntity>(t => t.F_ItemId == keyValue);
                    entity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion

        #region 数据字典明细
        /// <summary>
        /// 获取数据字典明显根据分类编号
        /// </summary>
        /// <param name="itemCode">分类编号</param>
        /// <param name="appId">所属应用主键：传值时只取本应用的明细加全局明细，为空时不过滤</param>
        /// <returns></returns>
        public Task<IEnumerable<DataItemDetailEntity>> GetDetailList(string itemCode, string appId = null)
        {
            return this.BaseRepository().FindListByQueryable<DataItemDetailEntity>(q => {
                var queryable = q
                .InnerJoin<DataItemEntity>((t, t1) => t.F_ItemId == t1.F_ItemId)
                .Where("t1.F_ItemCode = @itemCode", new { itemCode });
                // 带应用口径时只取本应用的明细加全局明细（F_AppId 为空的行属于全局字典）
                if (!string.IsNullOrEmpty(appId))
                {
                    queryable = queryable.Where(
                        "(t.F_AppId IS NULL OR t.F_AppId = '' OR t.F_AppId = @appId)",
                        new { appId });
                }
                queryable = queryable.OrderBy("t.F_SortCode");
                return queryable.Select((t,t1)=>t);
            });
        }
        /// <summary>
        /// 获取数据字典明细实体类
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DataItemDetailEntity> GetDetailEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity<DataItemDetailEntity>(keyValue);
        }
        /// <summary>
        /// 删除明细数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteDetail(string keyValue)
        {
            await this.BaseRepository().Delete<DataItemDetailEntity>(keyValue);
        }
        /// <summary>
        /// 保存明细数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public async Task SaveDetailEntity(string keyValue, DataItemDetailEntity entity)
        {
            entity.F_QuickQuery = Str.ConvertPinYin(entity.F_ItemName).ToUpper();
            entity.F_SimpleSpelling = Str.PinYin(entity.F_ItemName);
            if (string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                if (string.IsNullOrEmpty(entity.F_ParentId))
                {
                    entity.F_ParentId = "0";
                }
                entity.F_DeleteMark = 0;
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                // 更新实体是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(entity.F_AppId))
                {
                    var old = await this.BaseRepository().FindEntity<DataItemDetailEntity>(t => t.F_Id == keyValue);
                    entity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(entity);
            }
        }
        
        /// <summary>
        /// 保存明细数据实体
        /// </summary>
        /// <param name="list">实体</param>
        public async Task SaveDetailList(List<DataItemDetailEntity> list)
        {
            await this.BaseRepository().Inserts(list);
        }
        #endregion
    }
}
