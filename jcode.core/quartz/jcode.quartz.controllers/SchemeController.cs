using jcode.iquartz;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace jcode.quartz.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.04
    /// 描 述：任务计划模板
    /// </summary>
    public class SchemeController: BaseApiController
    {
        private readonly ITSSchemeIBLL _iTSSchemeIBLL;
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="tsSchemeIBLL">任务模版接口</param>
        public SchemeController(ITSSchemeIBLL tsSchemeIBLL)
        {
            _iTSSchemeIBLL = tsSchemeIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <param name="appId">所属应用主键（为空表示不过滤）</param>
        /// <returns></returns>
        [HttpGet("quartz/scheme/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<TSSchemeInfoNoSchemeEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword, string appId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _iTSSchemeIBLL.GetPageList(pagination, keyword, appId);
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
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("quartz/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto<TSSchemeInfoEntity>), 200)]
        public async Task<IActionResult> GetFormData(string id)
        {
            var data = await _iTSSchemeIBLL.GetEntity(id);
            return Success(data);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("quartz/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _iTSSchemeIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("quartz/scheme")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(TSSchemeInfoEntity entity)
        {
            await _iTSSchemeIBLL.SaveEntity(string.Empty, entity);
            return SuccessInfo("新增成功！");
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("quartz/scheme/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,TSSchemeInfoEntity entity)
        {
            await _iTSSchemeIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }

        #endregion

        #region 扩展应用
        /// <summary>
        /// 暂停一个任务
        /// </summary>
        /// <param name="id">任务进程主键</param>
        /// <returns></returns>
        [HttpPost("quartz/scheme/job/pause/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> PauseJob(string id)
        {
            await _iTSSchemeIBLL.PauseJob(id);
            return SuccessInfo("暂停成功！");
        }
        /// <summary>
        /// 启动一个任务
        /// </summary>
        /// <param name="id">任务进程主键</param>
        /// <returns></returns>
        [HttpPost("quartz/scheme/job/resume/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ResumeJob(string id)
        {
            await _iTSSchemeIBLL.EnAbleJob(id);
            return SuccessInfo("启动成功！");
        }
        #endregion
    }
}
