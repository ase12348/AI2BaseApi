using System;
using jcode.iapplication;
using jcode.oss;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices.JavaScript;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using jcode.autofac;
using jcode.office;
using Org.BouncyCastle.Utilities;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.13
    /// 描 述：附件管理
    /// </summary>
    public class AnnexesFileController : BaseApiController
    {
        private readonly AnnexesFileIBLL _annexesFileIBLL;
        private readonly IAnnexes _iAnnexes;
        private readonly LogIBLL _logIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="annexesFileIBLL"></param>
        /// <param name="iAnnexes"></param>
        /// <param name="logIBLL"></param>
        public AnnexesFileController(AnnexesFileIBLL annexesFileIBLL, IAnnexes iAnnexes,LogIBLL logIBLL)
        {
            _annexesFileIBLL = annexesFileIBLL;
            _iAnnexes = iAnnexes;
            _logIBLL = logIBLL;
        }
        /// <summary>
        /// 上传附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunk">分片序号</param>
        /// <returns></returns>
        [HttpPost("system/annexesfile/{fileId}/{chunk}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public IActionResult UploadAnnexesFileChunk(string fileId, int chunk)
        {
            var files = Request.Form.Files;
            //没有文件上传，直接返回
            if (files[0].Length == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return Fail("没有文件信息");
            }
            byte[] bytes = new byte[files[0].Length];
            MemoryStream ms = new MemoryStream();
            files[0].CopyTo(ms);
            ms.Flush();
            ms.Position = 0;
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            ms.Dispose();
            var res = _annexesFileIBLL.SaveChunkAnnexes(fileId, chunk, bytes);
            if (res)
            {
                return SuccessInfo("保存成功");
            }
            
            return Fail("保存失败");

        }


        /// <summary>
        /// 上传附件(不分片用于移动端上传)
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="dto">文件数据</param>
        /// <returns></returns>
        [HttpPost("system/annexesfile/{fileId}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveAnnexes(string fileId,[FromForm]AnnexesFileInputDto dto)
        {
            var files = Request.Form.Files;
            //没有文件上传，直接返回
            if (files[0].Length == 0 || string.IsNullOrEmpty(files[0].FileName))
            {
                return Fail("没有文件信息");
            }
            byte[] bytes = new byte[files[0].Length];
            MemoryStream ms = new MemoryStream();
            files[0].CopyTo(ms);
            ms.Flush();
            ms.Position = 0;
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            ms.Dispose();
            bool res = await _annexesFileIBLL.SaveAnnexes(dto.FolderId, fileId, dto.FileName, bytes, dto.DbCode,dto.TableName,dto.TableField);
            if (res)
            {
                return SuccessInfo("保存文件成功");
            }
            else
            {
                return Fail("保存文件失败");
            }
        }
        /// <summary>
        /// 合并上传附件的分片数据(固定文件夹)
        /// </summary>
        /// <param name="dto">提交参数</param>
        /// <returns></returns>
        [HttpPost("system/annexesfile/merge")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> MergeAnnexesFile(AnnexesFileInputDto dto)
        {
            var annexeEntity = await _annexesFileIBLL.SaveAnnexes(dto.FolderId, dto.FileId, dto.FileName, dto.Chunks, dto.DbCode, dto.TableName, dto.TableField);
            if (annexeEntity != null)
            {
                return SuccessInfo("保存文件成功");
            }
            else
            {
                return Fail("保存文件失败");
            }
        }






        /// <summary>
        /// 移除附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunks">总分片数</param>
        /// <returns></returns>
        [HttpDelete("system/annexesfile/{fileId}/{chunks}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public IActionResult RemoveAnnexesFileChunk(string fileId, int chunks)
        {
            _annexesFileIBLL.RemoveChunkAnnexes(fileId, chunks);
            return SuccessInfo("移除成功");
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <returns></returns>
        [HttpDelete("system/annexesfile/{fileId}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteAnnexesFile(string fileId)
        {
            await _annexesFileIBLL.DeleteEntity(fileId);
            return SuccessInfo("删除附件成功");
        }

        /// <summary>
        /// 丢弃暂存附件：表单关闭时清掉本次未保存的上传
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        /// <returns></returns>
        [HttpDelete("system/annexesfile/draft/{folderId}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DiscardDraftAnnexesFile(string folderId)
        {
            await _annexesFileIBLL.DiscardDraft(folderId);
            return SuccessInfo("丢弃暂存附件成功");
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/{fileId}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<FileResult> DownAnnexesFile(string fileId)
        {
            // 记录访问权限
            Stopwatch stopwatch = CommonHelper.TimerStart();
            
           
            var entity = await _annexesFileIBLL.GetDownFileEntity(fileId);
            string path = _annexesFileIBLL.DownFile(entity);
           
            if (string.IsNullOrWhiteSpace(path))
            {
                return File(Encoding.UTF8.GetBytes("找不到文件！"), FileHelper.getContentType("txt"));
            }
            else
            {
                // string FileName = Path.GetFileName(path);
                string fileType = Path.GetExtension(path).Replace(".", "");
                var contentType = FileHelper.getContentType(fileType);
                #region 写入日志

                if (contentType.IndexOf("image") == -1)
                {
                    LogEntity logEntity = new LogEntity();
                    logEntity.F_CategoryId = 5;
                    logEntity.F_ExecuteResult = 1;
                    logEntity.F_ExecuteResultJson = "下载文件:" + entity.F_FileName + $"({fileId})";
                    _logIBLL.Write(logEntity,stopwatch);
                }

                #endregion
                
                return new PhysicalFileResult(path, contentType)
                {
                    FileDownloadName = entity.F_FileName
                };
            }
        }
        
        
        /// <summary>
        /// 下载临时文件
        /// </summary>
        /// <param name="path">文件目录</param>
        /// <param name="id">文件id</param>
        /// <param name="name">下载文件名</param>
        /// <param name="fileType">文件类型</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/temp/{path}/{id}/{name}/{fileType}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public FileResult DownAnnexesFile(string path, string id,string name,string fileType)
        {
            string path2 = _iAnnexes.DownFile(id, $"{path}/{id}.{fileType}", $".{fileType}",0,true);
            return new PhysicalFileResult(path2, FileHelper.getContentType(fileType)) { FileDownloadName = $"{HttpUtility.UrlDecode(name)}.{fileType}" };
        }

        /// <summary>
        /// 获取附件列表
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/{folderId}/list")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<AnnexesFileEntity>>), 200)]
        public async Task<IActionResult> GetAnnexesFileList(string folderId)
        {
            var data = await _annexesFileIBLL.GetList(folderId);
            return Success(data);
        }
        
        /// <summary>
        /// 导入临时文件
        /// </summary>
        /// <param name="classname">导入方法类名</param>
        /// <param name="dto">文件参数</param>
        /// <returns></returns>
        [HttpPost("system/annexesfile/import/temp/{classname}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ExcelImport(string classname, AnnexesFileInputDto dto)
        {
            string path = _annexesFileIBLL.SaveAnnexes(dto.FileId, dto.FileName, dto.Chunks);
            if (!string.IsNullOrEmpty(path))
            {
                // 获取文件数据
                var data = FileHelper.Read(path);
                // 删除临时文件
                System.IO.File.Delete(path);
                var res = await IocManager.Instance.GetService<IFileImportMethod>(classname).Execute(data);

                if (res)
                {
                    return SuccessInfo("导入成功!");
                }
                else
                {
                    return Fail("导入失败，请检查后重新上传！");
                }
            }
            else
            {
                return Fail("导入失败!");
            }
        }
        
        #region 预览附件
        /// <summary>
        /// 获取文件预览方式
        /// </summary>
        /// <param name="id">预览参数，与文件预览地址中的 id 一致</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/preview-meta/{id}")]
        [ProducesResponseType(typeof(ResponseDto<AnnexesFilePreviewMetaDto>), 200)]
        public async Task<IActionResult> PreviewMeta(string id)
        {
            if (!TryDecryptPreviewId(id, out string fileId, out string error))
            {
                return Fail(error);
            }
            var meta = await _annexesFileIBLL.GetPreviewMeta(fileId);
            return Success(meta);
        }

        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="id">文件ID</param>
        /// <param name="isNotCache">是否缓存</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/preview/{id}")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        public async Task<IActionResult> PreviewFile(string id,int isNotCache = 0)
        {
            // 记录访问权限
            Stopwatch stopwatch = CommonHelper.TimerStart();
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "no-store";

            if (!TryDecryptPreviewId(id, out string fileId, out string error))
            {
                return PreviewFallbackResult(null, error);
            }

            string path;
            try
            {
                path = await _annexesFileIBLL.ToPreview(fileId, isNotCache);
            }
            catch (PreviewConvertException ex)
            {
                return PreviewFallbackResult(fileId, ex.Message);
            }

            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 6;
            logEntity.F_ExecuteResult = 1;
            logEntity.F_ExecuteResultJson = "预览文件:"  + $"({fileId})";
            _logIBLL.Write(logEntity,stopwatch);

            if (string.IsNullOrWhiteSpace(path) || !System.IO.File.Exists(path))
            {
                return PreviewFallbackResult(fileId, "文件已丢失，无法预览");
            }

            string fileType = Path.GetExtension(path).Replace(".", "").ToLower();
            var stream = new FileStream(path, FileMode.Open);
            switch (fileType)
            {
                case "txt":
                case "json":
                    return new FileStreamResult(stream, "text/plain;charset=utf-8");
                case "html":
                case "htm":
                    // 用户上传的 html / 转换产物一律沙箱化，禁止脚本执行
                    Response.Headers["Content-Security-Policy"] = "sandbox; default-src 'none'; img-src data: blob:; style-src 'unsafe-inline'; font-src data:";
                    return new FileStreamResult(stream, "text/html;charset=utf-8");
                default:
                    return new FileStreamResult(stream, FileHelper.getContentType(fileType));
            }
        }

        /// <summary>
        /// 解析预览参数，兼容历史前端 /、+、= 的不同替换方式
        /// </summary>
        /// <param name="id">预览参数</param>
        /// <param name="fileId">文件ID</param>
        /// <param name="error">错误提示</param>
        /// <returns></returns>
        private bool TryDecryptPreviewId(string id, out string fileId, out string error)
        {
            fileId = null;
            error = null;
            try
            {
                string raw = (id ?? string.Empty).Replace("|", "/").Replace("-", "+").Replace("_", "=");
                var idScheme = AESHelper.AesDecrypt(raw, "jcode").Split("|");
                if (idScheme.Length < 2 || string.IsNullOrWhiteSpace(idScheme[1]))
                {
                    error = "预览参数不正确！";
                    return false;
                }
                var time = idScheme[0].ToDate();
                if (DateTime.Now.AddMinutes(-1) > time)
                {
                    error = "预览文件已过期，请重新刷新！";
                    return false;
                }
                fileId = idScheme[1];
                return true;
            }
            catch
            {
                error = "预览参数不正确！";
                return false;
            }
        }

        /// <summary>
        /// 预览失败时返回兜底页，而不是浏览器直接显示的纯文本
        /// </summary>
        /// <param name="fileId">文件ID，用于给出下载入口</param>
        /// <param name="message">失败原因</param>
        /// <returns></returns>
        private IActionResult PreviewFallbackResult(string fileId, string message)
        {
            Response.Headers["X-Content-Type-Options"] = "nosniff";
            Response.Headers["Cache-Control"] = "no-store";
            string downloadUrl = string.IsNullOrEmpty(fileId) ? string.Empty : $"/system/annexesfile/{fileId}";
            return Content(PreviewFallbackPage.Render(null, downloadUrl, message), "text/html; charset=utf-8");
        }
        
        
        /// <summary>
        /// 文件预览
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        [HttpGet("system/annexesfile/html/{fileId}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> HtmlFile(string fileId)
        {
            string html = await _annexesFileIBLL.ToHtml(fileId);
            return Success(html);
        }
        #endregion
    }
}
