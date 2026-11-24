using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentController : BaseApiController
    {
        private readonly DepartmentIBLL _departmentIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="departmentIBLL"></param>
        public DepartmentController(DepartmentIBLL departmentIBLL)
        {
            _departmentIBLL = departmentIBLL;
        }

        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <param name="parentId">部门上级Id</param>
        /// <param name="type">部门类型</param>
        /// <param name="keyword">查询条件</param>
        /// <param name="isSyn">是否查询企业微信、钉钉同步状态(前端传true才查询)</param>
        /// <returns></returns>
        [HttpGet("organization/departments/{companyId}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DepartmentEntity>>), 200)]
        public async Task<IActionResult> GetList(string companyId,string parentId, string type,string keyword, bool isSyn = false)
        {
            var data = await _departmentIBLL.GetList(companyId,parentId,type, keyword);
            return Success(data);
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        [HttpGet("organization/department/{id}")]
        [ProducesResponseType(typeof(ResponseDto<DepartmentEntity>), 200)]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _departmentIBLL.GetEntity(id);
            return Success(data);
        }


        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="dto">部门ids</param>
        /// <returns></returns>
        [HttpPost("organization/departments")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DepartmentEntity>>), 200)]
        public async Task<IActionResult> GetEntitys(IDSDto dto)
        {
            var data = await _departmentIBLL.GetListByKeys(dto.Ids.Split(','));
            return Success(data);
        }
        
        
        /// <summary>
        /// 获取完整部门链名称
        /// </summary>
        /// <param name="id">部门ids</param>
        /// <returns></returns>
        [HttpGet("organization/department/name/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DepartmentEntity>>), 200)]
        public async Task<IActionResult> GetFullName(string id)
        {
            var data = await _departmentIBLL.GetFullName(id);
            return Success(data);
        }

        /// <summary>
        /// 新增部门
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("organization/department")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(DepartmentEntity entity)
        {
            await _departmentIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！",entity.F_DepartmentId);
        }

        /// <summary>
        /// 更新部门
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("organization/department/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,DepartmentEntity entity)
        {
            await _departmentIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/department/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _departmentIBLL.Delete(id);
            return SuccessInfo("更新成功！");
        }
    }
}
