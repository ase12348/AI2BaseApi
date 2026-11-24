using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯消息内容
    /// </summary>
    public interface IMMsgIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取最近的20条数据
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <returns></returns>
        Task<IEnumerable<IMMsgEntity>> GetLastList(string otherUserId);
        /// <summary>
        /// 获取列表数据(小于某个时间点的5条记录)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        Task<IEnumerable<IMMsgEntity>> GetListLessTime(string otherUserId, DateTime time);
        /// <summary>
        /// 获取列表数据(大于某个时间的所有数据)
        /// </summary>
        /// <param name="otherUserId">对方的ID</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        Task<IEnumerable<IMMsgEntity>> GetListBigTime(string otherUserId, DateTime time);
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="sendUserId"></param>
        /// <param name="recvUserId"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        Task<IEnumerable<IMMsgEntity>> GetPageList(Pagination pagination, string sendUserId, string recvUserId, string keyword);


        /// <summary>
        /// 获取系统消息列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="myId"></param>
        /// <param name="sysCode"></param>
        /// <param name="keyword"></param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        Task<IEnumerable<IMMsgEntity>> GetPageSysList(Pagination pagination, string myId, string sysCode, string keyword, int? isDelete);

        /// <summary>
        /// 获取系统消息条数
        /// </summary>
        /// <param name="myId"></param>
        /// <returns></returns>
        Task<int> GetPageSysNum(string myId);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteEntity(string keyValue);

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValues">主键集合</param>
        /// <returns></returns>
        Task DeleteEntitys(string keyValues);

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="userId">消息接收id</param>
        /// <returns></returns>
        Task VirtualDeleteAll(string userId);

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="keyValues">主键集合</param>
        /// <returns></returns>
        Task VirtualDeletes(string keyValues);

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="contentId">内容主键</param>
        /// <returns></returns>
        Task VirtualDeleteByContentId(string contentId);

        /// <summary>
        /// 删除数据(虚拟删除)
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task ReadMsg(string keyValue);

        /// <summary>
        /// 保存实体数据（新增）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(IMMsgEntity entity);
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
        Task SendMsg(string code, IEnumerable<string> userIdList, string content, string messageType, string contentId = "");
        #endregion
    }
}
