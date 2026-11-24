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
    /// 极简搭AI应用开发基座项目-应用案例
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2022-05-05 11:14:02
    /// 描 述： 数据表模型常用字段维护
    /// </summary>
    public class DbFieldController : BaseApiController
    {
        private readonly DbFieldIBLL _iLrBaseDbfieldBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrBaseDbfieldBLL">建表常用字段接口</param>
        public DbFieldController(DbFieldIBLL iLrBaseDbfieldBLL)
        {
            _iLrBaseDbfieldBLL = iLrBaseDbfieldBLL ?? throw new ArgumentNullException(nameof(iLrBaseDbfieldBLL));
        }


        #region 获取数据

        /// <summary>
        /// 获取数据表模型常用字段维护的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("data/dbFields")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DbFieldEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] DbFieldEntity queryParams)
        {
            var list = await _iLrBaseDbfieldBLL.GetList(queryParams);
            return Success(list);
        }
        /// <summary>
        /// 获取数据表模型常用字段维护的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("data/dbField/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<DbFieldEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] DbFieldEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrBaseDbfieldBLL.GetPageList(pagination, queryParams);
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
        [HttpGet("data/dbField/{id}")]
        [ProducesResponseType(typeof(ResponseDto<DbFieldEntity>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            var data = await _iLrBaseDbfieldBLL.GetEntity(id);
            return Success(data);
        }
        /// <summary>
        /// 获取建表常用字段lr_base_dbfield数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("data/dbField/lrBaseDbfield/{id}")]
        [ProducesResponseType(typeof(ResponseDto<DbFieldEntity>), 200)]
        public async Task<IActionResult> GetLrBaseDbfieldEntity(string id)
        {
            var data = await _iLrBaseDbfieldBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="dto">表单数据</param>
        /// <returns></returns>
        [HttpPost("data/dbField")]
        [ProducesResponseType(typeof(ResponseDto<DbFieldEntity>), 200)]
        public async Task<IActionResult> AddForm(DbFieldEntity dto)
        {
            await _iLrBaseDbfieldBLL.SaveEntity(null, dto);
            return Success("新增成功！", dto);
        }
        /// <summary>
        /// 新增建表常用字段lr_base_dbfield数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("data/dbField/lrBaseDbfield")]
        [ProducesResponseType(typeof(ResponseDto<DbFieldEntity>), 200)]
        public async Task<IActionResult> AddLrBaseDbfield(DbFieldEntity entity)
        {
            await _iLrBaseDbfieldBLL.SaveEntity(null, entity);
            return Success("新增成功！", entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        [HttpPut("data/dbField/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, DbFieldEntity dto)
        {
            await _iLrBaseDbfieldBLL.SaveEntity(id, dto);
            return Success("更新成功！");
        }
        /// <summary>
        /// 更新建表常用字段lr_base_dbfield数据
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPut("data/dbField/lrBaseDbfield/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateLrBaseDbfield(string id, DbFieldEntity entity)
        {
            await _iLrBaseDbfieldBLL.SaveEntity(id, entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除建表常用字段lr_base_dbfield数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/dbField/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iLrBaseDbfieldBLL.Delete(id);
            return Success("删除成功！");
        }
        /// <summary>
        /// 删除建表常用字段lr_base_dbfield数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/dbField/lrBaseDbfield/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLrBaseDbfield(string id)
        {
            await _iLrBaseDbfieldBLL.Delete(id);
            return Success("删除成功！");
        }


        #endregion       
    }
}