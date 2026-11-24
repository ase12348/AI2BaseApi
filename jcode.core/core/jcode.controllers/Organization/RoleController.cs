using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.operat;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.24
    /// 描 述：角色模块
    /// </summary>
    public class RoleController : BaseApiController
    {
        private readonly RoleIBLL _roleIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="roleIBLL"></param>
        public RoleController(RoleIBLL roleIBLL)
        {
            _roleIBLL = roleIBLL;
        }

        /// <summary>
        /// 获取角色列表信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("organization/roles")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<RoleEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _roleIBLL.GetList();
            return Success(data);
        }


        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="dto">主键</param>
        /// <returns></returns>
        [HttpPost("organization/roles")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<RoleEntity>>), 200)]
        public async Task<IActionResult> GetEntity(IDSDto dto)
        {
            var data = await _roleIBLL.GetListByRoleIds(dto.Ids);
            return Success(data);
        }

        /// <summary>
        /// 获取角色
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("organization/role/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _roleIBLL.GetEntity(id);
            return Success(data);
        }


        /// <summary>
        /// 新增角色
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("organization/role")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        [LogRecord("新增角色")]
        public async Task<IActionResult> SaveForm(RoleEntity entity)
        {
            await _roleIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_RoleId);
        }

        /// <summary>
        /// 更新角色
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("organization/role/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveForm(string id, RoleEntity entity)
        {
            await _roleIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }
        /// <summary>
        /// 删除角色
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/role/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _roleIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }
    }
}
