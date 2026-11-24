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
    /// 日 期：2026.09.20
    /// 描 述：模型管理
    /// </summary>
    public class AiModelController : BaseApiController
    {
        private readonly AiModelIBLL _aiModelIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="aiModelIBLL"></param>
        public AiModelController(AiModelIBLL aiModelIBLL)
        {
            _aiModelIBLL = aiModelIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyword">关键字</param>
        /// <returns></returns>
        [HttpGet("system/aimodels")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<AiModelEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] string keyword)
        {
            var data = await _aiModelIBLL.GetList(keyword);
            return Success(data);
        }

        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("system/aimodel/{id}")]
        [ProducesResponseType(typeof(ResponseDto<AiModelEntity>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var data = await _aiModelIBLL.GetEntity(id);
            return Success(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("system/aimodel")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(AiModelEntity entity)
        {
            await _aiModelIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_Id);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("system/aimodel/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, AiModelEntity entity)
        {
            await _aiModelIBLL.SaveEntity(id, entity);
            return Success("更新成功！", entity.F_Id);
        }

        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/aimodel/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _aiModelIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        #endregion
    }
}
