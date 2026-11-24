using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：数据字典
    /// </summary>
    public class DataItemController : BaseApiController
    {
        private readonly DataItemIBLL _dataItemIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataItemIBLL"></param>
        public DataItemController(DataItemIBLL dataItemIBLL)
        {
            _dataItemIBLL = dataItemIBLL;
        }

        /// <summary>
        /// 获取字典分类列表
        /// </summary>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的分类，为空时不过滤</param>
        /// <returns></returns>
        [HttpGet("data/dataitem/classifys")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DataItemEntity>>), 200)]
        public async Task<IActionResult> GetClassifyList(string appId)
        {
            var data = await _dataItemIBLL.GetClassifyList(string.Empty,false,appId);
            return this.Success(data);
        }


        /// <summary>
        /// 获取分类实体数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("data/dataitem/classify/{id}")]
        [ProducesResponseType(typeof(ResponseDto<DataItemEntity>), 200)]
        public async Task<IActionResult> UpdateClassify(string id)
        {
            var entity = await _dataItemIBLL.GetClassifyEntityByCode(id);
            return Success(entity);
        }


        /// <summary>
        /// 新增分类数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("data/dataitem/classify")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddClassify(DataItemEntity entity)
        {
            await _dataItemIBLL.SaveClassifyEntity(string.Empty,entity);
            return Success("保存成功！",entity.F_ItemId);
        }

        
        /// <summary>
        /// 更新分类数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("data/dataitem/classify/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateClassify(string id,DataItemEntity entity)
        {
            await _dataItemIBLL.SaveClassifyEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除分类数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/dataitem/classify/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteClassify(string id)
        {
            await _dataItemIBLL.DeleteClassify(id);
            return SuccessInfo("删除成功！");
        }


        
        /// <summary>
        /// 获取数据字典明显根据分类编号
        /// </summary>
        /// <param name="code">分类编号</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用明细加全局明细，为空时不过滤</param>
        /// <returns></returns>
        [HttpGet("data/dataitem/details/{code}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DataItemDetailEntity>>), 200)]
        public async Task<IActionResult> GetDetailList(string code, string appId)
        {
            var data = await _dataItemIBLL.GetDetailList(code, string.Empty, appId);
            return Success(data);
        }


        /// <summary>
        /// 新增明细数据实体
        /// </summary>
        /// <param name="code">分类编码</param>
        /// <param name="entity">实体</param>
        [HttpPost("data/dataitem/detail/{code}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddDetail(string code, DataItemDetailEntity entity)
        {
            var data = await _dataItemIBLL.GetClassifyEntityByCode(code);
            entity.F_ItemId = data.F_ItemId;
            await _dataItemIBLL.SaveDetailEntity(string.Empty, entity);
            return Success("保存成功！", entity.F_Id);
        }
        
        /// <summary>
        /// 新增明细数据实体
        /// </summary>
        /// <param name="code">分类编码</param>
        /// <param name="dto">实体</param>
        [HttpPost("data/dataitem/details/{code}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddDetails(string code, DataItemDetailDto dto)
        {
            await _dataItemIBLL.SaveDetailList(code, dto.List);
            return Success("保存成功！");
        }

        /// <summary>
        /// 更新明细数据实体
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        [HttpPut("data/dataitem/detail/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> UpdateDetail(string id, DataItemDetailEntity entity)
        {
            await _dataItemIBLL.SaveDetailEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 删除明细数据实体
        /// </summary>
        /// <param name="id">主键</param>
        [HttpDelete("data/dataitem/detail/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> DeleteDetail(string id)
        {
            await _dataItemIBLL.DeleteDetail(id);
            return SuccessInfo("删除成功！");
        }

    }
}
