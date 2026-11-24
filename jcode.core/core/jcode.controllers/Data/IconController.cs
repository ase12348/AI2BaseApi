using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-03-01 10:15:24
    /// 描 述： 图标管理
    /// </summary>
    public class IconController : BaseApiController
    {
        private readonly ILrBaseIconsBLL _iLrBaseIconsBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrBaseIconsBLL">图标保存接口</param>
        public IconController(ILrBaseIconsBLL iLrBaseIconsBLL)
        {
            _iLrBaseIconsBLL = iLrBaseIconsBLL?? throw new ArgumentNullException(nameof(iLrBaseIconsBLL));
        }

        
        #region 获取数据
        
        /// <summary>
        /// 获取图标管理的列表数据
        /// </summary>
        /// <param name="ver">查询参数</param>
        /// <returns></returns>
        [HttpGet("data/icons")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrBaseIconsEntity>>), 200)]
        public async Task<IActionResult> GetALLList(string ver)
        {
            var list = await _iLrBaseIconsBLL.GetALLList(ver);
            return Success(list);
        }
        /// <summary>
        /// 获取图标管理的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("data/icon/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrBaseIconsEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrBaseIconsEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrBaseIconsBLL.GetPageList(pagination,queryParams);
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
        [HttpGet("data/icon/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseIconsEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrBaseIconsBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单实体</param>
        /// <returns></returns>
        [HttpPost("data/icon")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseIconsEntity>), 200)]
        public async Task<IActionResult>AddForm(LrBaseIconsEntity entity)
        {
            await _iLrBaseIconsBLL.SaveEntity(null,entity);
            return Success("新增成功！",entity);
        }
        
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("data/icon/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrBaseIconsEntity entity)
        {
            await _iLrBaseIconsBLL.SaveEntity(id,entity);
            return Success("更新成功！");
        }

        /// <summary>
        /// 删除图标保存lr_base_icons数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/icon/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrBaseIconsBLL.Delete(id);
            return Success("删除成功！");
        }

        /// <summary>
        /// 批量删除图标保存lr_base_icons数据
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpDelete("data/icon/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForms(string ids)
        {
            await _iLrBaseIconsBLL.Deletes(ids);
            return Success("删除成功！");
        }
        #endregion       
    }
}