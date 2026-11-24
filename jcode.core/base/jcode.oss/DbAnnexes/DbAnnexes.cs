using System;
using jcode.util;
using System.IO;

namespace jcode.oss
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2022-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.10.08
    /// 描 述：附件操作实现（基于数据库存储）
    /// </summary>
    public class DbAnnexes: IAnnexes
    {

        private readonly DbAnnexesHelper dbAnnexesHelper = new DbAnnexesHelper();
        
        /// <summary>
        /// 上传附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunk">分片序号</param>
        /// <param name="bytes">文件二进制数据</param>
        public bool UploadFileChunk(string fileId, int chunk, byte[] bytes) {
            
            
            try
            {
                dbAnnexesHelper.SaveEntity(new DbAnnexesEntity()
                {
                    F_FileId = fileId,
                    F_Index = chunk,
                    F_Content = bytes
                });
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        /// <summary>
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        public void RemoveFileChunk(string fileId, int chunks)
        {
            for (int i = 0; i < chunks; i++)
            {
                dbAnnexesHelper.Delete(fileId,i);
            }
        }

        /// <summary>
        /// 合并上传附件的分片数据
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="chunks">文件分片数</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <param name="isNotUpload">不上传（不是本地存储时有效）</param>
        /// <returns></returns>
        public string MergeFileChunk(string fileId, string filePath, string fileEex, int chunks,int? isEncrypt, bool isNotUpload = false)
        {
            long filesize = 0;
            filePath = GetPath(filePath);
            string tmpPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/{fileId}{fileEex}";
            //创建一个FileInfo对象
            FileInfo file = new FileInfo(tmpPath);
            //创建文件
            FileStream fs = file.Create();
            for (int i = 0; i < chunks; i++)
            { 
                var entity = dbAnnexesHelper.GetEntity(fileId,i);
                if (entity == null)
                {
                    fs.Close();
                    fs.Dispose();
                    return string.Empty;
                }
                filesize += entity.F_Content.Length;

                if (i == chunks - 1 && !isNotUpload && isEncrypt == 1) //对最后一个分片加密
                {
                    var encryptContent = FileHelper.NewEncryptByte(entity.F_Content, fileId);
                    fs.Write(encryptContent, 0, encryptContent.Length);
                }
                else
                {
                    fs.Write(entity.F_Content, 0, entity.F_Content.Length);
                }


               
                dbAnnexesHelper.Delete(fileId,i);
            }
            //关闭文件流
            fs.Close();
            fs.Dispose();

            try
            {
                var entity = new DbAnnexesEntity()
                {
                    F_FileId = fileId,
                    F_Index = 0,
                    F_Content = FileHelper.Read(tmpPath),
                    F_Path = filePath
                };
                dbAnnexesHelper.SaveEntity(entity);
                
                if (isNotUpload) {
                    return tmpPath;
                }
                else
                {
                    return filesize.ToString();
                }
            }
            catch (System.Exception)
            {

                throw;
            }
            finally {
                if (!isNotUpload)
                {
                    File.Delete(tmpPath);
                }
            }
        }

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="content">文件内容</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <returns></returns>
        public long UploadFile(string fileId, string filePath, string fileEex, byte[] content,int? isEncrypt)
        {
            long filesize = 0;
            
            if (isEncrypt == 1)
            {
                content = FileHelper.NewEncryptByte(content, fileId);
            }

            dbAnnexesHelper.SaveEntity(new DbAnnexesEntity()
            {
                F_FileId = fileId,
                F_Index = 0,
                F_Content = content,
                F_Path = GetPath(filePath)
            });
            filesize += content.Length;
            return filesize;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">上传后的文件地址</param>
        /// <param name="myFilePath">当前文件地址</param>
        public long UploadFile(string filePath, string myFilePath)
        {
            var content = FileHelper.Read(myFilePath);
            dbAnnexesHelper.SaveEntity(new DbAnnexesEntity()
            {
                F_FileId = filePath,
                F_Index = 0,
                F_Content = content,
                F_Path = GetPath(filePath)
            });
            
            return content.Length;
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        public string DownFile(string fileId, string filePath, string fileEex,int? isEncrypt, bool isDelete = false)
        {
            string tempDirectory = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp";
            string tempPath = $"{tempDirectory}/{fileId}{fileEex}";
            if (File.Exists(tempPath))
            {
                return tempPath;
            }

            if (!Directory.Exists(tempDirectory)) {
                Directory.CreateDirectory(tempDirectory);
            }

            var entity = dbAnnexesHelper.GetEntity(GetPath(filePath));
            

            if (entity != null)
            {
                if (isEncrypt == 1)
                {
                    entity.F_Content = FileHelper.NewDecryptByte(entity.F_Content,fileId);
                }
                
                //创建一个FileInfo对象
                FileInfo file = new FileInfo(tempPath);
                //创建文件
                FileStream fs = file.Create();
                fs.Write(entity.F_Content, 0, entity.F_Content.Length);
                //关闭文件流
                fs.Close();
                fs.Dispose();
                
                
                if (isDelete)
                {
                    string tempInfoPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_delete/{fileId}.txt";
                    FileHelper.Write(tempInfoPath, filePath);
                }
                return tempPath;
            }
            else {
                return string.Empty;
            }
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件主键</param>
        /// <returns></returns>
        public void DeleteFile(string filePath) {
            //删除文件
            dbAnnexesHelper.Delete(GetPath(filePath));
        }
        
        

        private string GetPath(string filePath)
        {
            return filePath;
        }

    }
}
