using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Permission
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.13
    /// 描 述：数据权限
    /// </summary>
    public class DataController : BaseApiController
    {
        private readonly DataAuthorizeIBLL _dataAuthorizeIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataAuthorizeIBLL">数据权限操作接口</param>
        public DataController(DataAuthorizeIBLL dataAuthorizeIBLL)
        {
            _dataAuthorizeIBLL = dataAuthorizeIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取数据权限对应关系数据列表
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="objectId">用户或角色主键</param>
        /// <param name="type">1.普通权限 2.自定义表单权限</param>
        /// <returns></returns>
        [HttpGet("permission/data/page")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DataAuthorizeEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, string keyword, string objectId, int? type)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _dataAuthorizeIBLL.GetPageList(pagination, keyword, objectId, type);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取数据权限数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("permission/data/{id}")]
        [ProducesResponseType(typeof(ResponseDto<DataAuthorizeEntity>), 200)]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _dataAuthorizeIBLL.GetEntity(id);
            return Success(data);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 新增数据权限
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("permission/data")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(DataAuthorizeEntity entity)
        {
            await _dataAuthorizeIBLL.SaveEntity(string.Empty, entity);
            return SuccessInfo("新增成功！");
        }

        /// <summary>
        /// 更新数据权限
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("permission/data/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, DataAuthorizeEntity entity)
        {

            await _dataAuthorizeIBLL.SaveEntity(id, entity);
            return SuccessInfo("保存成功！");
        }
        /// <summary>
        /// 删除数据权限
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("permission/data/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _dataAuthorizeIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        #endregion

    }
}
