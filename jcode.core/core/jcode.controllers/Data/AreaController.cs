using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.13
    /// 描 述：区域划分
    /// </summary>
    public class AreaController : BaseApiController
    {
        private readonly AreaIBLL _areaIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="areaIBLL"></param>
        public AreaController(AreaIBLL areaIBLL)
        {
            _areaIBLL = areaIBLL;
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pid">父级主键</param>
        /// <returns></returns>
        [HttpGet("data/areas/{pid}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<AreaEntity>>), 200)]
        public async Task<IActionResult> Getlist(string pid)
        {
            var list = await _areaIBLL.GetList(pid);
            return Success(list);
        }

        /// <summary>
        /// 新增区域划分
        /// </summary>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPost("data/area")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddArea(AreaEntity entity)
        {

            await _areaIBLL.SaveEntity(string.Empty, entity);
            return Success("保存成功",entity.F_AreaId);
        }
        
        /// <summary>
        /// 新增区域划分
        /// </summary>
        /// <param name="dto">实体数据</param>
        /// <returns></returns>
        [HttpPost("data/areas")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddAreas( DtoAreaEntitys dto)
        {
            await _areaIBLL.SaveEntitys(dto.list);
            return Success("保存成功");
        }

        /// <summary>
        /// 修改区域划分
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        [HttpPut("data/area/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateArea(string id,AreaEntity entity)
        {

            await _areaIBLL.SaveEntity(id, entity);
            return SuccessInfo("修改成功！");
        }
        /// <summary>
        /// 删除区域划分
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("data/area/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _areaIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }

    }
}
