using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：数据编码
    /// </summary>
    public class CodeRuleController : BaseApiController
    {
        private readonly CodeRuleIBLL _codeRuleIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="codeRuleIBLL"></param>
        public CodeRuleController(CodeRuleIBLL codeRuleIBLL)
        {
            _codeRuleIBLL = codeRuleIBLL;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">关键字</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        [HttpGet("data/coderule/page")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<CodeRuleEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword, string appId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _codeRuleIBLL.GetPageList(pagination, keyword, appId);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("data/coderules")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<CodeRuleEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _codeRuleIBLL.GetList();
            return Success(data);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("data/coderule")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(CodeRuleEntity entity)
        {
            await _codeRuleIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_RuleId);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("data/coderule/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,CodeRuleEntity entity)
        {
            await _codeRuleIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/coderule/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _codeRuleIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }

        /// <summary>
        /// 获取单据编码
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="formData">表单值</param>
        /// <returns></returns>
        [HttpGet("data/coderule/{code}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> GetEnCode(string code,string formData = "")
        {
            var data = await _codeRuleIBLL.GetBillCode(code,formData);
            return Success(data);
        }
    }
}
