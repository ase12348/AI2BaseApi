using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.iapplication;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-05-07 11:16:00
    /// 描 述： 部门角色
    /// </summary>
    public class DeptRoleController : BaseApiController
    {
        private readonly ILrBaseDeptRoleBLL _iLrBaseDeptRoleBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iLrBaseDeptRoleBLL">部门角色表接口</param>
        public DeptRoleController(ILrBaseDeptRoleBLL iLrBaseDeptRoleBLL)
        {
            _iLrBaseDeptRoleBLL = iLrBaseDeptRoleBLL?? throw new ArgumentNullException(nameof(iLrBaseDeptRoleBLL));
        }

        
        #region 获取数据
        
        /// <summary>
        /// 获取部门角色的列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("organization/deptRoles")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<LrBaseDeptRoleEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var list = await _iLrBaseDeptRoleBLL.GetList();
            return Success(list);
        }
        /// <summary>
        /// 获取部门角色的分页列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("organization/deptRole/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<LrBaseDeptRoleEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]LrBaseDeptRoleEntity queryParams)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iLrBaseDeptRoleBLL.GetPageList(pagination,queryParams);
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
        [HttpGet("organization/deptRole/{id}")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseDeptRoleEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iLrBaseDeptRoleBLL.GetEntity(id);
            return Success(data);
        }


        #endregion

        #region 提交数据

        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单实体</param>
        /// <returns></returns>
        [HttpPost("organization/deptRole")]
        [ProducesResponseType(typeof(ResponseDto<LrBaseDeptRoleEntity>), 200)]
        public async Task<IActionResult>AddForm(LrBaseDeptRoleEntity entity)
        {
            await _iLrBaseDeptRoleBLL.SaveEntity(null,entity);
            return Success("新增成功！",entity);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        [HttpPut("organization/deptRole/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>UpdateForm(string id,LrBaseDeptRoleEntity entity)
        {
            await _iLrBaseDeptRoleBLL.SaveEntity(id,entity);
            return Success("更新成功！");
        }


        /// <summary>
        /// 删除部门角色表LrBaseDeptRole数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/deptRole/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForm(string id)
        {
            await _iLrBaseDeptRoleBLL.Delete(id);
            return Success("删除成功！");
        }


        /// <summary>
        /// 批量删除部门角色表LrBaseDeptRole数据
        /// </summary>
        /// <param name="ids">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/deptRole/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult>DeleteForms(string ids)
        {
            await _iLrBaseDeptRoleBLL.Deletes(ids);
            return Success("删除成功！");
        }


        #endregion       
    }
}