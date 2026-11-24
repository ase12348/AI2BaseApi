using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.09.21
    /// 描 述：智能体管理
    /// </summary>
    public class AgentController : BaseApiController
    {
        private readonly AgentIBLL _agentIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="agentIBLL"></param>
        public AgentController(AgentIBLL agentIBLL)
        {
            _agentIBLL = agentIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet("system/agents")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<AgentEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] string keyword)
        {
            var data = await _agentIBLL.GetList(keyword);
            return Success(data);
        }

        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("system/agent/{id}")]
        [ProducesResponseType(typeof(ResponseDto<AgentEntity>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var data = await _agentIBLL.GetEntity(id);
            return Success(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("system/agent")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(AgentEntity entity)
        {
            await _agentIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_Id);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("system/agent/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, AgentEntity entity)
        {
            await _agentIBLL.SaveEntity(id, entity);
            return Success("更新成功！", entity.F_Id);
        }

        /// <summary>
        /// 复制数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPost("system/agent/copy/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> CopyForm(string id)
        {
            var newId = await _agentIBLL.CopyEntity(id);
            return Success("复制成功！", newId);
        }

        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/agent/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _agentIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        #endregion
    }
}
