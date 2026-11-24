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
    /// 西安项目-极简搭AI应用开发基座
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-03-28 16:23:13
    /// 描 述： 日志接口配置
    /// </summary>
    public class LogConfigController : BaseApiController
    {
        private readonly ILrOtherLogconfigBLL _iLrOtherLogconfigBLL;
        private readonly ILrOtherLogschemeBLL _iLrOtherLogschemeBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrOtherLogconfigBLL">日志接口配置【lr_other_logconfig】接口</param>
        /// <param name="iLrOtherLogschemeBLL">日志接口配置-详情【lr_other_logscheme】接口</param>
        public LogConfigController(ILrOtherLogconfigBLL iLrOtherLogconfigBLL, ILrOtherLogschemeBLL iLrOtherLogschemeBLL)
        {
            _iLrOtherLogconfigBLL = iLrOtherLogconfigBLL ?? throw new ArgumentNullException(nameof(iLrOtherLogconfigBLL));
            _iLrOtherLogschemeBLL = iLrOtherLogschemeBLL ?? throw new ArgumentNullException(nameof(iLrOtherLogschemeBLL));
        }


        #region 获取数据

        /// <summary>
        /// 获取日志接口配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/logConfigs")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrOtherLogconfigEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] LrOtherLogconfigEntity queryParams)
        {
            var list = await _iLrOtherLogconfigBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取日志接口配置的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/logConfig/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrOtherLogconfigEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LrOtherLogconfigEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrOtherLogconfigBLL.GetPageList(pagination, queryParams);
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
        [HttpGet("system/logConfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LogConfigDto>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var res = new LogConfigDto();
            res.LrOtherLogconfigEntity = await _iLrOtherLogconfigBLL.GetEntity(id);
            if (res.LrOtherLogconfigEntity != null)
            {
                res.LrOtherLogschemeList = await _iLrOtherLogschemeBLL.GetList(new LrOtherLogschemeEntity { F_LogConfigId = res.LrOtherLogconfigEntity.F_Id });
            }
            return Success(res);
        }
        /// <summary>
        /// 获取日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("system/logConfig/lrOtherLogconfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrOtherLogconfigEntity>), 200)]
        public async Task<IActionResult> GetLrOtherLogconfigEntity(string id)
        {
            var data = await _iLrOtherLogconfigBLL.GetEntity(id);
            return Success(data);
        }
        /// <summary>
        /// 获取日志接口配置-详情【lr_other_logscheme】lr_other_logscheme的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/logConfig/lrOtherLogschemes")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrOtherLogschemeEntity>>), 200)]
        public async Task<IActionResult> GetLrOtherLogschemeList([FromQuery] LrOtherLogschemeEntity queryParams)
        {
            var list = await _iLrOtherLogschemeBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取日志接口配置-详情【lr_other_logscheme】lr_other_logscheme的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/logConfig/lrOtherLogscheme/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrOtherLogschemeEntity>>), 200)]
        public async Task<IActionResult> GetLrOtherLogschemePageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LrOtherLogschemeEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrOtherLogschemeBLL.GetPageList(pagination, queryParams);
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
        /// 获取日志接口配置-详情【lr_other_logscheme】lr_other_logscheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("system/logConfig/lrOtherLogscheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrOtherLogschemeEntity>), 200)]
        public async Task<IActionResult> GetLrOtherLogschemeEntity(string id)
        {
            var data = await _iLrOtherLogschemeBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="dto">表单数据</param>
        /// <returns></returns>
        [HttpPost("system/logConfig")]
        [ProducesResponseType(typeof(ResponseDto<LrOtherLogconfigEntity>), 200)]
        public async Task<IActionResult> AddForm(LogConfigDto dto)
        {
            await _iLrOtherLogconfigBLL.SaveAll(null, dto);
            return Success("新增成功！", dto.LrOtherLogconfigEntity);
        }
        /// <summary>
        /// 新增日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("system/logConfig/lrOtherLogconfig")]
        [ProducesResponseType(typeof(ResponseDto<LrOtherLogconfigEntity>), 200)]
        public async Task<IActionResult> AddLrOtherLogconfig(LrOtherLogconfigEntity entity)
        {
            await _iLrOtherLogconfigBLL.SaveEntity(null, entity);
            return Success("新增成功！", entity);
        }
        /// <summary>
        /// 新增日志接口配置-详情【lr_other_logscheme】lr_other_logscheme数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("system/logConfig/lrOtherLogscheme")]
        [ProducesResponseType(typeof(ResponseDto<LrOtherLogschemeEntity>), 200)]
        public async Task<IActionResult> AddLrOtherLogscheme(LrOtherLogschemeEntity entity)
        {
            await _iLrOtherLogschemeBLL.SaveEntity(null, entity);
            return Success("新增成功！", entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        [HttpPut("system/logConfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, LogConfigDto dto)
        {
            await _iLrOtherLogconfigBLL.SaveAll(id, dto);
            return Success("更新成功！");
        }
        /// <summary>
        /// 更新日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPut("system/logConfig/lrOtherLogconfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateLrOtherLogconfig(string id, LrOtherLogconfigEntity entity)
        {
            await _iLrOtherLogconfigBLL.SaveEntity(id, entity);
            return Success("更新成功！");
        }
        /// <summary>
        /// 更新日志接口配置-详情【lr_other_logscheme】lr_other_logscheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPut("system/logConfig/lrOtherLogscheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateLrOtherLogscheme(string id, LrOtherLogschemeEntity entity)
        {
            await _iLrOtherLogschemeBLL.SaveEntity(id, entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iLrOtherLogconfigBLL.DeleteAll(id);
            return Success("删除成功！");
        }
        /// <summary>
        /// 删除日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/lrOtherLogconfig/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrOtherLogconfig(string id)
        {
            await _iLrOtherLogconfigBLL.Delete(id);
            return Success("删除成功！");
        }
        /// <summary>
        /// 删除日志接口配置-详情【lr_other_logscheme】lr_other_logscheme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/lrOtherLogscheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrOtherLogscheme(string id)
        {
            await _iLrOtherLogschemeBLL.Delete(id);
            return Success("删除成功！");
        }


        /// <summary>
        /// 批量删除日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForms(string ids)
        {
            await _iLrOtherLogconfigBLL.DeleteAlls(ids);
            return Success("删除成功！");
        }
        /// <summary>
        /// 批量删除日志接口配置【lr_other_logconfig】lr_other_logconfig数据
        /// </summary>
        /// <param name="ids">主键集</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/lrOtherLogconfig/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrOtherLogconfigs(string ids)
        {
            await _iLrOtherLogconfigBLL.Deletes(ids);
            return Success("删除成功！");
        }
        /// <summary>
        /// 批量删除日志接口配置-详情【lr_other_logscheme】lr_other_logscheme数据
        /// </summary>
        /// <param name="ids">主键集</param>
        /// <returns></returns>
        [HttpDelete("system/logConfig/lrOtherLogscheme/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrOtherLogschemes(string ids)
        {
            await _iLrOtherLogschemeBLL.Deletes(ids);
            return Success("删除成功！");
        }
        #endregion       
    }
}