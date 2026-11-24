using jcode.application;
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
    /// 日 期： 2023-10-10 09:51:29
    /// 描 述： 数据列表配置-高级查询、表格列
    /// </summary>
    public class LrconfigtableController : BaseApiController
    {
        private readonly ILrConfigTableBLL _iLrConfigTableBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrConfigTableBLL">数据列表配置【lr_config_table】接口</param>
        public LrconfigtableController(ILrConfigTableBLL iLrConfigTableBLL)
        {
            _iLrConfigTableBLL = iLrConfigTableBLL?? throw new ArgumentNullException(nameof(iLrConfigTableBLL));
        }

        
        #region 获取数据
        
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigtables")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrConfigTableEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery]LrConfigTableEntity queryParams)
        {
            var list = await _iLrConfigTableBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取数据列表配置-高级查询、表格列的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigtable/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrConfigTableEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrConfigTableEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrConfigTableBLL.GetPageList(pagination,queryParams);
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
        [HttpGet("systemconfig/lrconfigtable/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigTableEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrConfigTableBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单实体</param>
        /// <returns></returns>
        [HttpPost("systemconfig/lrconfigtable")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigTableEntity>), 200)]
        public async Task<IActionResult>AddForm(LrConfigTableEntity entity)
        {
            await _iLrConfigTableBLL.SaveEntity(null,entity);
            return Success("新增成功！",entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("systemconfig/lrconfigtable/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrConfigTableEntity entity)
        {
            await _iLrConfigTableBLL.SaveEntity(id,entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除数据列表配置【lr_config_table】lr_config_table数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtable/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrConfigTableBLL.Delete(id);
            return Success("删除成功！");
        }


        /// <summary>
        /// 批量删除数据列表配置【lr_config_table】lr_config_table数据
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtable/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForms(string ids)
        {
            await _iLrConfigTableBLL.Deletes(ids);
            return Success("删除成功！");
        }


        #endregion

        #region 扩展方法

        #region 表格列
        /// <summary>
        /// 获取当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigtable/getthcol/{objectId}")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigTableEntity>), 200)]
        public async Task<IActionResult> GetThCol(string objectId)
        {
            var list = await _iLrConfigTableBLL.GetThCol(objectId);
            return Success(list);
        }
        /// <summary>
        /// 保存当前登录人员的个性化表格列（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("systemconfig/lrconfigtable/savethcol")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveThCol(LrConfigTableEntity entity)
        {
            await _iLrConfigTableBLL.SaveThCol(entity);
            return Success("保存成功！");
        }
        /// <summary>
        /// 清除当前登录人员的个性化表格列
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtable/deletethcol/{objectId}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteThCol(string objectId)
        {
            await _iLrConfigTableBLL.DeleteThCol(objectId);
            return Success("删除成功！");
        }
        #endregion

        #region 高级查询
        /// <summary>
        /// 获取当前人员满足条件的查询视图列表
        /// </summary>
        /// <param name="objectId">功能ID</param>
        /// <returns></returns>
        [HttpGet("systemconfig/lrconfigtable/getqueryviews/{objectId}")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrConfigTableEntity>>), 200)]
        public async Task<IActionResult> GetQueryViews(string objectId)
        {
            var list = await _iLrConfigTableBLL.GetQueryViews(objectId);
            return Success(list);
        }
        /// <summary>
        /// 新增查询视图
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPost("systemconfig/lrconfigtable/addqueryview")]
        [ProducesResponseType(typeof(ResponseDto<LrConfigTableEntity>), 200)]
        public async Task<IActionResult> AddQueryView(LrConfigTableEntity entity)
        {
            await _iLrConfigTableBLL.SaveQueryView(null,entity);
            return Success("保存成功！");
        }
        /// <summary>
        /// 编辑查询视图
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpPut("systemconfig/lrconfigtable/upqueryview/{keyValue}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpQueryView(string keyValue,LrConfigTableEntity entity)
        {
            await _iLrConfigTableBLL.SaveQueryView(keyValue,entity);
            return Success("保存成功！");
        }

        /// <summary>
        /// 删除查询视图
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        [HttpDelete("systemconfig/lrconfigtable/deletequeryview/{keyValue}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteQueryView(string keyValue)
        {
            await _iLrConfigTableBLL.DeleteQueryView(keyValue);
            return Success("删除成功！");
        }
        #endregion


        #endregion
    }
}