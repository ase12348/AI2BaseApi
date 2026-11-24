using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Message
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.07
    /// 描 述：消息列表
    /// </summary>
    public class MsgController: BaseApiController
    {
        private readonly IMMsgIBLL _iMMsgIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iMMsgIBLL"></param>
        public MsgController(IMMsgIBLL iMMsgIBLL)
        {
            _iMMsgIBLL = iMMsgIBLL;
        }


        /// <summary>
        /// 获取消息分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="sendId">发送者</param>
        /// <param name="toId">接收者</param>
        /// <param name="keyword">查询关键字</param>
        /// <returns></returns>
        [HttpGet("message/msg/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<IMMsgEntity>>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string sendId, string toId, string keyword)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _iMMsgIBLL.GetPageList(pagination, sendId, toId, keyword);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取系统消息分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询关键字</param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        [HttpGet("message/msg/sys/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<IMMsgEntity>>>), 200)]
        public async Task<IActionResult> GetPageSysList([FromQuery] PaginationInputDto paginationInputDto, string keyword,int? isDelete)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _iMMsgIBLL.GetPageSysList(pagination, this.GetUserId(), "", keyword, isDelete);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
            };
            return Success(jsonData);
        }


        /// <summary>
        /// 获取系统消息条数
        /// </summary>
        /// <returns></returns>
        [HttpGet("message/msg/sys/num")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> GetPageSysNum()
        {
            var data = await _iMMsgIBLL.GetPageSysNum(this.GetUserId());
            return Success(data);
        }

        /// <summary>
        /// 获取列表数据(小于某个时间点的5条记录)
        /// </summary>
        /// <param name="toId">聊天人id</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        [HttpGet("message/msg/list/less")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMMsgEntity>>), 200)]
        public async Task<IActionResult> GetListLessTime(string toId, DateTime time)
        {
            var data = await _iMMsgIBLL.GetListLessTime(toId, time);
            return Success(data);
        }


        /// <summary>
        /// 获取列表数据(大于某个时间的所有数据)
        /// </summary>
        /// <param name="toId">聊天人id</param>
        /// <param name="time">时间</param>
        /// <returns></returns>
        [HttpGet("message/msg/list/big")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMMsgEntity>>), 200)]
        public async Task<IActionResult> GetListBigTime(string toId, DateTime time)
        {
            var data = await _iMMsgIBLL.GetListBigTime(toId, time);
            return Success(data);
        }



        /// <summary>
        /// 获取列表数据(大于某个时间的所有数据)
        /// </summary>
        /// <param name="toId">聊天人id</param>
        /// <returns></returns>
        [HttpGet("message/msg/list/last")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMMsgEntity>>), 200)]
        public async Task<IActionResult> GetListBigTime(string toId)
        {
            var data = await _iMMsgIBLL.GetLastList(toId);
            return Success(data);
        }

        /// <summary>
        /// 添加消息
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("message/msg")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(IMMsgEntity entity)
        {
            await _iMMsgIBLL.SaveEntity(entity);
            return Success("添加成功！", entity.F_MsgId);
        }


        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("message/msg/send")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SendMsg(IMMsgEntity entity)
        {
            entity.F_SendUserId = this.GetUserId();
            await _iMMsgIBLL.SaveEntity(entity);
            // 向即时消息服务器发送一条信息
            await SendHubs.callMethod("SendMsg", entity.F_SendUserId, entity.F_RecvUserId, entity.F_Content, entity.F_MsgId,0);
            return Success("添加成功！", entity.F_MsgId);
        }


        /// <summary>
        /// 清空消息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("message/msg/clear")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> VirtualDeleteAll()
        {
            var userId = this.GetUserId();
            await _iMMsgIBLL.VirtualDeleteAll(userId);
            return Success("清空成功！");
        }

        /// <summary>
        /// 虚拟删除
        /// </summary>
        /// <returns></returns>
        [HttpDelete("message/msg/virtual")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> VirtualDelete(string ids)
        {
            await _iMMsgIBLL.VirtualDeletes(ids);
            return Success("删除成功！");
        }

        /// <summary>
        /// 彻底删除
        /// </summary>
        /// <returns></returns>
        [HttpDelete("message/msg")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Delete(string ids)
        {
            await _iMMsgIBLL.DeleteEntitys(ids);
            return Success("删除成功！");
        }

        /// <summary>
        /// 阅读消息
        /// </summary>
        /// <returns></returns>
        [HttpPut("message/msg/read/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ReadMsg(string id)
        {
            await _iMMsgIBLL.ReadMsg(id);
            return Success("阅读成功！");
        }



    }
}
