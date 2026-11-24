using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.SystemConfig
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-10-10 09:59:11
    /// 描 述： 个性化主题配置
    /// </summary>
    public class LrconfigthemeController : BaseApiController
    {
        private readonly ILrConfigThemeBLL _iLrConfigThemeBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrConfigThemeBLL">个性化主题配置【lr_config_theme】接口</param>
        public LrconfigthemeController(ILrConfigThemeBLL iLrConfigThemeBLL)
        {
            _iLrConfigThemeBLL = iLrConfigThemeBLL?? throw new ArgumentNullException(nameof(iLrConfigThemeBLL));
        }

        
        #region 获取数据
        
        /// <summary>
        /// 获取个性化主题配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigthemes")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrConfigThemeEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery]LrConfigThemeEntity queryParams)
        {
            var list = await _iLrConfigThemeBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取个性化主题配置的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigtheme/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrConfigThemeEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrConfigThemeEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrConfigThemeBLL.GetPageList(pagination,queryParams);
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
        [HttpGet("systemconfig/lrconfigtheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigThemeEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrConfigThemeBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单实体</param>
        /// <returns></returns>
        [HttpPost("systemconfig/lrconfigtheme")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigThemeEntity>), 200)]
        public async Task<IActionResult>AddForm(LrConfigThemeEntity entity)
        {
            await _iLrConfigThemeBLL.SaveEntity(null,entity);
            return Success("新增成功！",entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("systemconfig/lrconfigtheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrConfigThemeEntity entity)
        {
            await _iLrConfigThemeBLL.SaveEntity(id,entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除个性化主题配置【lr_config_theme】lr_config_theme数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrConfigThemeBLL.Delete(id);
            return Success("删除成功！");
        }


        /// <summary>
        /// 批量删除个性化主题配置【lr_config_theme】lr_config_theme数据
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtheme/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForms(string ids)
        {
            await _iLrConfigThemeBLL.Deletes(ids);
            return Success("删除成功！");
        }


        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigthemes/gettheme")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigThemeEntity>), 200)]
        public async Task<IActionResult> GetTheme()
        {
            var list = await _iLrConfigThemeBLL.GetTheme();
            return Success(list);
        }
        /// <summary>
        /// 保存当前登录人员的个性化主题（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("systemconfig/lrconfigthemes/savetheme")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveTheme(LrConfigThemeEntity entity)
        {
            await _iLrConfigThemeBLL.SaveTheme(entity);
            return Success("保存成功！");
        }
        /// <summary>
        /// 清除当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtheme/deletetheme")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteTheme()
        {
            await _iLrConfigThemeBLL.DeleteTheme();
            return Success("删除成功！");
        }
        #endregion
    }
}