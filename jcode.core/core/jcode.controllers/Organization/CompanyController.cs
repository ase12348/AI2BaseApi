using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：公司管理
    /// </summary>
    public class CompanyController : BaseApiController
    {
        private readonly CompanyIBLL _companyIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="companyIBLL"></param>
        public CompanyController(CompanyIBLL companyIBLL)
        {
            _companyIBLL = companyIBLL;
        }

        /// <summary>
        /// 获取公司列表信息test
        /// </summary>
        /// <returns></returns>
        [HttpGet("organization/companys")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<CompanyEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _companyIBLL.GetList();
            return Success(data);
        }

        /// <summary>
        /// 获取公司
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("organization/company/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _companyIBLL.GetEntity(id);
            return Success(data);
        }

        /// <summary>
        /// 新增公司
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("organization/company")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(CompanyEntity entity)
        {
            await _companyIBLL.SaveEntity(string.Empty,entity);
            return Success("保存成功！", entity.F_CompanyId);
        }

        /// <summary>
        /// 更新公司
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("organization/company/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,CompanyEntity entity)
        {
            await _companyIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/company/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _companyIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }
    }
}
