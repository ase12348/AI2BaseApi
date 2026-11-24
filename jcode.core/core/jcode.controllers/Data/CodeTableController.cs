using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jcode.iapplication.Data.CodeTable;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期： 2022-03-20 10:38:26
    /// 描 述： 数据对象模型管理
    /// </summary>
    public class CodeTableController : BaseApiController
    {
        private readonly CodeTableIBLL _iCodeTableBLL;
        private readonly CodeColumnsIBLL _codeColumnsIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCodeTableBLL">接口</param>
        /// <param name="codeColumnsIBLL">接口</param>
        public CodeTableController(CodeTableIBLL iCodeTableBLL, CodeColumnsIBLL codeColumnsIBLL)
        {
            _iCodeTableBLL = iCodeTableBLL?? throw new ArgumentNullException(nameof(iCodeTableBLL));
            _codeColumnsIBLL = codeColumnsIBLL ?? throw new ArgumentNullException(nameof(codeColumnsIBLL));
        }

        #region 获取数据
        /// <summary>
        /// 获取CodeTable的列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        [HttpGet("data/codeTable/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<CodeTableEntity>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery]PaginationInputDto paginationInputDto, [FromQuery]string keyWord, [FromQuery]string dbCode, [FromQuery]string appId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iCodeTableBLL.GetPageList(pagination, keyWord, dbCode, appId);
            var jsonData = new
            {
                rows = list,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }
        

        /// <summary>
        /// 获取主表lr_db_codetable的表单数据
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <returns></returns>
        [HttpGet("data/codeTable/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto<CodeTableColumnsDto>), 200)]
        public async Task<IActionResult> GetForm(string f_Id)
        {
            var data = new CodeTableColumnsDto();
            data.Lr_db_codetableEntity = await _iCodeTableBLL.GetEntity(f_Id);
            data.Lr_db_codecolumnsList = (List<CodeColumnsEntity>)await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = f_Id });
            return Success(data);
        }

        /// <summary>
        /// 获取表的对象集合数据
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="tableNames">数据表名集合</param>
        /// <returns></returns>
        [HttpGet("data/codeTables/{dbCode}")]
        [ProducesResponseType(typeof(ResponseDto<CodeTableColumnsDto>), 200)]
        public async Task<IActionResult> GetForms(string dbCode, string tableNames)
        {
            var list = await _iCodeTableBLL.GetEntitys(dbCode, tableNames.Split(',').ToList());
            List<CodeTableColumnsDto> res = new List<CodeTableColumnsDto>();
            foreach (var item in list) {
                var data = new CodeTableColumnsDto();
                data.Lr_db_codetableEntity = item;
                data.Lr_db_codecolumnsList = (List<CodeColumnsEntity>)await _codeColumnsIBLL.GetList(new CodeColumnsEntity { F_CodeTableId = item.F_Id });
                res.Add(data);
            }
            return Success(res);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        [HttpDelete("data/codeTable/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Delete(String f_Id)
        {
            await _iCodeTableBLL.Delete(f_Id);
            return SuccessInfo("删除成功！");
        }

        /// <summary>
        /// 批量删除登记：只删元数据，物理表一张都不动
        /// </summary>
        /// <param name="ids">登记主键，逗号分隔</param>
        /// <returns></returns>
        [HttpDelete("data/codeTable/deletes/{ids}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Deletes(string ids)
        {
            await _iCodeTableBLL.Deletes(ids);
            return SuccessInfo("删除成功！");
        }

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="codeTableColumnsDto">实体数据</param>
        [HttpPost("data/codeTable")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddEntity(CodeTableColumnsDto codeTableColumnsDto)
        {
            //验证数据表字段的唯一性
            var columnsList = codeTableColumnsDto.Lr_db_codecolumnsList;
            if (columnsList.Count>0)
            {
                foreach (var item in columnsList)
                {
                    var same = columnsList.Count(it => it.F_DbColumnName == item.F_DbColumnName);
                    if (same>1)
                    {
                        return Fail("数据表字段名重复！");
                    }
                }
            }

            if (await _iCodeTableBLL.SaveEntity(null, codeTableColumnsDto.Lr_db_codetableEntity, codeTableColumnsDto.Lr_db_codecolumnsList)) {
                return Success("新增成功！");
            }
            return Fail("数据表表名已存在！");
        }

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <param name="codeTableColumnsDto">实体数据</param>
        [HttpPut("data/codeTable/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateEntity(string f_Id, CodeTableColumnsDto codeTableColumnsDto)
        {
            //验证数据表字段类名/属性名的唯一性
            var columnsList = codeTableColumnsDto.Lr_db_codecolumnsList;
            if (columnsList.Count > 0)
            {
                foreach (var item in columnsList)
                {
                    var same = columnsList.Count(it => it.F_DbColumnName == item.F_DbColumnName);
                    if (same > 1)
                    {
                        return Fail("数据表字段名重复！");
                    }
                }
            }

            if (await _iCodeTableBLL.SaveEntity(f_Id, codeTableColumnsDto.Lr_db_codetableEntity, codeTableColumnsDto.Lr_db_codecolumnsList)) {
                return SuccessInfo("更新成功！");
            }
            return Fail("数据表类名或表名已存在！");
        }



        #endregion


        /// <summary>
        /// 导入表数据
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="dto">表数据</param>
        /// <param name="appId">所属应用主键：从应用工作台导入时自动归属该应用</param>
        /// <returns></returns>
        [HttpPost("data/codeTable/importTable/{dbCode}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ImportTable(string dbCode, CodeTableDto dto, [FromQuery] string appId)
        {
            await _iCodeTableBLL.ImportTable(dbCode, dto.TableList, appId);
            return SuccessInfo("导入成功！");
        }

        /// <summary>
        ///创建或更新表
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="dropFields">确认过要回收的列名，逗号分隔；不给就只加不减</param>
        /// <returns></returns>
        [HttpPut("data/codeTable/table/{tableid}/{dbcode}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> CreateOrUpdateTable(string tableid, string dbcode,string dbName, string dropFields)
        {
            await _iCodeTableBLL.CreateOrUpdateTable(tableid, dbcode,dbName, dropFields);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 模型与物理表的列差异：回收列之前必须先看这一份，确认过的列名才带回去
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        [HttpGet("data/codeTable/diff/{tableid}/{dbcode}")]
        [ProducesResponseType(typeof(ResponseDto<CodeTableColumnDiffDto>), 200)]
        public async Task<IActionResult> GetColumnDiff(string tableid, string dbcode)
        {
            var data = await _iCodeTableBLL.GetColumnDiff(tableid, dbcode);
            return Success(data);
        }

        /// <summary>
        /// 数据表模型里已经不该存在的登记行（库里没这张表 / 宿主自有表）
        /// </summary>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        [HttpGet("data/codeTable/invalids/{dbcode}")]
        [ProducesResponseType(typeof(ResponseDto<List<CodeTableInvalidDto>>), 200)]
        public async Task<IActionResult> GetInvalidTableList(string dbcode)
        {
            var data = await _iCodeTableBLL.GetInvalidTableList(dbcode);
            return Success(data);
        }
        
        
        
        /// <summary>
        /// 导入表数据
        /// </summary>
        /// <param name="tableDataUpdateModel">更新数据</param>
        /// <returns></returns>
        [HttpPost("data/codeTable/updateTableData")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateTable(TableDataUpdateModel tableDataUpdateModel)
        {
            await _iCodeTableBLL.UpdateTable(tableDataUpdateModel);
            return SuccessInfo("更新成功！");
        }
        

    }
}
