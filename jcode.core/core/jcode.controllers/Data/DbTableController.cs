using jcode.iapplication;
using jcode.orm;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jcode.controllers.Data
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：数据库表
    /// </summary>
    public class DbTableController : BaseApiController
    {
        private readonly DatabaseTableIBLL _databaseTableIBLL;
        private readonly DbDraftIBLL _dbDraftIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="databaseTableIBLL"></param>
        /// <param name="dbDraftIBLL"></param>
        public DbTableController(DatabaseTableIBLL databaseTableIBLL, DbDraftIBLL dbDraftIBLL)
        {
            _databaseTableIBLL = databaseTableIBLL;
            _dbDraftIBLL = dbDraftIBLL;
        }

        /// <summary>
        /// 获取数据表数据
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <returns></returns>
        [HttpGet("data/dbtables/{code}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DbTableInfo>>), 200)]
        public IActionResult GetList(string code)
        {
            var list = _databaseTableIBLL.GetImportTableList(code);
            return Success(list);
        }

        /// <summary>
        /// 获取可以导入数据表模型的表：宿主自有表不在清单里，导入接口也会拒收
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <returns></returns>
        [HttpGet("data/dbtables/importable/{code}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DbTableInfo>>), 200)]
        public IActionResult GetImportableList(string code)
        {
            var list = _databaseTableIBLL.GetImportableTableList(code);
            return Success(list);
        }

        /// <summary>
        /// 获取表数据
        /// </summary>
        /// <param name="paginationInputDto"></param>
        /// <param name="code"></param>
        /// <param name="tableName"></param>
        /// <param name="field"></param>
        /// <param name="logic"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("data/dbtable/data/page")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetTableDataPage([FromQuery] PaginationInputDto paginationInputDto, string code,string tableName, string field, string logic, string keyword)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _databaseTableIBLL.GetTableDataList(code, tableName, field, logic, keyword, pagination);
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
        /// 获取表数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        [HttpGet("data/dbtable/datas")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> GetTableDataList(string code,string tableName)
        {
            var data = await _databaseTableIBLL.GetTableDataList(code, tableName);
            return Success(data);
        }

        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="input">存储过程名称及入参</param>
        /// <returns></returns>
        [HttpPost("data/dbtable/proc/{code}")]
        [ProducesResponseType(typeof(ResponseDto<DataTable>), 200)]
        public async Task<IActionResult> ExecuteProc(string code, [FromBody] ExecuteProcInputDto input)
        {
            if (string.IsNullOrWhiteSpace(code))
            {
                return Fail("数据库编码不能为空");
            }

            if (input == null || string.IsNullOrWhiteSpace(input.ProcName))
            {
                return Fail("存储过程名称不能为空");
            }

            var procName = input.ProcName.Trim();
            if (!Regex.IsMatch(procName, @"^[a-zA-Z0-9_\.]+$"))
            {
                return Fail("存储过程名称不合法");
            }

            var inputParams = input.InputParams ?? new Dictionary<string, object>();
            List<SugarParameter> dbParameters = null;
            if (inputParams.Count > 0)
            {
                dbParameters = new List<SugarParameter>(inputParams.Count);
                var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                foreach (var kv in inputParams)
                {
                    if (string.IsNullOrWhiteSpace(kv.Key))
                    {
                        return Fail("输入参数名称不能为空");
                    }

                    var paramName = kv.Key.Trim();
                    if (paramName.StartsWith("@", StringComparison.Ordinal))
                    {
                        paramName = paramName.Substring(1);
                    }

                    if (!Regex.IsMatch(paramName, @"^[a-zA-Z0-9_]+$"))
                    {
                        return Fail("输入参数名称不合法");
                    }

                    if (!usedNames.Add(paramName))
                    {
                        return Fail("输入参数名称重复");
                    }

                    dbParameters.Add(new SugarParameter(paramName, NormalizeJsonValue(kv.Value)));
                }
            }

            var repositoryFactory = new RepositoryFactory();
            var data = await repositoryFactory.BaseRepository(code).QueryProc(procName, 3000, dbParameters);
            return Success(data);
        }

        private static object NormalizeJsonValue(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is JValue jValue)
            {
                return jValue.Value;
            }

            if (value is JToken jToken)
            {
                if (jToken.Type == JTokenType.Null || jToken.Type == JTokenType.Undefined)
                {
                    return null;
                }

                return jToken.Type switch
                {
                    JTokenType.Boolean => jToken.Value<bool>(),
                    JTokenType.Integer => jToken.Value<long>(),
                    JTokenType.Float => jToken.Value<double>(),
                    JTokenType.Date => jToken.Value<DateTime>(),
                    JTokenType.Guid => jToken.Value<Guid>(),
                    JTokenType.String => jToken.Value<string>(),
                    _ => jToken.ToString(Formatting.None)
                };
            }

            return value;
        }

        /// <summary>
        /// 获取数据表数据
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="table">表名</param>
        /// <returns></returns>
        [HttpGet("data/dbtable/columns/{code}/{table}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<DbColumnInfo>>), 200)]
        public IActionResult GetTable(string code,string table)
        {
            var list = _databaseTableIBLL.GetColumnInfosByTableName(code, table);
            return Success(list);           
        }

        #region 扩展方法
        /// <summary>
        /// 判断数据表字段重复
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <param name="filedsJson">数据字段</param>
        /// <param name="isNotSass">不需要多租户</param>
        /// <returns></returns>
        [HttpGet("data/dbtable/exist/filed")]
        [ProducesResponseType(typeof(ResponseDtoBool), 200)]
        public async Task<IActionResult> ExistFiled(string keyValue, string tableName, string keyName, string filedsJson, int? isNotSass )
        {
            var res = await _databaseTableIBLL.ExistFiled(keyValue, tableName, keyName, filedsJson, isNotSass == 1);
            return Success(res);
        }
        #endregion
    }

    /// <summary>
    /// 执行存储过程的入参
    /// </summary>
    public class ExecuteProcInputDto
    {
        /// <summary>
        /// 存储过程名称
        /// </summary>
        public string ProcName { get; set; }
        /// <summary>
        /// 存储过程入参
        /// </summary>
        public Dictionary<string, object> InputParams { get; set; }
    }
}
