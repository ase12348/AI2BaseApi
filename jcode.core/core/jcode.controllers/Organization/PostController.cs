using System.Collections.Generic;
using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：岗位管理
    /// </summary>
    public class PostController : BaseApiController
    {
        private readonly PostIBLL _postIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="postIBLL"></param>
        public PostController(PostIBLL postIBLL)
        {
            _postIBLL = postIBLL;
        }

        /// <summary>
        /// 获取岗位列表信息
        /// </summary>
        /// <param name="companyId">公司</param>
        /// <param name="departmentId">部门</param>
        /// <returns></returns>
        [HttpGet("organization/posts")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<PostEntity>>), 200)]
        public async Task<IActionResult> GetList(string companyId, string departmentId)
        {
            var data = await _postIBLL.GetList(companyId, departmentId, string.Empty);
            return Success(data);
        }


        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("organization/post/{id}")]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _postIBLL.GetEntity(id);
            return Success(data);
        }

        /// <summary>
        /// 获取岗位信息
        /// </summary>
        /// <param name="dto">主键集合</param>
        /// <returns></returns>
        [HttpPost("organization/posts")]
        public async Task<IActionResult> GetListByPostIds(IDSDto dto)
        {
            var data = await _postIBLL.GetListByPostIds(dto.Ids);
            return Success(data);
        }

        

        /// <summary>
        /// 新增岗位
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("organization/post")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> SaveForm(PostEntity entity)
        {
            await _postIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_PostId);
        }

        /// <summary>
        /// 更新岗位
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("organization/post/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveForm(string id, PostEntity entity)
        {
            await _postIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }
        /// <summary>
        /// 删除岗位
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/post/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _postIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }
    }
}
