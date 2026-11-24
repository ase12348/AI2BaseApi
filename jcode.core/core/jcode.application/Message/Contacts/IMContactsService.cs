using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：最近联系人列表
    /// </summary>
    public class IMContactsService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public Task<IEnumerable<IMContactsEntity>> GetList(string userId)
        {
            return this.BaseRepository().FindList<IMContactsEntity>(t=>t.F_MyUserId == userId, "F_Time Desc");
        }
        
        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="time">最近一次消息时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMContactsEntity>> GetListByTime(string userId,DateTime time)
        {
            return this.BaseRepository().FindList<IMContactsEntity>(t=>t.F_MyUserId == userId && t.F_Time > time, "F_Time Desc");
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userId">发送人</param>
        /// <param name="otherUserId">接收人</param>
        /// <returns></returns>
        public Task<IMContactsEntity> GetEntity(string userId, string otherUserId)
        {
            return this.BaseRepository().FindEntity<IMContactsEntity>(t=>t.F_MyUserId == userId && t.F_OtherUserId == otherUserId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(IMContactsEntity entity)
        {
            var entityTmp = await GetEntity(entity.F_MyUserId, entity.F_OtherUserId);
            entity.F_NoReadNum = 2;
            if (entityTmp == null)
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_Time = DateTime.Now;
                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_Id = entityTmp.F_Id;
                entity.F_Time = DateTime.Now;
                await this.BaseRepository().Update(entity);
            }
        }


        /// <summary>
        /// 新增联系人
        /// </summary>
        /// <param name="list">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(List<IMContactsEntity> list)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                foreach (var item in list)
                {
                    var entityTmp =    await db.FindEntity<IMContactsEntity>(t=>t.F_MyUserId == item.F_MyUserId && t.F_OtherUserId == item.F_OtherUserId);
                    if (entityTmp == null)
                    {
                        item.F_Content = "";
                        item.F_NoReadNum = 0;
                        item.F_Id = Guid.NewGuid().ToString();
                        item.F_Time = DateTime.Now;
                        await db.Insert(item);
                    }
                    else
                    {
                        item.F_Id = entityTmp.F_Id;
                        item.F_Time = DateTime.Now;
                        await db.Update(item,true);
                    }
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
        /// 更新记录读取状态
        /// </summary>
        /// <param name="myUserId">自己本身用户ID</param>
        /// <param name="otherUserId">对方用户ID</param>
        public async Task UpdateState(string myUserId, string otherUserId)
        {
            var entity = await GetEntity(myUserId, otherUserId);
            if (entity != null)
            {
                entity.F_NoReadNum = 0;
                await this.BaseRepository().Update(entity,true);
            }
        }

        /// <summary>
        /// 更新记录读取状态
        /// </summary>
        /// <param name="id">id</param>
        public async Task UpdateState(string id)
        {
            await this.BaseRepository().Update<IMContactsEntity>(new {F_NoReadNum = 0},t=>t.F_OtherUserId == id);
        }


        /// <summary>
        /// 删除最近联系人
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteEntity(string id)
        {
            await this.BaseRepository().Delete<IMContactsEntity>(t=>t.F_Id == id);
        }
        #endregion
    }
}
