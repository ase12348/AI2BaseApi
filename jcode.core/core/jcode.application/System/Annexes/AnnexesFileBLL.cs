using jcode.autofac;
using jcode.iapplication;
using jcode.office;
using jcode.oss;
using jcode.util;
using jcode.util.NLog;
using Newtonsoft.Json.Linq;
using NLog;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TencentCloud.Cme.V20191029.Models;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：附件管理
    /// </summary>
    public class AnnexesFileBLL : BLLBase, AnnexesFileIBLL, BLL
    {
        private readonly AnnexesFileService annexesFileService = new AnnexesFileService();
        private readonly IAnnexes _iAnnexes;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iAnnexes"></param>
        public AnnexesFileBLL(IAnnexes iAnnexes) {
            _iAnnexes = iAnnexes;
        }

        #region 获取数据
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <returns></returns>
        public Task<IEnumerable<AnnexesFileEntity>> GetList(string folderId)
        {
            return annexesFileService.GetList(folderId);
        }
        /// <summary>
        /// 获取附件名称集合
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <returns></returns>
        public Task<string> GetFileNames(string folderId)
        {
            return annexesFileService.GetFileNames(folderId);
        }
        /// <summary>
        /// 获取附件实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<AnnexesFileEntity> GetEntity(string keyValue)
        {
            return annexesFileService.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取附件实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<AnnexesFileEntity> GetEntityByFolderId(string keyValue)
        {
            var list = (List<AnnexesFileEntity>)await GetList(keyValue);
            if (list.Count == 0)
            {
                return null;
            }
            return list[0];
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据实体
        /// </summary>
        /// <param name="keyValue">附件夹主键</param>
        /// <param name="annexesFileEntity">附件实体数据</param>
        public async Task SaveEntity(string keyValue, AnnexesFileEntity annexesFileEntity)
        {
            if (!string.IsNullOrEmpty(annexesFileEntity.F_Id) && ConfigHelper.GetConfig().IsSecretLevel)
            {
                if (annexesFileEntity.F_Id.Length == 46)
                {
                    annexesFileEntity.F_SecretLevel = int.Parse(annexesFileEntity.F_Id.Substring(45,1));
                }
            }

            await annexesFileService.SaveEntity(keyValue, annexesFileEntity);
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        public async Task DeleteEntity(string fileId)
        {
            var entity = await annexesFileService.GetMyEntity(fileId);
            if (entity != null) {
                await annexesFileService.DeleteEntity(fileId);
                if (!string.IsNullOrEmpty(entity.F_SaveType) && entity.F_SaveType != ConfigHelper.GetConfig().AnnexesType)
                {
                    IocManager.Instance.GetService<IAnnexes>(entity.F_SaveType).DeleteFile(entity.F_FilePath);
                }
                else
                {
                    _iAnnexes.DeleteFile(entity.F_FilePath);
                }
                //文件日志记录
                //NLogHelper.WriteFileLogOther(entity.ToJson(), "lr_base_annexesfile");
            }
        }

        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        public async Task DeleteEntityByFolderId(string folderId)
        {
            await DeleteByFolderIds(new[] { folderId });
        }

        /// <summary>
        /// 按附件夹主键批量删除附件（同时删除存储的物理文件）
        /// 附件夹对应的暂存附件夹会一并清理，避免业务数据删除后遗留未保存的上传
        /// </summary>
        /// <param name="folderIds">附件夹主键集合</param>
        public async Task DeleteByFolderIds(IEnumerable<string> folderIds)
        {
            if (folderIds == null)
            {
                return;
            }

            var deleteFolderIds = new List<string>();
            foreach (var folderId in folderIds.Where(t => !string.IsNullOrWhiteSpace(t)).Distinct())
            {
                deleteFolderIds.Add(folderId);
                if (!AnnexesDraft.IsDraftFolderId(folderId))
                {
                    deleteFolderIds.Add(AnnexesDraft.ToDraftFolderId(folderId));
                }
            }

            foreach (var folderId in deleteFolderIds)
            {
                var list = await annexesFileService.GetList(folderId);
                foreach (var entity in list)
                {
                    await DeleteEntity(entity.F_Id);
                }
            }
        }

        /// <summary>
        /// 回写附件归属信息，供孤儿附件清理时反查业务表
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        public async Task BindOwner(string folderId, string dbCode, string tableName, string tableField)
        {
            if (string.IsNullOrWhiteSpace(folderId) || string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(tableField))
            {
                return;
            }

            await annexesFileService.UpdateOwner(folderId.Trim(), dbCode, tableName, tableField);
        }

        /// <summary>
        /// 将暂存附件夹中的附件转正到真实附件夹，表单保存成功后调用
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        public async Task CommitDraft(string folderId, string dbCode, string tableName, string tableField)
        {
            if (string.IsNullOrWhiteSpace(folderId) || string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(tableField))
            {
                return;
            }

            folderId = folderId.Trim();
            var draftFolderId = AnnexesDraft.ToDraftFolderId(folderId);
            await annexesFileService.PromoteDraft(draftFolderId, folderId, dbCode, tableName, tableField);
        }

        /// <summary>
        /// 丢弃暂存附件夹中的附件（记录与物理文件一并删除），表单取消关闭时调用
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        public async Task DiscardDraft(string folderId)
        {
            if (string.IsNullOrWhiteSpace(folderId))
            {
                return;
            }

            await DeleteByFolderIds(new[] { AnnexesDraft.ToDraftFolderId(folderId.Trim()) });
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <param name="isDb">是否必须存储到数据库</param>
        /// <returns></returns>
        public async Task<AnnexesFileEntity> SaveAnnexes(string folderId, string fileGuid, string fileName, int chunks,string dbCode,string tableName,string tableField,bool isDb = false)
        {

            var isEncryptFile = ConfigHelper.GetConfig().IsEncryptFile ? 1 : 0;
            
            string filePath = GetFilePath(fileGuid, fileName);
            string FileEextension = Path.GetExtension(fileName);
            AnnexesFileEntity fileAnnexesEntity = new AnnexesFileEntity();
            string filesize = string.Empty;
            if (isDb && ConfigHelper.GetConfig().AnnexesType != "Db" )
            {
                var path = _iAnnexes.MergeFileChunk(fileGuid, filePath, FileEextension, chunks,0,true);
                if (!string.IsNullOrEmpty(path))
                {
                    var _filesize = IocManager.Instance.GetService<IAnnexes>("Db").UploadFile(fileGuid,filePath,FileEextension,FileHelper.Read(path),isEncryptFile);
                    if (_filesize != -1)
                    {
                        filesize = _filesize.ToString();
                    }
                    
                    _iAnnexes.DeleteFile(path);
                }
            }
            else
            {
                filesize = _iAnnexes.MergeFileChunk(fileGuid, filePath, FileEextension, chunks,isEncryptFile);;
            }



            if (string.IsNullOrEmpty(filesize))// 表示保存失败
            {
                //_iAnnexes.RemoveFileChunk(fileGuid, chunks);
                return null;
            }
            //文件信息写入数据库
            fileAnnexesEntity.F_Id = fileGuid;
            fileAnnexesEntity.F_FolderId = folderId;
            fileAnnexesEntity.F_FileName = fileName;
            fileAnnexesEntity.F_FilePath = filePath;
            fileAnnexesEntity.F_FileSize = filesize.ToString();
            fileAnnexesEntity.F_FileExtensions = FileEextension;
            fileAnnexesEntity.F_FileType = FileEextension.Replace(".", "");
            fileAnnexesEntity.F_CreateUserId = this.GetUserId();
            fileAnnexesEntity.F_CreateUserName = this.GetUserName();
            fileAnnexesEntity.F_DbCode = dbCode;
            fileAnnexesEntity.F_TableName = tableName;
            fileAnnexesEntity.F_TableField = tableField;
            fileAnnexesEntity.F_SaveType = isDb ? "Db": ConfigHelper.GetConfig().AnnexesType;
            fileAnnexesEntity.F_IsEncrypt = isEncryptFile;
            await SaveEntity(string.Empty, fileAnnexesEntity);
            return fileAnnexesEntity;
        }


        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="bytes">文件数据</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <param name="isEdit">是否可编辑</param>
        /// <returns></returns>
        public async Task<bool> SaveAnnexes(string folderId, string fileGuid, string fileName, byte[] bytes, string dbCode, string tableName, string tableField,bool isEdit = false)
        {
            string filePath = GetFilePath(fileGuid, fileName);
            string FileEextension = Path.GetExtension(fileName);

            var entity =await GetEntity(fileGuid);

            var isEncryptFile = ConfigHelper.GetConfig().IsEncryptFile ? 1 : 0;

            if (entity == null || isEdit) {
                AnnexesFileEntity fileAnnexesEntity = new AnnexesFileEntity();
                long filesize = _iAnnexes.UploadFile(fileGuid, filePath, FileEextension, bytes,isEncryptFile);
                
                //文件信息写入数据库
                fileAnnexesEntity.F_Id = fileGuid;
                fileAnnexesEntity.F_FolderId = folderId;
                fileAnnexesEntity.F_FileName = fileName;
                fileAnnexesEntity.F_FilePath = filePath; //保存相对的地址，方便后续文件的迁移
                fileAnnexesEntity.F_FileSize = filesize.ToString();
                fileAnnexesEntity.F_FileExtensions = FileEextension;
                fileAnnexesEntity.F_FileType = FileEextension.Replace(".", "");
                fileAnnexesEntity.F_CreateUserId = this.GetUserId();
                fileAnnexesEntity.F_CreateUserName = this.GetUserName();
                fileAnnexesEntity.F_DbCode = dbCode;
                fileAnnexesEntity.F_TableName = tableName;
                fileAnnexesEntity.F_TableField = tableField;
                fileAnnexesEntity.F_SaveType = ConfigHelper.GetConfig().AnnexesType;
                fileAnnexesEntity.F_IsEncrypt = isEncryptFile;
                await SaveEntity(entity == null?"": entity.F_Id, fileAnnexesEntity);
            }
            
            return true;
        }

        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="path">文件地址</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <returns></returns>
        public async Task<bool> SaveAnnexes(string folderId, string fileGuid, string path,  string dbCode, string tableName, string tableField)
        {
            string fileName = Path.GetFileName(path);
            string filePath = GetFilePath(fileGuid, Path.GetFileName(path));
            string FileEextension = Path.GetExtension(path);

            var entity = await GetEntity(fileGuid);

            if (entity == null)
            {
                AnnexesFileEntity fileAnnexesEntity = new AnnexesFileEntity();
                var filesize = _iAnnexes.UploadFile(filePath, path);
                //文件信息写入数据库
                fileAnnexesEntity.F_Id = fileGuid;
                fileAnnexesEntity.F_FolderId = folderId;
                fileAnnexesEntity.F_FileName = fileName;
                fileAnnexesEntity.F_FilePath = filePath; //保存相对的地址，方便后续文件的迁移
                fileAnnexesEntity.F_FileSize = filesize.ToString();
                fileAnnexesEntity.F_FileExtensions = FileEextension;
                fileAnnexesEntity.F_FileType = FileEextension.Replace(".", "");
                fileAnnexesEntity.F_CreateUserId = this.GetUserId();
                fileAnnexesEntity.F_CreateUserName = this.GetUserName();
                fileAnnexesEntity.F_DbCode = dbCode;
                fileAnnexesEntity.F_TableName = tableName;
                fileAnnexesEntity.F_TableField = tableField;
                fileAnnexesEntity.F_SaveType = ConfigHelper.GetConfig().AnnexesType;
                await SaveEntity(entity == null ? "" : entity.F_Id, fileAnnexesEntity);
            }

            return true;
        }

        /// <summary>
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <returns></returns>
        public string SaveAnnexes(string fileGuid, string fileName, int chunks)
        {
            string filePath = GetFilePath(fileGuid, fileName);
            string FileEextension = Path.GetExtension(fileName);
            return _iAnnexes.MergeFileChunk(fileGuid, filePath, FileEextension, chunks,0,true);
        }

        /// <summary>
        /// 保存分片附件
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunk">分片文件序号</param>
        /// <param name="bytes">文件流</param>
        public bool SaveChunkAnnexes(string fileGuid, int chunk, byte[] bytes)
        {
            return _iAnnexes.UploadFileChunk(fileGuid, chunk, bytes);
        }
        /// <summary>
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        public void RemoveChunkAnnexes(string fileGuid, int chunks)
        {
            _iAnnexes.RemoveFileChunk(fileGuid, chunks);
        }
        
        /// <summary>
        /// 获取下载文件实体
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        public async Task<AnnexesFileEntity> GetDownFileEntity(string fileId) {
            var data = await GetEntity(fileId);
            if (data == null)
            {
                // 说明上传的是附件夹主键
                var list = (List<AnnexesFileEntity>)await GetList(fileId);
                if (list.Count == 0)
                {
                    return null;
                }
                data = list[0];
            }

            return data;
        }


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        public async Task<string> DownFile(string fileId) {
            var data = await GetEntity(fileId);
            if (data == null)
            {
                // 说明上传的是附件夹主键
                var list = (List<AnnexesFileEntity>)await GetList(fileId);
                if (list.Count == 0)
                {
                    return "";
                }
                data = list[0];
            }

            return DownFile(data);
        }
        
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="data">文件</param>
        /// <returns></returns>
        public string DownFile(AnnexesFileEntity data) {

            if (data == null)
            {
                return  string.Empty;
            }

            // 密级控制文件访问权限
            if (ConfigHelper.GetConfig().IsSecretLevel && data.F_SecretLevel != null && data.F_SecretLevel > this.GetSecretLevel())
            {
                throw new Exception("#NotLog#当前文件密级高于账户权限！"); 
            }

            string path = string.Empty;
            
            if (!string.IsNullOrEmpty(data.F_SaveType) && data.F_SaveType != ConfigHelper.GetConfig().AnnexesType)
            {
                path = IocManager.Instance.GetService<IAnnexes>(data.F_SaveType).DownFile(data.F_Id, data.F_FilePath, data.F_FileExtensions,data.F_IsEncrypt);
            }
            else
            {
                path = _iAnnexes.DownFile(data.F_Id, data.F_FilePath, data.F_FileExtensions,data.F_IsEncrypt);
            }

            //NLogHelper.WriteFileLogOther(data.ToJson(), "lr_base_annexesfile");
            return path;
        }


        /// <summary>
        /// 删除临时文件
        /// </summary>
        public void DeleteTempFile()
        {
            var now = DateTime.Now;

            // 删除超过30分钟的临时文件
            var path = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp";
            if (Directory.Exists(path)) {
                var fileList = Directory.GetFiles(path);
                foreach (var item in fileList)
                {
                    FileInfo fileInfo = new FileInfo(item);
                    if (fileInfo.CreationTime.AddMinutes(30) <= now)
                    {
                        fileInfo.Delete();
                    }
                }
            }


            

            // 删除超过半小时的上传文件（需要删除的）
            var path2 = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_delete";
            if (Directory.Exists(path2))
            {
                var fileList2 = Directory.GetFiles(path2);
                foreach (var item in fileList2)
                {
                    var pathInfo = File.ReadAllText(item);
                    FileInfo fileInfo = new FileInfo(item);
                    if (fileInfo.CreationTime.AddMinutes(30) <= now)
                    {
                        _iAnnexes.DeleteFile(pathInfo);
                        fileInfo.Delete();
                    }
                }
            }
            
            
            // 删除预览文件
            var path3 = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_local_delete";
            if (Directory.Exists(path3))
            {
                var fileList3 = Directory.GetFiles(path3);
                foreach (var item in fileList3)
                {
                    var pathInfo = File.ReadAllText(item);
                    FileInfo fileInfo = new FileInfo(item);
                    if (fileInfo.CreationTime.AddMinutes(10) <= now)
                    {
                        if (File.Exists(pathInfo))
                        {
                            File.Delete(pathInfo);
                        }
                        fileInfo.Delete();
                    }
                }
            }
        }

        /// <summary>
        /// 获取文件路径
        /// </summary>
        /// <param name="fileGuid">文件id</param>
        /// <param name="fileName">文件名称</param>
        /// <returns></returns>
        private string GetFilePath(string fileGuid,string fileName) {
            string tenantNo = "";
            if (ConfigHelper.GetConfig().IsSass)
            {
                tenantNo = this.GetTenantNo();
            }
            if (tenantNo == "host")
            {
                tenantNo = "";
            }
            //string filePath = string.Format("{0}/{1}_{2}", this.GetUserAccount(), fileGuid, fileName);
            string filePath = string.Format("{0}/{1}{2}", this.GetUserAccount(), fileGuid,Path.GetExtension(fileName));

            if (!string.IsNullOrEmpty(tenantNo)) {
                filePath = $"{tenantNo}/{filePath}";
            }

            return filePath;
        }


        #endregion

        #region 文件预览处理
        /// <summary>
        /// 获取预览方式，前端据此选择渲染器（不读取文件内容）
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<AnnexesFilePreviewMetaDto> GetPreviewMeta(string fileId) {
            var data = await GetDownFileEntity(fileId);
            if (data == null)
            {
                throw new Exception("文件不存在或已删除！");
            }
            string ext = GetPreviewExt(data);
            var plan = ResolvePreviewPlan(ext);
            long size = 0;
            long.TryParse(data.F_FileSize, out size);
            return new AnnexesFilePreviewMetaDto
            {
                FileId = data.F_Id,
                FileName = data.F_FileName,
                Ext = ext,
                Size = size,
                Mode = plan.Mode,
                CanPreview = plan.Mode != AnnexesFilePreviewMode.Download,
                Reason = plan.Reason,
                ReasonMessage = GetPreviewReasonMessage(plan.Reason),
                DownloadUrl = $"/system/annexesfile/{data.F_Id}"
            };
        }

        /// <summary>
        /// 获取预览文件地址，按 NPOI → LibreOffice → 下载提示的顺序降级
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="isNotCache">为 1 时忽略已有转换结果强制重转</param>
        /// <returns></returns>
        /// <exception cref="PreviewConvertException"></exception>
        public async Task<string> ToPreview(string fileId,int? isNotCache) {
            var data = await GetDownFileEntity(fileId);
            if (data == null) {
                throw new PreviewConvertException(PreviewErrorCode.FileMissing, "该文件已经丢失！");
            }

            string ext = GetPreviewExt(data);
            var plan = ResolvePreviewPlan(ext);
            if (plan.Mode == AnnexesFilePreviewMode.Download) {
                throw new PreviewConvertException(plan.Reason, GetPreviewReasonMessage(plan.Reason));
            }

            string path = DownFile(data);
            if (string.IsNullOrEmpty(path) || !File.Exists(path)) {
                throw new PreviewConvertException(PreviewErrorCode.FileMissing, "该文件已经丢失！");
            }

            // 浏览器可直接显示、或交给前端渲染的格式，直接返回原始文件
            if (plan.Mode == AnnexesFilePreviewMode.Image
                || plan.Mode == AnnexesFilePreviewMode.Docx
                || plan.Mode == AnnexesFilePreviewMode.Pptx
                || (plan.Mode == AnnexesFilePreviewMode.Iframe && IsBrowserRenderableExt(ext))) {
                return path;
            }

            var options = ConfigHelper.GetConfig().Preview ?? new PreviewOptions();
            bool isRefresh = isNotCache == 1;
            string directory = $"{Path.GetDirectoryName(path)}/preview";
            string baseName = Path.GetFileNameWithoutExtension(path);

            if (ext == "xls" || ext == "xlsx") {
                string htmlPath = $"{directory}/{baseName}.html";
                // 后缀带版本号：渲染结构变化时递增，避免继续复用旧版产物
                string npoiHtmlPath = $"{directory}/{baseName}.npoi2.html";

                // 1. NPOI 直接渲染
                if (options.EnableNpoi && IsWithinNpoiLimit(path, options)) {
                    if (!isRefresh && File.Exists(npoiHtmlPath)) {
                        return npoiHtmlPath;
                    }
                    try {
                        string html = ExcelNpoiHtmlRenderer.Render(path, options);
                        Directory.CreateDirectory(directory);
                        File.WriteAllText(npoiHtmlPath, html, new UTF8Encoding(false));
                        RegisterPreviewFile(fileId, npoiHtmlPath);
                        return npoiHtmlPath;
                    } catch (PreviewConvertException) {
                        // 加密表格、特殊结构等 NPOI 解析不了，继续走 LibreOffice
                    }
                }

                // 2. LibreOffice 转换，未安装时抛 lo_missing 由上层给出下载提示
                if (!isRefresh && File.Exists(htmlPath)) {
                    return htmlPath;
                }
                DocumentConverter.ToHtml(path, directory);
                if (!File.Exists(htmlPath)) {
                    throw new PreviewConvertException(PreviewErrorCode.LibreOfficeFailed, "文件转换失败，请下载后查看");
                }
                RegisterPreviewFile(fileId, htmlPath);
                return htmlPath;
            }

            // doc/ppt/dxf 等：LibreOffice 统一转 PDF
            string pdfPath = $"{directory}/{baseName}.pdf";
            if (!isRefresh && File.Exists(pdfPath)) {
                return pdfPath;
            }
            DocumentConverter.ToPdf(path, directory);
            if (!File.Exists(pdfPath)) {
                throw new PreviewConvertException(PreviewErrorCode.LibreOfficeFailed, "文件转换失败，请下载后查看");
            }
            RegisterPreviewFile(fileId, pdfPath);
            return pdfPath;
        }

        /// <summary>
        /// 获取文件后缀（小写，不含点）
        /// </summary>
        private static string GetPreviewExt(AnnexesFileEntity data) {
            string ext = (data.F_FileExtensions ?? string.Empty).Trim().TrimStart('.').ToLower();
            if (string.IsNullOrEmpty(ext)) {
                ext = Path.GetExtension(data.F_FileName ?? string.Empty).TrimStart('.').ToLower();
            }
            return ext;
        }

        /// <summary>
        /// 浏览器可直接显示的原始文件（xls/xlsx、doc 等仍必须服务端转换）
        /// </summary>
        private static bool IsBrowserRenderableExt(string ext) {
            switch (ext) {
                case "pdf":
                case "txt":
                case "csv":
                case "json":
                case "html":
                case "htm":
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// 解析预览方式与不可预览原因
        /// </summary>
        private static (string Mode, string Reason) ResolvePreviewPlan(string ext) {
            var options = ConfigHelper.GetConfig().Preview ?? new PreviewOptions();
            switch (ext)
            {
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                case "bmp":
                case "webp":
                case "svg":
                    return (AnnexesFilePreviewMode.Image, null);
                case "pdf":
                case "txt":
                case "csv":
                case "json":
                case "html":
                case "htm":
                    return (AnnexesFilePreviewMode.Iframe, null);
                case "xls":
                case "xlsx":
                    if (options.EnableNpoi || DocumentConverter.IsAvailable()) {
                        return (AnnexesFilePreviewMode.Iframe, null);
                    }
                    return (AnnexesFilePreviewMode.Download, PreviewErrorCode.LibreOfficeMissing);
                case "docx":
                    return (AnnexesFilePreviewMode.Docx, null);
                case "pptx":
                    if (options.EnableClientPptx) {
                        return (AnnexesFilePreviewMode.Pptx, null);
                    }
                    break;
                case "doc":
                case "ppt":
                case "rtf":
                case "odt":
                case "odp":
                case "ods":
                case "dxf":
                    break;
                default:
                    return (AnnexesFilePreviewMode.Download, PreviewErrorCode.Unsupported);
            }

            if (DocumentConverter.IsAvailable()) {
                return (AnnexesFilePreviewMode.Iframe, null);
            }
            return (AnnexesFilePreviewMode.Download, PreviewErrorCode.LibreOfficeMissing);
        }

        /// <summary>
        /// 原因码对应的提示文案
        /// </summary>
        private static string GetPreviewReasonMessage(string reason) {
            switch (reason)
            {
                case null:
                case "":
                    return null;
                case PreviewErrorCode.LibreOfficeMissing:
                    return "服务器未安装 LibreOffice，无法在线预览该格式，请下载后查看";
                case PreviewErrorCode.Unsupported:
                    return "该文件格式暂不支持在线预览，请下载后查看";
                case PreviewErrorCode.FileMissing:
                    return "文件已丢失，无法预览";
                default:
                    return "该文件暂时无法在线预览，请下载后查看";
            }
        }

        private static bool IsWithinNpoiLimit(string path, PreviewOptions options) {
            try {
                return new FileInfo(path).Length <= Math.Max(1, options.MaxFileSizeMb) * 1024L * 1024L;
            } catch {
                return false;
            }
        }

        /// <summary>
        /// 登记预览产物，由 DeleteTempFile 定时清理
        /// </summary>
        private void RegisterPreviewFile(string fileId, string previewPath) {
            try {
                string tempInfoPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_local_delete/{fileId}_preview.txt";
                FileHelper.Write(tempInfoPath, previewPath);
            } catch {
                // 清理登记失败不影响本次预览
            }
        }
        
        
        /// <summary>
        /// 获取文件的html格式，目前只支持  excel word ppt
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<string> ToHtml(string fileId) {
            
            
            var path =await DownFile(fileId);
            if (string.IsNullOrEmpty(path)) {
                throw new Exception("该文件已经丢失！");
            }
            string fileType = Path.GetExtension(path).Replace(".", "");
            string directory = $"{Path.GetDirectoryName(path)}/preview";
            string res = $"{Path.GetDirectoryName(path)}/preview/{Path.GetFileNameWithoutExtension(path)}.html";
            string html = "";
            switch (fileType)
            {
                case "xls":
                case "xlsx":
                case "doc":
                case "ppt":
                case "pptx":
                case "docx":
                    if (!File.Exists(res)) {
                        DocumentConverter.ToHtml(path, directory);
                    }
                    html = File.ReadAllText(res);
                    File.Delete(res);
                    break;
            }
            return html;
        }
        #endregion
        
        /// <summary>
        /// 存储转化
        /// </summary>
        public async Task TransformStorage()
        {
            var list = await annexesFileService.GetAllList();
            foreach (var item in list)
            {
                if (item.F_SaveType  != "Db")
                {
                    var path = await DownFile(item.F_Id);

                    if (File.Exists(path))
                    {
                        var filesize = IocManager.Instance.GetService<IAnnexes>("Db").UploadFile(item.F_Id,
                            item.F_FilePath, item.F_FileExtensions, FileHelper.Read(path),item.F_SecretLevel);
                        if (filesize != -1)
                        {
                            _iAnnexes.DeleteFile(path);
                            item.F_SaveType = "Db";
                            await SaveEntity(item.F_Id, item);
                        }
                    }
                    else
                    {
                        await DeleteEntity(item.F_Id);
                    }
                }
                else
                {
                    /*var path = await DownFile(item.F_Id);
                    if (FileHelper.Read(path).Length.ToString() != item.F_FileSize && item.F_IsEncrypt != 1)
                    {
                        item.F_IsEncrypt = 1;
                        await SaveEntity(item.F_Id, item);
                    }*/

                    /*var content =await annexesFileService.BaseRepository()
                        .FindEntity<DbAnnexesEntity>(t => t.F_FileId == item.F_Id);
                    if (content.F_Content.Length == 0)
                    {
                        await DeleteEntity(item.F_Id);
                    }*/

                    if (string.IsNullOrEmpty(item.F_TableName) 
                        && item.F_FolderId != "16086ccb-91be-4a93-851c-e5f1a9605b2e"
                        && item.F_FolderId != "b158c175-6ca5-475e-93a6-53ba77f92821"
                        && item.F_FolderId != "3b26afdf-9a39-4996-835d-ccf15e8d7358"
                        && item.F_FolderId != "6beb51cf-f81b-4fa6-8655-9cfc1a47dfb9"
                        && item.F_FolderId != "2b56d8ee-d74c-4102-8d64-3173a661c0d3"
                        && item.F_FolderId != "6df7874a-6cd1-46c6-9d85-96347dc90789"
                        && item.F_FolderId != "6e6fd430-5247-4371-88ff-4a822a200ede")
                    {
                        /*var dt = await annexesFileService.BaseRepository(item.F_DbCode).FindTable(
                            $"select * from {item.F_TableName} where {item.F_TableField} = '{item.F_FolderId}'");
                        if (dt.Rows.Count == 0)
                        {
                            await DeleteEntity(item.F_Id);
                        }*/
                        
                        await DeleteEntity(item.F_Id);
                    }

                    

                }
            }
        }

        #region 孤儿附件清理
        /// <summary>
        /// 单次清理的最大附件数量
        /// </summary>
        private const int OrphanBatchSize = 50;
        /// <summary>
        /// 单次清理的最大扫描数量
        /// </summary>
        private const int OrphanMaxRows = 2000;

        /// <summary>
        /// 清理孤儿附件：仅处理已登记归属、超过保留期且业务表中已无引用的附件
        /// </summary>
        /// <returns>实际删除的附件数量</returns>
        public async Task<int> CleanOrphanFiles()
        {
            var retentionDays = ConfigHelper.GetConfig().AnnexesOrphanDays;
            if (retentionDays <= 0)
            {
                retentionDays = 7;
            }

            var candidates = (await annexesFileService.GetOrphanCandidates(DateTime.Now.AddDays(-retentionDays), OrphanMaxRows)).ToList();
            if (candidates.Count == 0)
            {
                return 0;
            }

            int deletedCount = 0;
            foreach (var group in candidates.GroupBy(t => new
                     {
                         DbCode = t.F_DbCode ?? string.Empty,
                         t.F_TableName,
                         t.F_TableField
                     }))
            {
                // 归属信息来自附件表，查询前必须校验标识符，避免拼接出非预期 SQL
                if (!IsSafeIdentifier(group.Key.F_TableName) || !IsSafeIdentifier(group.Key.F_TableField))
                {
                    continue;
                }

                var folderIds = group.Select(t => t.F_FolderId)
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Distinct()
                    .ToList();

                HashSet<string> referencedFolderIds;
                try
                {
                    referencedFolderIds = await GetReferencedFolderIds(group.Key.DbCode, group.Key.F_TableName, group.Key.F_TableField, folderIds);
                }
                catch (Exception)
                {
                    // 业务表查询失败时整组跳过，宁可漏删也不能误删
                    continue;
                }

                foreach (var entity in group)
                {
                    if (string.IsNullOrWhiteSpace(entity.F_FolderId) || referencedFolderIds.Contains(entity.F_FolderId))
                    {
                        continue;
                    }

                    await DeleteEntity(entity.F_Id);
                    deletedCount++;
                }
            }

            return deletedCount;
        }

        /// <summary>
        /// 回收遗留的暂存附件：表单关闭后未被转正、超过保留时长的暂存附件
        /// 暂存附件夹不会被业务数据引用，因此无需反查业务表
        /// </summary>
        /// <returns>实际删除的附件数量</returns>
        public async Task<int> CleanDraftFiles()
        {
            var retentionHours = ConfigHelper.GetConfig().AnnexesDraftHours;
            if (retentionHours <= 0)
            {
                retentionHours = 24;
            }

            var candidates = (await annexesFileService.GetDraftCandidates(
                DateTime.Now.AddHours(-retentionHours), AnnexesDraft.FolderSuffix, OrphanMaxRows)).ToList();

            int deletedCount = 0;
            foreach (var entity in candidates)
            {
                // 二次确认后缀，避免表达式翻译差异导致误删正式附件
                if (!AnnexesDraft.IsDraftFolderId(entity.F_FolderId))
                {
                    continue;
                }

                await DeleteEntity(entity.F_Id);
                deletedCount++;
            }

            return deletedCount;
        }

        /// <summary>
        /// 查询业务表中仍然被引用的附件夹主键
        /// </summary>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        /// <param name="folderIds">待校验的附件夹主键集合</param>
        /// <returns></returns>
        private async Task<HashSet<string>> GetReferencedFolderIds(string dbCode, string tableName, string tableField, List<string> folderIds)
        {
            var referencedFolderIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            if (folderIds.Count == 0)
            {
                return referencedFolderIds;
            }

            var repository = string.IsNullOrEmpty(dbCode) ? annexesFileService.BaseRepository() : annexesFileService.BaseRepository(dbCode);

            for (int i = 0; i < folderIds.Count; i += OrphanBatchSize)
            {
                var batch = folderIds.Skip(i).Take(OrphanBatchSize).ToList();
                var likeBatch = new List<string>();
                foreach (var folderId in batch)
                {
                    // 含 LIKE 通配符时无法安全匹配，保守视为仍被引用
                    if (folderId.IndexOf('%') >= 0 || folderId.IndexOf('_') >= 0)
                    {
                        referencedFolderIds.Add(folderId);
                    }
                    else
                    {
                        likeBatch.Add(folderId);
                    }
                }

                if (likeBatch.Count == 0)
                {
                    continue;
                }

                var sql = new StringBuilder($"SELECT {tableField} AS F_Value FROM {tableName} WHERE ");
                var parameters = new List<SugarParameter>();
                for (int j = 0; j < likeBatch.Count; j++)
                {
                    if (j > 0)
                    {
                        sql.Append(" OR ");
                    }

                    sql.Append($"{tableField} LIKE @p{j}");
                    parameters.Add(new SugarParameter($"p{j}", $"%{likeBatch[j]}%"));
                }

                var dataTable = await repository.FindTable(sql.ToString(), parameters.ToArray());
                foreach (DataRow row in dataTable.Rows)
                {
                    if (row[0] == DBNull.Value)
                    {
                        continue;
                    }

                    // 字段可能存 JSON 数组或逗号分隔的多个值，用原文包含判断更保守，宁可漏删不误删
                    var value = Convert.ToString(row[0]);
                    foreach (var folderId in likeBatch)
                    {
                        if (value.IndexOf(folderId, StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            referencedFolderIds.Add(folderId);
                        }
                    }
                }
            }

            return referencedFolderIds;
        }

        /// <summary>
        /// 校验数据库标识符（支持 schema.table 形式）
        /// </summary>
        /// <param name="value">标识符</param>
        /// <returns></returns>
        private static bool IsSafeIdentifier(string value)
        {
            return !string.IsNullOrEmpty(value) && Regex.IsMatch(value, @"^[A-Za-z_][A-Za-z0-9_\.]*$");
        }
        #endregion
    }
}
