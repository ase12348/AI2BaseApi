using jcode.iapplication;
using jcode.iapplication.Excel.Export;
using jcode.operat;
using jcode.oss;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web;
using jcode.office;

namespace jcode.controllers.Excel
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.03.31
    /// 描 述：excel导出接口
    /// </summary>
    public class ExportController : BaseApiController
    {
        private readonly ExcelExportIBLL _excelExportIBLL;
        private readonly DataItemIBLL _dataItemIBLL;
        private readonly DataSourceIBLL _dataSourceIBLL;
        private readonly CompanyIBLL _companyIBLL;
        private readonly DepartmentIBLL _departmentIBLL;
        private readonly UserIBLL _userIBLL;

        private readonly IAnnexes _iAnnexes;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="excelExportIBLL"></param>
        /// <param name="dataItemIBLL"></param>
        /// <param name="dataSourceIBLL"></param>
        /// <param name="companyIBLL"></param>
        /// <param name="departmentIBLL"></param>
        /// <param name="userIBLL"></param>
        /// <param name="iAnnexes"></param>
        public ExportController(ExcelExportIBLL excelExportIBLL, DataItemIBLL dataItemIBLL, DataSourceIBLL dataSourceIBLL, CompanyIBLL companyIBLL, DepartmentIBLL departmentIBLL, UserIBLL userIBLL, IAnnexes iAnnexes)
        {
            _excelExportIBLL = excelExportIBLL;
            _dataItemIBLL = dataItemIBLL;
            _dataSourceIBLL = dataSourceIBLL;
            _companyIBLL = companyIBLL;
            _departmentIBLL = departmentIBLL;
            _userIBLL = userIBLL;

            _iAnnexes = iAnnexes;
        }


        #region 获取数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <param name="moduleId">模块</param>
        /// <returns></returns>
        [HttpGet("excel/export/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<ExcelExportEntity>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword, string moduleId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _excelExportIBLL.GetPageList(pagination, keyword, moduleId);
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
        /// 获取列表数据
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        [HttpGet("excel/exports/{moduleId}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<ExcelExportEntity>>), 200)]
        public async Task<IActionResult> GetList(string moduleId)
        {
            var data = await _excelExportIBLL.GetList(moduleId);
            return Success(data);
        }
        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("excel/export/{id}")]
        [ProducesResponseType(typeof(ResponseDto<ExcelExportEntity>), 200)]
        public async Task<IActionResult> GetEntity(string id)
        {
            var data = await _excelExportIBLL.GetEntity(id);
            return Success(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPost("excel/export")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> AddForm(ExcelExportEntity entity)
        {
            await _excelExportIBLL.SaveEntity(string.Empty, entity);
            return SuccessInfo("新增成功！");
        }
        /// <summary>
        /// 更新表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        [HttpPut("excel/export/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id,ExcelExportEntity entity)
        {
            await _excelExportIBLL.SaveEntity(id, entity);
            return SuccessInfo("更新成功！");
        }
        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("excel/export/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _excelExportIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功！");
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 创建需要导出的文件
        /// </summary>
        /// <param name="dto">导出对象</param>
        /// <returns></returns>
        [HttpPost("excel/export/file")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> CreateFile(ExcelExportDto dto)
        {
            List<ExcelSheet> sheets = new List<ExcelSheet>();
            ExcelSheet sheet = new ExcelSheet();
            sheets.Add(sheet);
            sheet.Name = dto.FileName;
            sheet.Data = new List<ExcelArea>();
            var excelArea = new ExcelArea();
            sheet.Data.Add(excelArea);
            excelArea.Type = 2;
            excelArea.Table = new ExcelTable();
            excelArea.Table.IsHeaderMerged = true;
            excelArea.Table.Header = new List<ExcelCell>();

            foreach (var col in dto.TableHeader)
            {
                if (col.isHidden)
                {
                    continue;
                }

                if (col.Width == null)
                {
                    col.Width = 240;
                }

                excelArea.Table.Header.Add(new ExcelCell()
                {
                    Prop = col.Prop,
                    Align = col.Align,
                    Text = col.Label,
                    Width = (int)col.Width,
                    ColSpan = (int)col.ColSpan,
                    RowSpan = (int)col.RowSpan,
                    X = col.X,
                    Y = col.Y
                });
            }
            
            //设置导出格式
            //ExcelConfig excelconfig = new ExcelConfig();
            //excelconfig.Title = HttpUtility.UrlDecode(dto.FileName);
            //excelconfig.TitleFont = "微软雅黑";
            //excelconfig.TitlePoint = 15;

            //excelconfig.FileName = HttpUtility.UrlDecode(dto.FileName) + ".xls";
            //excelconfig.IsAllSizeColumn = true;
            //excelconfig.ColumnEntity = new List<ColumnModel>();

            DataTable rowData = null;
            if (!string.IsNullOrEmpty(dto.DataUrl))
            {
                if (dto.DataUrl.IndexOf("?") == -1)
                {
                    dto.DataUrl = $"{dto.DataUrl}?token={RequestPermission.GetRequestToken}";
                }
                else
                {
                    dto.DataUrl = $"{dto.DataUrl}&token={RequestPermission.GetRequestToken}";
                }

                if (dto.IsPost)
                {
                    var res = await HttpMethods.Post(dto.DataUrl,dto.ReqData);
                    rowData = GetData(res, dto.IsPage);
                }
                else {
                    var res = await HttpMethods.Get(dto.DataUrl);
                    rowData = GetData(res, dto.IsPage);
                }
            }
            else {
                rowData = dto.DataList;
            }
            await FormatData(rowData, dto.Columns);
            excelArea.Table.Data = rowData;
            
            /*foreach (var columnModel in dto.Columns)
            {
                excelconfig.ColumnEntity.Add(new ColumnModel()
                {
                    Column = columnModel.Prop,
                    ExcelColumn = columnModel.Label,
                    Alignment = columnModel.Align,
                    Width = columnModel.Prop.Length,
                    DateFormat = columnModel.Format
                });
            }*/

            var id = Guid.NewGuid().ToString();
            //var path = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/{id}.xls";
            //_iAnnexes.UploadFile($"Excel/{id}.xls", path);
            //FileHelper.Write(path, ExcelHelper.ExportMemoryStream(rowData, excelconfig).ToArray());
            _iAnnexes.UploadFile(id,$"Excel/{id}.xlsx", ".xlsx", ExcelNewHelper.ExportMemoryStream(sheets, HttpUtility.UrlDecode(dto.FileName) + ".xlsx").ToArray(),0);
            
            return Success(id);
        }
        /// <summary>
        /// 下载导出文件
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        [HttpGet("excel/export/file/{name}/{id}")]
        public PhysicalFileResult DownFile(string id,string name)
        {
            string path = _iAnnexes.DownFile(id, $"Excel/{id}.xlsx", ".xlsx",0,true);
            return new PhysicalFileResult(path, "application/ms-excel") { FileDownloadName = $"{HttpUtility.UrlDecode(name)}.xlsx" };
        }


        
        private DataTable GetData(string res,bool isPage) {
            if (isPage)
            {
                var resData = res.ToObject<ResponseDto<PaginationOutputDto<DataTable>>>();
                return resData.data.rows;
            }
            else {
                var resData = res.ToObject<ResponseDto<DataTable>>();
                return resData.data;
            }
        }

        /// <summary>
        /// 格式化数据
        /// </summary>
        /// <param name="dt">数据</param>
        /// <param name="Columns">列信息</param>
        private async Task FormatData(DataTable dt, List<TableColumnModel> Columns) {
            // 需要做转换的列
            List<TableColumnModel> list = new List<TableColumnModel>();
            foreach (var col in Columns) {
                dt.Columns.Add($"{col.Prop}_string",typeof(string));
            }

            // 数据字典数据
            Dictionary<string, List<DataItemDetailEntity>> dataItemMap = new Dictionary<string, List<DataItemDetailEntity>>();
            // 数据源数据
            Dictionary<string, Dictionary<string,DataRow>> dataSourceMap = new Dictionary<string, Dictionary<string,DataRow>>();
            // 公司
            List<CompanyEntity> companyList = null;
            // 部门
            Dictionary<string, string> departmentMap = new Dictionary<string, string>();
            // 用户
            Dictionary<string, string> userMap = new Dictionary<string, string>();

            foreach (DataRow row in dt.Rows) {
                foreach (var col in Columns) {
                    if (!row[col.Prop].IsEmpty())
                    {
                        switch (col.Config?.DataType)
                        {
                            case "options":
                                row[$"{col.Prop}_string"] = FindOptionsName(col.Config?.Options, row[col.Prop].ToString());
                                break;
                            case "dataItem":
                                if (!dataItemMap.ContainsKey(col.Config?.DataCode))
                                {
                                    List<DataItemDetailEntity> dataItemList = (List<DataItemDetailEntity>)await _dataItemIBLL.GetDetailList(col.Config?.DataCode);
                                    dataItemMap.Add(col.Config?.DataCode, dataItemList);
                                }
                                row[$"{col.Prop}_string"] = FindDataItemName(dataItemMap[col.Config?.DataCode], row[col.Prop].ToString());
                                break;
                            case "dataSource":
                                if (!dataSourceMap.ContainsKey(col.Config?.DataCode + col.Config?.ValueKey))
                                {
                                    Dictionary<string, DataRow> dic = new Dictionary<string, DataRow>();
                                    DataTable sourceDt = await _dataSourceIBLL.GetDataTable(col.Config?.DataCode,"");
                                    foreach (DataRow rowItem in sourceDt.Rows)
                                    {
                                        var dicKeyValue = rowItem[col.Config?.ValueKey].ToString();
                                        if (!string.IsNullOrEmpty(dicKeyValue) && !dic.ContainsKey(dicKeyValue))
                                        {
                                            dic.Add(dicKeyValue,rowItem);
                                        }
                                    }
                                    dataSourceMap.Add(col.Config?.DataCode + col.Config?.ValueKey, dic);
                                }
                                row[$"{col.Prop}_string"] = FindDataSourceName(dataSourceMap[col.Config?.DataCode + col.Config?.ValueKey], col.Config?.LabelKey, row[col.Prop].ToString());
                                break;
                            case "dateTime":
                                if (!row[col.Prop].IsEmpty())
                                {
                                    row[$"{col.Prop}_string"] = row[col.Prop].ToDate().ToString("yyyy-MM-dd HH:mm:ss");
                                }
                                
                                break;
                            case "company":
                                if (companyList == null) {
                                    companyList = (List<CompanyEntity>)await _companyIBLL.GetList();
                                }
                                row[$"{col.Prop}_string"] = companyList.Find(t => t.F_CompanyId == row[col.Prop].ToString())?.F_FullName;
                                break;
                            case "department":
                                if (!departmentMap.ContainsKey(row[col.Prop].ToString())) {
                                    string name = (await _departmentIBLL.GetEntity(row[col.Prop].ToString()))?.F_FullName;
                                    if (!string.IsNullOrEmpty(name))
                                    {
                                        departmentMap.Add(row[col.Prop].ToString(), name);
                                    }
                                    else {
                                        departmentMap.Add(row[col.Prop].ToString(), row[col.Prop].ToString());
                                    }
                                }
                                row[$"{col.Prop}_string"] = departmentMap[row[col.Prop].ToString()];
                                break;
                            case "user":
                                var userValue = row[col.Prop].ToString();
                                if (!string.IsNullOrEmpty(userValue))
                                {
                                    var userLabel = new List<string>();
                                    var userValues = userValue.Split(",");
                                    foreach (var userValueItem  in userValues)
                                    {
                                        if (!userMap.ContainsKey(userValueItem))
                                        {
                                            string name = (await _userIBLL.GetEntity(userValueItem))?.F_RealName;
                                            if (!string.IsNullOrEmpty(name))
                                            {
                                                userMap.Add(userValueItem, name);
                                            }
                                            else
                                            {
                                                userMap.Add(userValueItem, userValueItem);
                                            }
                                        }

                                        userLabel.Add(userMap[userValueItem]);
                                    }
                                    row[$"{col.Prop}_string"] = string.Join(",", userLabel.ToArray());
                                }
                                else
                                {
                                    row[$"{col.Prop}_string"] = "";
                                }
                                break;
                            default:
                                if (!row[col.Prop].IsEmpty())
                                {
                                    row[$"{col.Prop}_string"] = row[col.Prop].ToString();
                                }
                                break;
                        }
                    }
                       
                }
            }

            foreach (var col in Columns)
            {
                if(dt.Columns.CanRemove(dt.Columns[col.Prop])) {
                    dt.Columns.Remove(dt.Columns[col.Prop]);
                }
                dt.Columns[$"{col.Prop}_string"].ColumnName = col.Prop;
            }
        }   

        /// <summary>
        /// 自定义数据查找
        /// </summary>
        /// <param name="Options">数据字典数据</param>
        /// <param name="itemValue">项目名</param>
        /// <returns></returns>
        private string FindOptionsName(List<TableDataOptions> Options, string itemValue)
        {
            List<string> res = new List<string>();
            var itemValueList = itemValue.Split(",");
            foreach (var item in itemValueList)
            {
                var dataItem = Options.Find(t => t.Value == item);
                if (dataItem != null)
                {
                    res.Add(dataItem.Label);
                }
                else
                {
                    res.Add(item);
                }
            }
            return string.Join(",", res.ToArray());
        }
        /// <summary>
        /// 数据字典查找
        /// </summary>
        /// <param name="dataItemList">数据字典数据</param>
        /// <param name="itemValue">项目名</param>
        /// <returns></returns>
        private string FindDataItemName(List<DataItemDetailEntity> dataItemList, string itemValue)
        {
            List<string> res = new List<string>();
            var itemValueList = itemValue.Split(",");
            foreach (var item in itemValueList) {
                DataItemDetailEntity dataItem = dataItemList.Find(t => t.F_ItemValue == item);
                if (dataItem != null)
                {
                    res.Add(dataItem.F_ItemName);
                }
                else
                {
                    res.Add(item);
                }
            }
            return string.Join(",", res.ToArray());           
        }

        /// <summary>
        /// 数据源查找
        /// </summary>
        /// <param name="dic">数据源数据</param>
        /// <param name="labelKey">显示字段</param>
        /// <param name="itemValue">值字段</param>
        /// <returns></returns>
        private string FindDataSourceName(Dictionary<string, DataRow> dic, string labelKey, string itemValue)
        {
            
            List<string> res = new List<string>();
            var itemValueList = itemValue.Split(",");
            foreach (var item in itemValueList)
            {
                var label = string.Empty;
                if (dic.ContainsKey(item))
                {
                    DataRow dr = dic[item];
                    label = dr[labelKey].ToString();
                }
                if (!string.IsNullOrEmpty(label))
                {
                    res.Add(label);
                }
                else
                {
                    res.Add(item);
                }
            }
            return string.Join(",", res.ToArray());
        }




        #endregion
    }
}
