using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;
using jcode.operat;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯消息内容
    /// </summary>
    public class IMMsgBLL : BLLBase, IMMsgIBLL, BLL
    {
        private readonly IMMsgService iMMsgService = new IMMsgService();

        #region 获取数据
        /// <summary>
        /// 获取最近的20条数据
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetLastList(string otherUserId) {
            return iMMsgService.GetLastList(otherUserId);
        }
        /// <summary>
        /// 获取列表数据(小于某个时间点的5条记录)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetListLessTime(string otherUserId, DateTime time)
        {
            return iMMsgService.GetListLessTime(otherUserId, time);
        }
        /// <summary>
        /// 获取列表数据(大于某个时间的所有数据)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetListBigTime(string otherUserId, DateTime time)
        {
            return iMMsgService.GetListBigTime(otherUserId, time);
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
            return iMMsgService.GetPageList(pagination, sendUserId, recvUserId, keyword);
        }


        /// <summary>
        /// 获取系统消息列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="myId"></param>
        /// <param name="sysCode"></param>
        /// <param name="keyword"></param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        public Task<IEnumerable<IMMsgEntity>> GetPageSysList(Pagination pagination, string myId, string sysCode, string keyword, int? isDelete) {
            return iMMsgService.GetPageSysList(pagination, myId, sysCode, keyword, isDelete);
        }

        /// <summary>
        /// 获取系统消息条数
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        public Task<int> GetPageSysNum(string myId) {
            return iMMsgService.GetPageSysNum(myId);
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
            await iMMsgService.DeleteEntity(keyValue);
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValues">主键集合</param>
        /// <returns></returns>
        public async Task DeleteEntitys(string keyValues)
        {
            var list = keyValues.Split(",");
            iMMsgService.BeginTrans();
            try
            {
                foreach (var keyValue in list)
                {
                    await iMMsgService.DeleteEntity(keyValue);
                }

                iMMsgService.Commit();
            }
            catch (Exception)
            {
                iMMsgService.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="userId">消息接收id</param>
        /// <returns></returns>
        public async Task VirtualDeleteAll(string userId)
        {
            await iMMsgService.VirtualDeleteAll(userId);
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="keyValues">主键集合</param>
        /// <returns></returns>
        public async Task VirtualDeletes(string keyValues)
        {
            var list = keyValues.Split(",");
            iMMsgService.BeginTrans();
            try
            {
                foreach (var keyValue in list)
                {
                    await iMMsgService.VirtualDelete(keyValue);
                }

                iMMsgService.Commit();
            }
            catch (Exception)
            {
                iMMsgService.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="contentId">内容主键</param>
        /// <returns></returns>
        public async Task VirtualDeleteByContentId(string contentId)
        {
            await iMMsgService.VirtualDeleteByContentId(contentId);
            await SendHubs.callMethod("SendMsg", "", this.GetUserId(), "", "", 1);
        }



        /// <summary>
        /// 确认阅读
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task ReadMsg(string keyValue) {
            await iMMsgService.ReadMsg(keyValue);
        }


        /// <summary>
        /// 保存实体数据（新增）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(IMMsgEntity entity)
        {
            await iMMsgService.SaveEntity(entity);
        }
        #endregion
        
        #region 扩展方法
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="userIdList">用户列表</param>
        /// <param name="content">消息内容</param>
        /// <param name="messageType">消息类型1.短信 2.邮箱 3.微信 4.IM（站内消息）</param>
        /// <param name="contentId">消息内容id</param>
        public async Task SendMsg(string code, IEnumerable<string> userIdList, string content,string messageType,string contentId = "")
        {
            try
            {
                if (!string.IsNullOrEmpty(content) && userIdList != null)
                {
                    foreach (var userId in userIdList)
                    {
                        UserEntity userInfo = null;
                        //短信
                        try
                        {
                            if (messageType.IndexOf("1") != -1)//短信
                            {
                                if (userInfo == null)
                                {
                                    userInfo = await this.CurrentUser(userId);
                                }
                                if (!string.IsNullOrEmpty(userInfo.F_Telephone))
                                {
                                    // 优先用「系统配置-发送短信」里保存的配置；还没有配置时保持原有 appsettings 行为
                                    var smsBLL = IocManager.Instance.GetService<ISystemConfigBLL>();
                                    var smsConfig = smsBLL == null ? null : await smsBLL.GetSmsConfig();
                                    if (smsConfig == null)
                                    {
                                        // 兼容旧逻辑：模板名称和参数需要按短信平台后台的设置自行调整
                                        SmsHelper.SendSmsByAli(userInfo.F_Telephone, "", "");
                                    }
                                    else if (smsConfig.IsEnabled == 1)
                                    {
                                        SmsHelper.SendSms(userInfo.F_Telephone, content, smsConfig.ToSmsOptions());
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                            LogEntity logEntity = new LogEntity();
                            logEntity.F_CategoryId = 4;
                            logEntity.F_ExecuteResult = -1;
                            logEntity.F_ExecuteResultJson ="【发送消息】" + logIBLL.ExceptionFormat(ex);
                            logIBLL.Write(logEntity, null);
                        }
                        //邮箱
                        try
                        {

                            if (messageType.IndexOf("2") != -1)//邮箱
                            {
                                if (userInfo == null)
                                {
                                    userInfo = await this.CurrentUser(userId);
                                }

                                if (!string.IsNullOrEmpty(userInfo.F_Email))
                                {
                                    await MailHelper.Send(userInfo.F_Email, "系统消息", content.Replace("-", ""));
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                            LogEntity logEntity = new LogEntity();
                            logEntity.F_CategoryId = 4;
                            logEntity.F_ExecuteResult = -1;
                            logEntity.F_ExecuteResultJson ="【发送邮箱】" + logIBLL.ExceptionFormat(ex);
                            logIBLL.Write(logEntity,null);
                        }
                        // 站内消息
                        try
                        { 
                            if (messageType.IndexOf("4") != -1) // 站内消息
                            {
                                IMMsgEntity iMMsgEntity = new IMMsgEntity();
                                iMMsgEntity.F_SendUserId = code;
                                iMMsgEntity.F_RecvUserId = userId;
                                iMMsgEntity.F_Content = content;
                                iMMsgEntity.F_ContentId = contentId;
                                iMMsgEntity.F_IsSystem = 1;
                                iMMsgEntity.F_IsRead = 0;
                                iMMsgEntity.F_DeleteMark = 0;

                                iMMsgEntity.F_MsgId = Guid.NewGuid().ToString();
                                iMMsgEntity.F_CreateDate = DateTime.Now;
                                await iMMsgService.BaseRepository().Insert(iMMsgEntity);
                                await SendHubs.callMethod("SendMsg", code, userId, content, iMMsgEntity.F_MsgId, 1);
                            }
                        }
                        catch (Exception ex)
                        {
                            var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                            LogEntity logEntity = new LogEntity();
                            logEntity.F_CategoryId = 4;
                            logEntity.F_ExecuteResult = -1;
                            logEntity.F_ExecuteResultJson ="【发送站内消息】" + logIBLL.ExceptionFormat(ex);
                            logIBLL.Write(logEntity, null);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = -1;
                logEntity.F_ExecuteResultJson = "【发送消息】" + logIBLL.ExceptionFormat(ex);
                logIBLL.Write(logEntity,null);
            }

        }
        #endregion
    }
}
