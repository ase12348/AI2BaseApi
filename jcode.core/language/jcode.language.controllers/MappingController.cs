using jcode.ilanguage;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.controllers.Language
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：多语言翻译
    /// </summary>
    public class MappingController : BaseApiController
    {
        private readonly LGMapIBLL _lGMapIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="lGMapIBLL"></param>
        public MappingController(LGMapIBLL lGMapIBLL)
        {
            _lGMapIBLL = lGMapIBLL;
        }


        /// <summary>
        ///  分页查询
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <returns></returns>
        [HttpGet("language/mapping/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<DataTable>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _lGMapIBLL.GetPageList(pagination, keyword);
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
        /// 获取菜单功能翻译数据
        /// </summary>
        /// <param name="moduleId">功能模块id</param>
        /// <param name="isParent">是否父级菜单</param>
        /// <returns></returns>
        [HttpGet("language/mapping/modules/{moduleId}/{isParent}")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetModuleList(string moduleId, int isParent)
        {
            var data = await _lGMapIBLL.GetModuleList(moduleId, isParent);
            return Success(data);
        }

        /// <summary>
        /// 获取数据字典翻译数据
        /// </summary>
        /// <param name="code">数据字典编码</param>
        /// <returns></returns>
        [HttpGet("language/mapping/dataitems/{code}")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetDataItemList(string code)
        {
            var data = await _lGMapIBLL.GetDataItemList(code);
            return Success(data);
        }

        /// <summary>
        /// 根据语言类型编码获取语言包
        /// </summary>
        /// <param name="mainCode">主语言</param>
        /// <param name="code">语言编码</param>
        /// <returns></returns>
        [HttpGet("language/mapping/{mainCode}/{code}")]
        [ProducesResponseType(typeof(ResponseDto<Dictionary<string, string>>), 200)]
        [AllowAnonymous]
        public async Task<IActionResult> GetLanguageByCode(string mainCode,string code)
        {
            var data = await _lGMapIBLL.GetMap(code, mainCode);
            return Success(data);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="code">关联编码</param>
        /// <returns></returns>
        [HttpDelete("language/mapping/{code}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Remove(string code) {
            await _lGMapIBLL.DeleteEntity(code);
            return SuccessInfo("删除成功");
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <returns></returns>
        [HttpPost("language/mapping")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Add(List<LGMapEntity> list)
        {
            await _lGMapIBLL.SaveEntity(list,string.Empty);
            return SuccessInfo("新增成功");
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="code">关联编码</param>
        /// <param name="list">翻译数据</param>
        /// <returns></returns>
        [HttpPut("language/mapping/{code}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Update(string code,List<LGMapEntity> list)
        {
            await _lGMapIBLL.SaveEntity(list, code);
            return SuccessInfo("更新成功");
        }
    }
}
