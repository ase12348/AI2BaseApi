using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.iapplication;

namespace jcode.controllers.WordPrint
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2023-05-06 14:06:35
    /// 描 述： word打印模版设置
    /// </summary>
    public class TemplateController : BaseApiController
    {
        private readonly ILrPrintWordBLL _iLrPrintWordBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrPrintWordBLL">打印模版表接口</param>
        public TemplateController(ILrPrintWordBLL iLrPrintWordBLL)
        {
            _iLrPrintWordBLL = iLrPrintWordBLL?? throw new ArgumentNullException(nameof(iLrPrintWordBLL));
        }
        #region 获取数据
        
        /// <summary>
        /// 获取word打印模版设置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("wordPrint/templates")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrPrintWordEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery]LrPrintWordEntity queryParams)
        {
            var list = await _iLrPrintWordBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取word打印模版设置的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        [HttpGet("wordPrint/template/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrPrintWordEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrPrintWordEntity queryParams, string tags)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrPrintWordBLL.GetPageList(pagination,queryParams, tags);
            var jsonData = new
            {
                rows = list,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("wordPrint/template/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrPrintWordEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrPrintWordBLL.GetEntity(id);
            return Success(data);
        }
        
        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        [HttpGet("wordPrint/template/code/{code}")]
        [ProducesResponseType(typeof(ResponseDto<LrPrintWordEntity>), 200)]
        public async Task<IActionResult>GetFormByCode(string code)
        {
            var data = await _iLrPrintWordBLL.GetEntityByCode(code);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("wordPrint/template")]
        [ProducesResponseType(typeof(ResponseDto<LrPrintWordEntity>), 200)]
        public async Task<IActionResult>AddForm(LrPrintWordEntity entity)
        {
            await _iLrPrintWordBLL.SaveEntity(null,entity);
            return Success("新增成功！", entity);
        }
        
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据集合</param>
        /// <returns></returns>
        [HttpPut("wordPrint/template/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrPrintWordEntity entity)
        {
            await _iLrPrintWordBLL.SaveEntity(id,entity);
            return SuccessInfo("更新成功！");
        }
        
        /// <summary>
        /// 删除打印模版表lr_print_word数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("wordPrint/template/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrPrintWordBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }
        #endregion
    }
}