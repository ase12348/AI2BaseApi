using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Message
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：即时通讯用户注册
    /// </summary>
    public class UserController : BaseApiController
    {
        private readonly IMSysUserIBLL _iMSysUserIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iMSysUserIBLL"></param>
        public UserController(IMSysUserIBLL iMSysUserIBLL) {
            _iMSysUserIBLL = iMSysUserIBLL;
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("message/users")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<IMSysUserEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _iMSysUserIBLL.GetList(string.Empty);
            return Success(data);
        }

        
        /// <summary>
        /// 新增im系统用户
        /// </summary>
        /// <param name="entity">数据</param>
        /// <returns></returns>
        [HttpPost("message/user")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(IMSysUserEntity entity)
        {
            await _iMSysUserIBLL.SaveEntity(string.Empty, entity);
            return Success("新增成功！", entity.F_Id);
        }

        /// <summary>
        /// 更新im系统用户
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据</param>
        /// <returns></returns>
        [HttpPut("message/user/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> UpdateForm(string id , IMSysUserEntity entity)
        {
            await _iMSysUserIBLL.SaveEntity(id, entity);
            return Success("编辑成功！");
        }

        /// <summary>
        /// 删除消息用户
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("message/user/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            var res = await _iMSysUserIBLL.DeleteEntity(id);
            if (res)
            {
                return SuccessInfo("删除成功！");
            }
            else {
                return Fail("有发送消息，不能删除！");
            }
        }
    }
}
