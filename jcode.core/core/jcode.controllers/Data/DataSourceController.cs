using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：数据源管理
    /// </summary>
    public class DataSourceController : BaseApiController
    {
        private readonly DataSourceIBLL _dataSourceIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="dataSourceIBLL"></param>
        public DataSourceController(DataSourceIBLL dataSourceIBLL)
        {
            _dataSourceIBLL = dataSourceIBLL;
        }


        /// <summary>
        /// 获取数据源列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("data/dbsources")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DataSourceEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            var data = await _dataSourceIBLL.GetList();
            return Success(data);
        }

        /// <summary>
        /// 获取数据源列表
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="category">分类</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        [HttpGet("data/dbsource/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<IEnumerable<DataSourceEntity>>), 200)]
        public async Task<IActionResult> GetPage([FromQuery] PaginationInputDto paginationInputDto, string keyword,string category, string appId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _dataSourceIBLL.GetPageList(pagination, keyword, category, appId);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet("data/dbsource/{code}")]
        [ProducesResponseType(typeof(ResponseDto<DataSourceEntity>), 200)]
        public async Task<IActionResult> GetEntityByCode(string code)
        {
            var data = await _dataSourceIBLL.GetEntityByCode(code);
            return Success(data);
        }
        /// <summary>
        /// 新增数据源
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("data/dbsource")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(DataSourceEntity entity)
        {
            bool res = await _dataSourceIBLL.SaveEntity(string.Empty, entity);
            if (res)
            {
                return Success("保存成功！", entity.F_Id);
            }
            else
            {
                return Fail("保存失败,编码重复！");
            }
        }
        /// <summary>
        /// 更新数据源
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("data/dbsource/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,DataSourceEntity entity)
        {
            bool res = await _dataSourceIBLL.SaveEntity(id, entity);
            if (res)
            {
                return SuccessInfo("保存成功！");
            }
            else
            {
                return Fail("保存失败,编码重复！");
            }
        }
        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("data/dbsource/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _dataSourceIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }







        /// <summary>
        /// 获取数据源列名
        /// </summary>
        /// <param name="code">数据源编码</param>
        /// <returns></returns>
        [HttpGet("data/dbsource/{code}/colnames")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetDataColName(string code)
        {
            var data = await _dataSourceIBLL.GetDataColName(code);
            return Success(data);
        }


        /// <summary>
        /// 获取数据源列名
        /// </summary>
        /// <param name="dbCode">数据源编码</param>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/sql/colnames")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetDataColNameBySql(string dbCode,string sql)
        {
            var data = await _dataSourceIBLL.GetDataColName(dbCode, sql);
            return Success(data);
        }


        /// <summary>
        /// 获取数据视图列信息
        /// </summary>
        /// <param name="code">数据源编码</param>
        /// <returns></returns>
        [HttpGet("data/dbsource/{code}/columns")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetViewColumns(string code)
        {
            var data = await _dataSourceIBLL.GetViewColumns(code);
            return Success(data);
        }
        
        
        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="code">数据源编号</param>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/{code}/list")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetDataTable(string code,DataQueryInputModel query)
        {
            var data = await _dataSourceIBLL.GetDataTable(code,query.Sidx, query.ParamsJson);
            return SuccessTable(data);
        }
        
        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="code">数据源编号</param>
        /// <param name="query">请求参数</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/{code}/listWithCols")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetDataTableWithCols(string code,DataQueryInputModel query)
        {
            var data = await _dataSourceIBLL.GetDataTable(code,query.Sidx,query.ParamsJson);
            var cols = _dataSourceIBLL.GetDataColumnsInfo(data);
            var jsonData = new
            {
                rows = data,
                cols
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="paginationInputDto"></param>
        /// <param name="whereSql"></param>
        /// <param name="code">数据源编号</param>
        /// <param name="field">字段名</param>
        /// <param name="dto">字段值</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/{code}/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<DataTable>>), 200)]
        public async Task<IActionResult> GetDataTablePage([FromQuery] PaginationInputDto paginationInputDto,string whereSql,string code,string field, IDSDto dto)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _dataSourceIBLL.GetDataTable(code, pagination, dto.paramsJson,field,dto.Ids, whereSql);
            var cols = _dataSourceIBLL.GetDataColumnsInfo(data);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
                cols
            };
            return Success(jsonData);
        }

        /// <summary>
        /// 获取数据源数据,数据预览，查看，不添加数据权限
        /// </summary>
        /// <param name="entity">数据源信息</param>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="whereSql">查询参数</param>
        /// <param name="param">参数</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/view/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<DataTable>>), 200)]
        public async Task<IActionResult> GetDataViewPageByEntity(DataSourceEntity entity,[FromQuery] PaginationInputDto paginationInputDto, [FromQuery] string whereSql, [FromQuery] string param)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _dataSourceIBLL.GetDataTable(entity, pagination, param, whereSql);
            var cols = _dataSourceIBLL.GetDataColumnsInfo(data);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
                cols
            };
            return Success(jsonData);
        }



        /// <summary>
        /// 获取数据源数据(根据指定字段和值)
        /// </summary>
        /// <param name="code">数据源编号</param>
        /// <param name="field">字段名</param>
        /// <param name="dto">字段值</param>
        /// <returns></returns>
        [HttpPost("data/dbsource/{code}/{field}/list")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetDataTableByValues(string code, string field, IDSDto dto)
        {
            var data = await _dataSourceIBLL.GetDataTableByValues(code, field, dto.Ids,dto.paramsJson);
            return SuccessTable(data);
        }
        
        

    }
}
