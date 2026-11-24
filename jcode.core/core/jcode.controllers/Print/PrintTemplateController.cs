using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Print
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板
    /// </summary>
    public class PrintTemplateController : BaseApiController
    {
        private readonly ILrPrintTemplateBLL _iLrPrintTemplateBll;
        private readonly ILrPrintTemplateinfoBLL _iLrPrintTemplateinfoBll;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrPrintTemplateBll">打印模板【lr_print_template】接口</param>
        /// <param name="iLrPrintTemplateinfoBll">打印模板信息【lr_print_templateinfo】接口</param>
        public PrintTemplateController(ILrPrintTemplateBLL iLrPrintTemplateBll, ILrPrintTemplateinfoBLL iLrPrintTemplateinfoBll)
        {
            _iLrPrintTemplateBll = iLrPrintTemplateBll ?? throw new ArgumentNullException(nameof(iLrPrintTemplateBll));
            _iLrPrintTemplateinfoBll = iLrPrintTemplateinfoBll ?? throw new ArgumentNullException(nameof(iLrPrintTemplateinfoBll));
        }
        
        #region 获取数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="isEnabled">是否获取正式表单</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrPrintTemplateinfoEntity>>), 200)]
        public async Task<IActionResult> GetTemplateinfoPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LrPrintTemplateinfoEntity queryParams, bool isEnabled = false, string tags = null)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrPrintTemplateinfoBll.GetPageList(pagination, queryParams, isEnabled, tags);
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
        /// 获取打印模板历史数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="templateInfoId">模板详情主键</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/history/page/{templateInfoId}")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<FormSchemeEntity>>), 200)]
        public async Task<IActionResult> GetTemplatePageList([FromQuery] PaginationInputDto paginationInputDto, string templateInfoId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _iLrPrintTemplateBll.GetPageList(pagination, new LrPrintTemplateEntity(){F_TemplateInfoId = templateInfoId});
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
        /// 获取打印模板历史版本设计
        /// </summary>
        /// <param name="templateId">id</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/history/{templateId}")]
        [ProducesResponseType(typeof(ResponseDto<PrintTemplateDto>), 200)]
        public async Task<IActionResult> GetHistoryForm(string templateId)
        {
            var res = new PrintTemplateDto();
            res.LrPrintTemplateEntity = await _iLrPrintTemplateBll.GetEntity(templateId);
            if (res.LrPrintTemplateEntity != null)
            {
                res.LrPrintTemplateinfoEntity = await _iLrPrintTemplateinfoBll.GetEntity(res.LrPrintTemplateEntity.F_TemplateInfoId);
            }
            return Success(res);
        }
        
        /// <summary>
        /// 获取表单数据--根据模板编码
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/{id}")]
        [ProducesResponseType(typeof(ResponseDto<PrintTemplateDto>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var res = new PrintTemplateDto();
            res.LrPrintTemplateinfoEntity = await _iLrPrintTemplateinfoBll.GetEntity(id);
            if (res.LrPrintTemplateinfoEntity != null)
            {
                res.LrPrintTemplateEntity = await _iLrPrintTemplateBll.GetEntity(res.LrPrintTemplateinfoEntity.F_TemplateId);
            }
            return Success(res);
        }
        
        /// <summary>
        /// 获取表单数据--根据模板编码
        /// </summary>
        /// <param name="code">主键</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/code/{code}")]
        [ProducesResponseType(typeof(ResponseDto<PrintTemplateDto>), 200)]
        public async Task<IActionResult> GetTemplateByCode(string code)
        {
            var res = new PrintTemplateDto();
            res.LrPrintTemplateinfoEntity = await _iLrPrintTemplateinfoBll.GetEntityByCode(code);
            if (res.LrPrintTemplateinfoEntity != null)
            {
                res.LrPrintTemplateEntity = await _iLrPrintTemplateBll.GetLastEntity(res.LrPrintTemplateinfoEntity.F_Id);
            }
            return Success(res);
        }
        /// <summary>
        /// 获取表单数据--根据模板编码
        /// </summary>
        /// <param name="code">主键</param>
        /// <returns></returns>
        [HttpGet("print/printTemplate/info/code/{code}")]
        [ProducesResponseType(typeof(ResponseDto<LrPrintTemplateinfoEntity>), 200)]
        public async Task<IActionResult> GetTemplateInfoByCode(string code)
        {
            var data = await _iLrPrintTemplateinfoBll.GetEntityByCode(code);
            return Success(data);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="dto">表单数据</param>
        /// <returns></returns>
        [HttpPost("print/printTemplate")]
        [ProducesResponseType(typeof(ResponseDto<LrPrintTemplateEntity>), 200)]
        public async Task<IActionResult> AddForm(PrintTemplateDto dto)
        {
            await _iLrPrintTemplateinfoBll.SaveAll(null, dto);
            return Success("新增成功！", dto.LrPrintTemplateEntity);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        [HttpPut("print/printTemplate/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, PrintTemplateDto dto)
        {
            await _iLrPrintTemplateinfoBll.SaveAll(id, dto);
            return SuccessInfo("更新成功！");
        }
        /// <summary>
        /// 删除打印模板
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("print/printTemplate/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iLrPrintTemplateinfoBll.DeleteAll(id);
            return SuccessInfo("删除成功！");
        }
        
        /// <summary>
        /// 删除打印模板【lr_print_template】lr_print_template数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("print/printTemplate/lrPrintTemplate/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrPrintTemplate(string id)
        {
            await _iLrPrintTemplateBll.Delete(id);
            return SuccessInfo("删除成功！");
        }
        
        /// <summary>
        /// 启用/停用模板
        /// </summary>
        /// <param name="templateInfoId">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        [HttpPut("print/printTemplate/state/{templateInfoId}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateState(string templateInfoId, int state)
        {
            await _iLrPrintTemplateinfoBll.UpdateState(templateInfoId, state);
            return SuccessInfo((state == 1 ? "启用" : "禁用") + "成功！");
        }
        
        #endregion
        
        #region 扩展方法
        /// <summary>
        /// 导出打印模板数据
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="id">流程主键</param>
        /// <returns></returns>
        [HttpPost("print/printTemplate/download/{path}/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> ExportTemplate(string path, string id)
        {
            var fileName = await _iLrPrintTemplateinfoBll.Export(id, path);
            return Success(fileName);
        }
        #endregion
    }
}