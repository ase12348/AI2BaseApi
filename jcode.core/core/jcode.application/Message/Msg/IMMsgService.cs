using jcode.iapplication;
using jcode.util;
using SqlSugar;
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
    /// 描 述：即时通讯消息内容
    /// </summary>
    public class IMMsgService : ServiceBase
    {
        private readonly IMContactsService iMContactsService = new IMContactsService();
        
        #region 获取数据
        /// <summary>
        /// 获取最近的20条数据
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetLastList(string otherUserId)
        {
            var myUserId = this.GetUserId();
            
            Pagination pagination = new Pagination();
            pagination.page = 1;
            pagination.rows = 20;
            pagination.sidx = "F_CreateDate DESC";

            return this.BaseRepository().FindList<IMMsgEntity>(t=>(t.F_SendUserId == myUserId && t.F_RecvUserId == otherUserId) 
            || (t.F_SendUserId == otherUserId && t.F_RecvUserId == myUserId),  pagination);
        }

        /// <summary>
        /// 获取列表数据(小于某个时间点的5条记录)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetListLessTime(string otherUserId, DateTime time)
        {
            var myUserId = this.GetUserId();
            
            Pagination pagination = new Pagination();
            pagination.page = 1;
            pagination.rows = 5;
            pagination.sidx = "F_CreateDate DESC";

            return this.BaseRepository().FindList<IMMsgEntity>(t => ((t.F_SendUserId == myUserId && t.F_RecvUserId == otherUserId)
           || (t.F_SendUserId == otherUserId && t.F_RecvUserId == myUserId))
           && t.F_CreateDate <= time
           , pagination);
        }
        /// <summary>
        /// 获取列表数据(大于某个时间的所有数据)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetListBigTime(string otherUserId, DateTime time)
        {
            var myUserId = this.GetUserId();
            time = time.AddSeconds(1);
            
            return this.BaseRepository().FindList<IMMsgEntity>(t => ((t.F_SendUserId == myUserId && t.F_RecvUserId == otherUserId)
              || (t.F_SendUserId == otherUserId && t.F_RecvUserId == myUserId))
              && t.F_CreateDate >= time
            ,"F_CreateDate");
        }

        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="sendUserId"></param>
        /// <param name="recvUserId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetPageList(Pagination pagination, string sendUserId, string recvUserId, string keyword)
        {
            var exp = Expressionable.Create<IMMsgEntity>()
                .And(t => (t.F_SendUserId == sendUserId && t.F_RecvUserId == recvUserId)
                || (t.F_SendUserId == recvUserId && t.F_RecvUserId == sendUserId))
                .AndIF(!string.IsNullOrEmpty(keyword),t=>t.F_Content.Contains(keyword)).ToExpression();
            
            return this.BaseRepository().FindList(exp, pagination);
        }

        /// <summary>
        /// 获取系统消息列表
        /// </summary>
        /// <param name="pagination"></param>
        /// <param name="myId"></param>
        /// <param name="sysCode"></param>
        /// <param name="keyword"></param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetPageSysList(Pagination pagination,string myId,string sysCode,string keyword,int? isDelete) {
            var expression = LinqExtensions.True<IMMsgEntity>();
            expression = expression.And(t => t.F_IsSystem == 1);
            if (!string.IsNullOrEmpty(myId))
            {
                expression = expression.And(t => t.F_RecvUserId == myId);
            }

            if (!string.IsNullOrEmpty(sysCode))
            {
                expression = expression.And(t => t.F_SendUserId==sysCode);
            }

            if (!string.IsNullOrEmpty(keyword)){
                expression = expression.And(t => t.F_Content.Contains(keyword));
            }
            if (isDelete != null) {
                expression = expression.And(t => t.F_DeleteMark == isDelete);
            }
            


            return this.BaseRepository().FindList(expression, pagination);
        }


        /// <summary>
        /// 获取系统消息条数
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        public async Task<int> GetPageSysNum(string myId)
        {
            var expression = LinqExtensions.True<IMMsgEntity>();
            expression = expression.And(t => t.F_IsSystem == 1);
            if (!string.IsNullOrEmpty(myId))
            {
                expression = expression.And(t => t.F_RecvUserId == myId);
            }

            expression = expression.And(t => t.F_DeleteMark == 0);
            expression = expression.And(t => t.F_IsRead == 0);
            Pagination pagination = new Pagination() {
                rows = 0,
                page = 1
            };


            var list  =await this.BaseRepository().FindList(expression, pagination);

            return (int)pagination.records;
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
            await this.BaseRepository().Delete<IMMsgEntity>(keyValue);
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="userId">消息接收id</param>
        /// <returns></returns>
        public async Task VirtualDeleteAll(string userId)
        {
            await this.BaseRepository().Update<IMMsgEntity>(new { F_DeleteMark = 1 },t=>t.F_IsSystem == 1 && t.F_RecvUserId == userId);
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task VirtualDelete(string keyValue)
        {
            await this.BaseRepository().Update<IMMsgEntity>(new { F_DeleteMark = 1 }, t => t.F_MsgId == keyValue);
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="contentId">内容主键</param>
        /// <returns></returns>
        public async Task VirtualDeleteByContentId(string contentId)
        {
            await this.BaseRepository().Update<IMMsgEntity>(new { F_DeleteMark = 1 }, t => t.F_ContentId == contentId && t.F_RecvUserId == this.GetUserId());
        }


        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task ReadMsg(string keyValue)
        {
            await this.BaseRepository().Update<IMMsgEntity>(new { F_IsRead = 1 }, t => t.F_MsgId == keyValue);
        }




        /// <summary>
        /// 保存实体数据（新增）
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(IMMsgEntity entity)
        {

            IMContactsEntity myContacts = new IMContactsEntity();
            IMContactsEntity otherContacts = new IMContactsEntity();

            myContacts.F_MyUserId = entity.F_SendUserId;
            myContacts.F_OtherUserId = entity.F_RecvUserId;
            myContacts.F_Content = entity.F_Content;

            otherContacts.F_MyUserId = entity.F_RecvUserId;
            otherContacts.F_OtherUserId = entity.F_SendUserId;
            otherContacts.F_Content = entity.F_Content;

            var myContactsTmp = await iMContactsService.GetEntity(myContacts.F_MyUserId, myContacts.F_OtherUserId);
            var otherContactsTmp = await iMContactsService.GetEntity(otherContacts.F_MyUserId, otherContacts.F_OtherUserId);

            var nowDateTime = DateTime.Now;

            var db = this.BaseRepository().BeginTrans();
            try
            {
                myContacts.F_NoReadNum = 0;
                myContacts.F_Time = nowDateTime;
                if (myContactsTmp == null)
                {
                    myContacts.F_Id = Guid.NewGuid().ToString();
                    await db.Insert(myContacts);
                }
                else
                {
                    myContacts.F_Id = myContactsTmp.F_Id;
                    await db.Update(myContacts);
                }


                
                otherContacts.F_NoReadNum = 1;
                otherContacts.F_Time = nowDateTime;
                if (otherContactsTmp == null)
                {
                    otherContacts.F_Id = Guid.NewGuid().ToString();
                    await db.Insert(otherContacts);
                }
                else
                {
                    otherContacts.F_NoReadNum += otherContactsTmp.F_NoReadNum;
                    otherContacts.F_Id = otherContactsTmp.F_Id;
                    await db.Update(otherContacts);
                }


                entity.F_MsgId = Guid.NewGuid().ToString();

                entity.F_CreateDate = nowDateTime;
                entity.F_IsSystem = 0;
                entity.F_DeleteMark = 0;

                await db.Insert(entity);

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        #endregion
    }
}
