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
    /// 极简搭AI应用开发基座项目-图片模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2022-07-07 11:19:40
    /// 描 述： 图片管理
    /// </summary>
    public class ImgController : BaseApiController
    {
        private readonly ILrBaseImgBLL _iLrBaseImgBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrBaseImgBLL">极简搭图片存储表接口</param>
        public ImgController(ILrBaseImgBLL iLrBaseImgBLL)
        {
            _iLrBaseImgBLL = iLrBaseImgBLL ?? throw new ArgumentNullException(nameof(iLrBaseImgBLL));
        }


        #region 获取数据

        /// <summary>
        /// 获取图片管理的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/imgs")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrBaseImgEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] LrBaseImgEntity queryParams)
        {
            var list = await _iLrBaseImgBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取图片管理的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("system/img/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrBaseImgEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LrBaseImgEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrBaseImgBLL.GetPageList(pagination, queryParams);
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
        [HttpGet("system/img/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseImgEntity>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var data = await _iLrBaseImgBLL.GetEntity(id);
            return Success(data);
        }
        /// <summary>
        /// 获取极简搭图片存储表lr_base_img数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("system/img/lrBaseImg/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseImgEntity>), 200)]
        public async Task<IActionResult> GetLrBaseImgEntity(string id)
        {
            var data = await _iLrBaseImgBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="dto">表单数据</param>
        /// <returns></returns>
        [HttpPost("system/img")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseImgEntity>), 200)]
        public async Task<IActionResult> AddForm(LrBaseImgEntity dto)
        {
            await _iLrBaseImgBLL.SaveEntity(null, dto);
            return Success("新增成功！", dto);
        }
        /// <summary>
        /// 新增极简搭图片存储表lr_base_img数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("system/img/lrBaseImg")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseImgEntity>), 200)]
        public async Task<IActionResult> AddLrBaseImg(LrBaseImgEntity entity)
        {
            await _iLrBaseImgBLL.SaveEntity(null, entity);
            return Success("新增成功！", entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        [HttpPut("system/img/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, LrBaseImgEntity dto)
        {
            await _iLrBaseImgBLL.SaveEntity(id, dto);
            return Success("更新成功！");
        }
        /// <summary>
        /// 更新极简搭图片存储表lr_base_img数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPut("system/img/lrBaseImg/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateLrBaseImg(string id, LrBaseImgEntity entity)
        {
            await _iLrBaseImgBLL.SaveEntity(id, entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除极简搭图片存储表lr_base_img数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/img/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iLrBaseImgBLL.Delete(id);
            return Success("删除成功！");
        }
        /// <summary>
        /// 删除极简搭图片存储表lr_base_img数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("system/img/lrBaseImg/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrBaseImg(string id)
        {
            await _iLrBaseImgBLL.Delete(id);
            return Success("删除成功！");
        }


        #endregion


        #region 扩展方法

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="dto">文件信息</param>
        /// <param name="fileType">图片分类</param>
        /// <returns></returns>
        [HttpPost("system/img/upload/{fileType}")]
        public async Task<IActionResult> UploadImg(string fileType,AnnexesFileInputDto dto)
        {
            if (fileType == "jcode_img") {
                fileType = "";
            }
            var res =  await _iLrBaseImgBLL.UploadImg(dto, fileType);
            return Success("上传成功！", res);
        }

        #endregion
    }
}
