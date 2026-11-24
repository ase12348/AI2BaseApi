using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 极简搭AI应用开发基座-OA模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：tobin
    /// 日 期： 2022-05-05 10:40:00
    /// 描 述： 简洁报表
    /// </summary>
    public class SchemeController : BaseApiController
    {
        private readonly ILrReportSchemeBLL _iLrReportSchemeBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrReportSchemeBLL">接口</param>
        public SchemeController(ILrReportSchemeBLL iLrReportSchemeBLL)
        {
            _iLrReportSchemeBLL = iLrReportSchemeBLL?? throw new ArgumentNullException(nameof(iLrReportSchemeBLL));
        }

        
        #region 获取数据
        
        /// <summary>
        /// 获取简洁报表的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("report/schemes")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrReportSchemeEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery]LrReportSchemeEntity queryParams)
        {
            var list = await _iLrReportSchemeBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取简洁报表的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("report/scheme/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrReportSchemeEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrReportSchemeEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrReportSchemeBLL.GetPageList(pagination,queryParams);
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
        [HttpGet("report/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrReportSchemeEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrReportSchemeBLL.GetEntity(id);
            return Success(data);
        }
        /// <summary>
        /// 获取lr_report_scheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("report/scheme/lrReportScheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrReportSchemeEntity>), 200)]
        public async Task<IActionResult>GetLrReportSchemeEntity(string id)
        {
            var data = await _iLrReportSchemeBLL.GetEntity(id);
            return Success(data);
        }

        /// <summary>
        /// 获取报表数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("report/scheme/data/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrReportSchemeEntity>), 200)]
        public async Task<IActionResult> GetData(string id)
        {
            var data = await _iLrReportSchemeBLL.GetData(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="dto">表单数据</param>
        /// <returns></returns>
        [HttpPost("report/scheme")]
        [ProducesResponseType(typeof(ResponseDto<LrReportSchemeEntity>), 200)]
        public async Task<IActionResult>AddForm(LrReportSchemeEntity dto)
        {
            await _iLrReportSchemeBLL.SaveEntity(null,dto);
            return Success("新增成功！",dto);
        }
        /// <summary>
        /// 新增lr_report_scheme数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("report/scheme/lrReportScheme")]
        [ProducesResponseType(typeof(ResponseDto<LrReportSchemeEntity>), 200)]
        public async Task<IActionResult>AddLrReportScheme(LrReportSchemeEntity entity)
        {
            await _iLrReportSchemeBLL.SaveEntity(null,entity);
            return Success("新增成功！", entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        [HttpPut("report/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrReportSchemeEntity dto)
        {
            await _iLrReportSchemeBLL.SaveEntity(id,dto);
            return Success("更新成功！");
        }
        /// <summary>
        /// 更新lr_report_scheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPut("report/scheme/lrReportScheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateLrReportScheme(string id,LrReportSchemeEntity entity)
        {
            await _iLrReportSchemeBLL.SaveEntity(id,entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除lr_report_scheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("report/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrReportSchemeBLL.Delete(id);
            return Success("删除成功！");
        }
        /// <summary>
        /// 删除lr_report_scheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("report/scheme/lrReportScheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteLrReportScheme(string id)
        {
            await _iLrReportSchemeBLL.Delete(id);
            return Success("删除成功！");
        }
        #endregion       
    }
}