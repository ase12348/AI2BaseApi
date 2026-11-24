using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.25
    /// 描 述：功能模块
    /// </summary>
    public class ModuleController : BaseApiController
    {
        private readonly ModuleIBLL _moduleIBLL;

        /// <summary>
        /// 构造注入
        /// </summary>
        /// <param name="moduleIBLL"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleController(
            ModuleIBLL moduleIBLL
            ) {
            _moduleIBLL = moduleIBLL ?? throw new ArgumentNullException(nameof(moduleIBLL));
        }
        
        /// <summary>
        /// 获取所有模块信息
        /// </summary>
        /// <param name="terminalType">终端类型 0电脑端 1移动端，不传返回全部</param>
        /// <returns></returns>
        [HttpGet("system/modules")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<ModuleEntity>>), 200)]
        public async Task<IActionResult> GetALLList(int? terminalType)
        {
            var list = await _moduleIBLL.GetModuleList();
            if (terminalType.HasValue)
            {
                list = list.Where(t => (t.F_TerminalType ?? 0) == terminalType.Value).ToList();
            }
            return Success(list);
        }



        /// <summary>
        /// 模块信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("system/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto<ModuleOutputDto>), 200)]
        public async Task<IActionResult> GetModule(string id)
        {
            ModuleOutputDto dto = new ModuleOutputDto();
            dto.BaseInfo = await _moduleIBLL.GetModuleEntity(id);
            return Success(dto);
        }

       



        /// <summary>
        /// 添加模块信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("system/module")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(ModuleInputDto dto)
        {
            await _moduleIBLL.SaveEntity(string.Empty, dto.BaseInfo);
            return SuccessInfo("新增成功");
        }

        /// <summary>
        /// 修改模块信息
        /// </summary>
        /// <returns></returns>
        [HttpPut("system/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateModule(string id, ModuleInputDto dto)
        {
            await _moduleIBLL.SaveEntity(id, dto.BaseInfo);
            return SuccessInfo("更新成功");
        }
        
        /// <summary>
        /// 更新菜单状态
        /// </summary>
        /// <returns></returns>
        [HttpPut("system/module/state/{id}/{state}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateModuleState(string id, int state)
        {
            await _moduleIBLL.UpdateState(id, state);
            return SuccessInfo("更新成功");
        }


        /// <summary>
        /// 删除模块信息
        /// </summary>
        /// <returns></returns>
        [HttpDelete("system/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _moduleIBLL.Delete(id);
            return SuccessInfo("删除成功");
        }

        #region Vue3 版本

        /// <summary>
        /// 模块信息
        /// </summary>
        /// <returns></returns>
        [HttpGet("system/module3/{id}")]
        [ProducesResponseType(typeof(ResponseDto<ModuleOutputDto>), 200)]
        public async Task<IActionResult> GetModule3(string id)
        { 
            var data = await _moduleIBLL.GetModuleEntity(id);
            return Success(data);
        }
        
        /// <summary>
        /// 添加模块信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("system/module3")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm3(ModuleEntity entity)
        {
            var id = await _moduleIBLL.SaveEntity(string.Empty, entity);
            return Success("新增成功",id);
        }

        /// <summary>
        /// 修改模块信息
        /// </summary>
        /// <returns></returns>
        [HttpPut("system/module3/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateModule3(string id, ModuleEntity entity)
        {
            await _moduleIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功");
        }

        #endregion
    }
}
