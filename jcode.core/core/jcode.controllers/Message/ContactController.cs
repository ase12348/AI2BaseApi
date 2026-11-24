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
    /// 描 述：联系人列表
    /// </summary>
    public class ContactController: BaseApiController
    {
        private readonly IMContactsIBLL _iMContactsIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iMContactsIBLL"></param>
        public ContactController(IMContactsIBLL iMContactsIBLL)
        {
            _iMContactsIBLL = iMContactsIBLL;
        }

        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("message/contacts")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMContactsEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _iMContactsIBLL.GetList(this.GetUserId());
            return Success(data);
        }


        /// <summary>
        /// 获取联系人列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("message/contacts/{time}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMContactsEntity>>), 200)]
        public async Task<IActionResult> GetListByTime(DateTime time)
        {
            var data = await _iMContactsIBLL.GetListByTime(this.GetUserId(), time);
            return Success(data);
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("message/contact/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iMContactsIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="list">数据</param>
        /// <returns></returns>
        [HttpPost("message/contacts")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(List<IMContactsEntity> list)
        {
            await _iMContactsIBLL.SaveEntity(list);
            return SuccessInfo("添加成功！");
        }

        /// <summary>
        /// 更新记录读取状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("message/contact/state/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateState(string id)
        {
            await _iMContactsIBLL.UpdateState(id);
            return SuccessInfo("更新成功！");
        }
    }
}
