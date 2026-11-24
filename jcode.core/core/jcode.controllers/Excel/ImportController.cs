using System;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using jcode.office;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using jcode.oss;

namespace jcode.controllers.Excel
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2021.03.31
    /// 描 述：excel导入接口
    /// </summary>
    public class ImportController : BaseApiController
    {
        private readonly ExcelImportIBLL _excelImportIBLL;
        private readonly AnnexesFileIBLL _annexesFileIBLL;
        private readonly IAnnexes _iAnnexes;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="annexesFileIBLL"></param>
        /// <param name="excelImportIBLL"></param>
        /// <param name="iAnnexes"></param>
        public ImportController(AnnexesFileIBLL annexesFileIBLL, ExcelImportIBLL excelImportIBLL,IAnnexes iAnnexes)
        {
            _annexesFileIBLL = annexesFileIBLL;
            _excelImportIBLL = excelImportIBLL;
            _iAnnexes = iAnnexes;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        [HttpGet("excel/import/page")]
        [ProducesResponseType(typeof(PaginationOutputDto<ExcelImportEntity>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword, string moduleId)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _excelImportIBLL.GetPageList(pagination, keyword, moduleId);
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
        /// 获取表单数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("excel/import/{id}")]
        [ProducesResponseType(typeof(ResponseDto<ExcelImportDto>), 200)]
        public async Task<IActionResult> GetForm(string  id)
        {
            var data = new ExcelImportDto();
            data.Entity = await _excelImportIBLL.GetEntity(id);
            data.List = (List<ExcelImportFieldEntity>)await _excelImportIBLL.GetFieldList(id);
            return Success(data);
        }



        /// <summary>
        /// 获取对应模块id
        /// </summary>
        /// <returns></returns>
        [HttpGet("excel/imports/{moduleId}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<ExcelImportEntity>>), 200)]
        public async Task<IActionResult> GetModuleList(string moduleId)
        {
            var data = await _excelImportIBLL.GetList(moduleId);
            return Success(data);
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">关联编码</param>
        /// <returns></returns>
        [HttpDelete("excel/import/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Remove(string id)
        {
            await _excelImportIBLL.DeleteEntity(id);
            return SuccessInfo("删除成功");
        }


        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [HttpPost("excel/import")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Add(ExcelImportDto dto)
        {
            await _excelImportIBLL.SaveEntity(string.Empty,dto.Entity, dto.List);
            return SuccessInfo("新增成功");
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">关联编码</param>
        /// <param name="dto">导入配置</param>
        /// <returns></returns>
        [HttpPut("excel/import/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Update(string id, ExcelImportDto dto)
        {
            await _excelImportIBLL.SaveEntity(id, dto.Entity, dto.List);
            return SuccessInfo("更新成功");
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="id">关联编码</param>
        /// <param name="state">状态</param>
        /// <returns></returns>
        [HttpPut("excel/import/state/{id}/{state}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateState(string id, int state)
        {
            await _excelImportIBLL.UpdateEntity(id, new ExcelImportEntity { F_EnabledMark = state });
            return SuccessInfo("更新成功");
        }

        
        #region 扩展方法

        /// <summary>
        /// 通用excel导入方法
        /// </summary>
        /// <param name="classname">导入方法类名</param>
        /// <param name="dto">文件参数</param>
        /// <returns></returns>
        [HttpPost("excel/import/file/{classname}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ExcelImport(string classname, AnnexesFileInputDto dto)
        {
            string path = _annexesFileIBLL.SaveAnnexes(dto.FileId, dto.FileName, dto.Chunks);
            if (!string.IsNullOrEmpty(path))
            {
                
                bool res = false;
                if(dto.ExcelAreas == null){
                    // 获取excel文件数据（暂时只支持但sheet导入，后续升级）
                    DataTable dt = ExcelHelper.ExcelImport(path);
                    // 删除临时文件
                    System.IO.File.Delete(path);
                    res = await IocManager.Instance.GetService<IExcelImportMethod>(classname).Execute(dt);
                }
                else{
                    // 获取excel文件数据（暂时只支持但sheet导入，后续升级）
                    var sheets = ExcelNewHelper.ExcelImport(path,dto.ExcelAreas);
                    // 删除临时文件
                    System.IO.File.Delete(path);
                    res = await IocManager.Instance.GetService<IExcelImportMethod2>(classname).Execute(sheets);
                }
                

                if (res)
                {
                    return SuccessInfo("文件导入成功");
                }
                else
                {
                    return Fail("文件导入失败");
                }
            }
            else
            {
                return Fail("文件导入失败");
            }
        }

                
        /// <summary>
        /// 下载文件(模版文件)
        /// </summary>
        /// <returns></returns>
        [HttpPost("excel/import/schemeId")]
        public IActionResult DownSchemeFile2(AnnexesFileInputDto dto)
        {
            var id = Guid.NewGuid().ToString();
            List<ExcelSheet> sheets = new List<ExcelSheet>();
            sheets.Add(new ExcelSheet(){Data = dto.ExcelAreas,Name = "模版"});
            _iAnnexes.UploadFile(id,$"Excel/{id}.xlsx", ".xlsx", ExcelNewHelper.ExportMemoryStream(sheets,"导出模版").ToArray(),0);
            
            return Success(id);
        }

        
        
        
        /// <summary>
        /// 下载模版文件
        /// </summary>
        /// <param name="code">模版编码</param>
        /// <returns></returns>
        [HttpGet("excel/import/scheme/{code}")]
        public async Task<FileStreamResult> DownSchemeFile(string code)
        {
            var templateInfo = await _excelImportIBLL.GetEntityByCode(code);
            if (templateInfo == null)
            {
                throw new Exception("找不到模版信息！");
            }

            var fileds = await _excelImportIBLL.GetFieldList(templateInfo.F_Id);

            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = templateInfo.F_Name + ".xlsx";
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnEntity = new List<ColumnModel>();
            //表头
            DataTable dt = new DataTable();
            foreach (var col in fileds)
            {
                if (col.F_RelationType != 1 && col.F_RelationType != 4 && col.F_RelationType != 5 && col.F_RelationType != 6 && col.F_RelationType != 7 && col.F_RelationType != 8)
                {
                    excelconfig.ColumnEntity.Add(new ColumnModel()
                    {
                        Column = col.F_Name,
                        ExcelColumn = col.F_ColName,
                        Alignment = "center",
                    });
                    dt.Columns.Add(col.F_Name, typeof(string));
                }
            }

            return File(ExcelHelper.ExportMemoryStream(dt, excelconfig), "application/ms-excel", excelconfig.FileName);
        }
        


        /// <summary>
        /// excel文件导入（通用）
        /// </summary>
        /// <param name="code">模板编码</param>
        /// <param name="dto">文件数据</param>
        /// <returns></returns>
        [HttpPost("excel/import/{code}")]
        public async Task<IActionResult> ExecuteImportExcel(string code, AnnexesFileInputDto dto)
        {
            string path = _annexesFileIBLL.SaveAnnexes(dto.FileId, dto.FileName, dto.Chunks);
            if (!string.IsNullOrEmpty(path))
            {
                // 获取excel文件数据（暂时只支持但sheet导入，后续升级）
                DataTable dt = ExcelHelper.ExcelImport(path);
                // 删除临时文件
                System.IO.File.Delete(path);

                var res2 = await _excelImportIBLL.ImportTable(code, dt);
                var data = new
                {
                    Success = res2.snum,
                    Fail = res2.fnum,
                    data = res2.elist
                };
                return Success(data);
            }
            else
            {
                return Fail("导入数据失败!");
            }
        }
        
        
        
                
        /// <summary>
        /// 下载模版文件
        /// </summary>
        /// <param name="dto">模版配置信息</param>
        /// <returns></returns>
        [HttpPost("excel/import/scheme/down")]
        public IActionResult DownSchemeFile3(ExcelImportConfig dto)
        {

            var fileds = dto.List;

            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.FileName = dto.Name + ".xlsx";
            excelconfig.IsAllSizeColumn = true;
            excelconfig.ColumnEntity = new List<ColumnModel>();
            //表头
            DataTable dt = new DataTable();
            foreach (var col in fileds)
            {
                if (col.F_RelationType != 1 && col.F_RelationType != 4 && col.F_RelationType != 5 && col.F_RelationType != 6 && col.F_RelationType != 7 && col.F_RelationType != 8)
                {
                    excelconfig.ColumnEntity.Add(new ColumnModel()
                    {
                        Column = col.F_Name,
                        ExcelColumn = col.F_ColName,
                        Alignment = "center",
                    });
                    dt.Columns.Add(col.F_Name, typeof(string));
                }
            }
            
            var id = Guid.NewGuid().ToString();
            _iAnnexes.UploadFile(id,$"Excel/{id}.xlsx", ".xlsx", ExcelHelper.ExportMemoryStream(dt, excelconfig).ToArray(),0);
            
            return Success(id);

            
        }
        
        
        
        /// <summary>
        /// excel文件导入
        /// </summary>
        /// <param name="dto">文件数据</param>
        /// <returns></returns>
        [HttpPost("excel/import/scheme/upload")]
        public async Task<IActionResult> ExecuteImportExcel3(ExcelImportConfig dto)
        {
            string path = _annexesFileIBLL.SaveAnnexes(dto.FileId, dto.FileName, dto.Chunks);
            if (!string.IsNullOrEmpty(path))
            {
                // 获取excel文件数据（暂时只支持但sheet导入，后续升级）
                DataTable dt = ExcelHelper.ExcelImport(path);
                // 删除临时文件
                System.IO.File.Delete(path);
                
                var res2 = await _excelImportIBLL.ImportTable(dto.Entity,dto.List, dt);
                
                var data = new
                {
                    Success = res2.snum,
                    Fail = res2.fnum,
                    data = res2.elist
                };
                
                return Success(data);
            }
            else
            {
                return Fail("导入数据失败!");
            }
        }
        
        /// <summary>
        /// excel文件导入（获取导入数据）
        /// </summary>
        /// <param name="dto">文件数据</param>
        /// <returns></returns>
        [HttpPost("excel/import/scheme/upload/data")]
        public IActionResult ExecuteImportExcel4(ExcelImportConfig dto)
        {
            string path = _annexesFileIBLL.SaveAnnexes(dto.FileId, dto.FileName, dto.Chunks);
            if (!string.IsNullOrEmpty(path))
            {
                // 获取excel文件数据（暂时只支持但sheet导入，后续升级）
                DataTable dt = ExcelHelper.ExcelImport(path);
                // 删除临时文件
                System.IO.File.Delete(path);
                
                var data = new
                {
                    Success = dt.Rows.Count,
                    Fail = 0,
                    data = dt
                };
                
                return Success(data);
            }
            else
            {
                return Fail("导入数据失败!");
            }
        }
        #endregion
    }
}
