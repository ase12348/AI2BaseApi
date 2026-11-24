using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace jcode.controllers.Message
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.07
    /// 描 述：消息策略
    /// </summary>
    public class StrategyController: BaseApiController
    {
        private readonly MStrategyInfoIBLL _mStrategyInfoIBLL;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="mStrategyInfoIBLL"></param>
        public StrategyController(MStrategyInfoIBLL mStrategyInfoIBLL)
        {
            _mStrategyInfoIBLL = mStrategyInfoIBLL;
        }


        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("message/strategys")]
        [ProducesResponseType(typeof(ResponseDto<MStrategyInfoEntity>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _mStrategyInfoIBLL.GetList();
            return Success(data);
        }
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        [HttpGet("message/strategy/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<MStrategyInfoEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _mStrategyInfoIBLL.GetPageList(pagination, keyword);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("message/strategy/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _mStrategyInfoIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        /// <summary>
        /// 新增实体数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("message/strategy")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(MStrategyInfoEntity entity)
        {
            await _mStrategyInfoIBLL.SaveEntity(string.Empty, entity);
            return SuccessInfo("保存成功！");
        }
        /// <summary>
        /// 更新实体数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("message/strategy/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, MStrategyInfoEntity entity)
        {
            await _mStrategyInfoIBLL.SaveEntity(id, entity);
            return SuccessInfo("保存成功！");
        }
        #endregion
    }
}
