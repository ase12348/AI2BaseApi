using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：最近联系人列表
    /// </summary>
    public class IMContactsBLL : BLLBase, IMContactsIBLL, BLL
    {
        private readonly IMContactsService iMContactsService = new IMContactsService();

        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <returns></returns>
        public Task<IEnumerable<IMContactsEntity>> GetList(string userId)
        {
            return iMContactsService.GetList(userId);
        }

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="time">最近一次消息时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMContactsEntity>> GetListByTime(string userId, DateTime time)
        {
            return iMContactsService.GetListByTime(userId, time);
        }

        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="userId">发送人</param>
        /// <param name="otherUserId">接收人</param>
        /// <returns></returns>
        public Task<IMContactsEntity> GetEntity(string userId, string otherUserId)
        {
            return iMContactsService.GetEntity(userId, otherUserId);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(IMContactsEntity entity)
        {
            await iMContactsService.SaveEntity(entity);
        }

        /// <summary>
        /// 新增联系人
        /// </summary>
        /// <param name="list">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(List<IMContactsEntity> list) {
            await iMContactsService.SaveEntity(list);
        }


        /// <summary>
        /// 更新记录读取状态
        /// </summary>
        /// <param name="myUserId">自己本身用户ID</param>
        /// <param name="otherUserId">对方用户ID</param>
        public async Task UpdateState(string myUserId, string otherUserId)
        {
            await iMContactsService.UpdateState(myUserId, otherUserId);
        }


        /// <summary>
        /// 更新记录读取状态
        /// </summary>
        /// <param name="id">id</param>
        public async Task UpdateState(string id)
        {
            await iMContactsService.UpdateState(id);
        }

        /// <summary>
        /// 删除最近联系人
        /// </summary>
        /// <param name="id">主键</param>
        public async Task DeleteEntity(string id)
        {
            await iMContactsService.DeleteEntity(id);
        }
        #endregion
    }
}
