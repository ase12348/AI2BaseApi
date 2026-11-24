using jcode.iapplication;
using jcode.operat;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.BaseInfo
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.25
    /// 描 述：日志模块
    /// </summary>
    public class LogController : BaseApiController
    {
        private readonly LogIBLL _logIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="logIBLL"></param>
        public LogController(LogIBLL logIBLL)
        {
            _logIBLL = logIBLL;
        }

        /// <summary>
        ///  分页查询
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="query">查询条件</param>
        /// <returns></returns>
        [HttpGet("system/log/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<LogEntity>>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LogQueryDto query)
        {

            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _logIBLL.GetPageList(pagination, query);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }
        
        // 关闭，日志不让清除
        // /// <summary>
        // /// 清空日志 7：保留最近7天 1:保留最近一个月 3:保留最近3个月
        // /// </summary>
        // /// <param name="categoryId">日志分类 1.登录日志 2.访问日志 3.操作日志 4.异常日志</param>
        // /// <param name="keepTime">保留时间段内</param>
        // /// <returns></returns>
        // [HttpDelete("system/log/{categoryId}/{keepTime}")]
        // [ProducesResponseType(typeof(ResponseDto), 200)]
        // public async Task<IActionResult> SaveRemoveLog(int categoryId, string keepTime)
        // {
        //     await _logIBLL.Remove(categoryId, keepTime);
        //     return SuccessInfo("清空成功。");
        // }


        
        
        #region 扩展方法
        #region 我的
        /// <summary>
        /// 获取审计员日志
        /// </summary>
        /// <param name="paginationInputDto"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet("system/log/my/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<LogEntity>>>), 200)]
        public async Task<IActionResult> GetUserPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] LogQueryDto query)
        {

            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _logIBLL.GetMyPageList(pagination, query);
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
        #endregion
    }
}
