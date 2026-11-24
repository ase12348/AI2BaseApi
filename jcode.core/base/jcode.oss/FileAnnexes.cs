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
    /// 描 述：附件操作实现（基于本地文件）
    /// </summary>
    public class FileAnnexes: IAnnexes
    {
        /// <summary>
        /// 上传附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunk">分片序号</param>
        /// <param name="bytes">文件二进制数据</param>
        public bool UploadFileChunk(string fileId, int chunk, byte[] bytes) {
            try
            {
                FileHelper.WriteCache(chunk + "_" + fileId, bytes);
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
                FileHelper.RemoveCache(i + "_" + fileId);
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

            //创建文件夹
            string path = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //创建一个FileInfo对象
            FileInfo file = new FileInfo(filePath);
            //创建文件
            FileStream fs = file.Create();
            for (int i = 0; i < chunks; i++)
            {
                byte[] bufferByRedis = FileHelper.ReadCache(i + "_" + fileId);
                if (bufferByRedis == null)
                {
                    return string.Empty;
                }
                
                if (i== chunks-1 && !isNotUpload && isEncrypt == 1)//对最后一个分片加密
                {
                    //对字节数组的最后一组数据进行加密并且返回全部字节数组
                    var encryptContent = FileHelper.NewEncryptByte(bufferByRedis, fileId);
                    //写入二进制流-文件流加密上传----ADD BY SSY 20230403
                    fs.Write(encryptContent, 0, encryptContent.Length);
                }
                else
                {
                    fs.Write(bufferByRedis, 0, bufferByRedis.Length);
                }
                

                filesize += bufferByRedis.Length;
                FileHelper.RemoveCache(i + "_" + fileId);
            }


            //关闭文件流
            fs.Close();
            if (!isNotUpload)
            {
                return filesize.ToString();
            }
            else {
                return filePath;
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

            filePath = GetPath(filePath);

            //创建文件夹
            var directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            //创建一个FileInfo对象
            FileInfo file = new FileInfo(filePath);
            //创建文件
            FileStream fs = file.Create();
            if (isEncrypt == 1)
            {
                //对字节数组的最后一组数据进行加密并且返回全部字节数组
                var encryptContent = FileHelper.NewEncryptByte(content, fileId);
                //写入二进制流
                fs.Write(encryptContent, 0, encryptContent.Length);
            }
            else
            {
                fs.Write(content, 0, content.Length);
            }

            
            filesize += content.Length;
            //关闭文件流
            fs.Close();
            return filesize;
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">上传后的文件地址</param>
        /// <param name="myFilePath">当前文件地址</param>
        public long UploadFile(string filePath, string myFilePath) {
            filePath = GetPath(filePath);
            //创建文件夹
            var path = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            //创建一个FileInfo对象
            var file = new FileInfo(myFilePath);
            file.CopyTo(filePath);
            
            return file.Length;
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
            filePath = GetPath(filePath);
            if (!File.Exists(filePath)) {
                return string.Empty;
            }
            
            if (isEncrypt == 1)
            {
                var directory = Path.GetDirectoryName(filePath);
                var tempDirectory = $"{directory}/_temp";
                var tempPath = $"{tempDirectory}/{fileId}{fileEex}";
            
                if (!Directory.Exists(tempDirectory))
                {
                    Directory.CreateDirectory(tempDirectory);
                }

                var file = new FileInfo(filePath);
                file.CopyTo(tempPath,true);
                
                FileHelper.NewDecryptByte(tempPath,fileId);
                
                if (isDelete)
                {
                    File.Delete(filePath);
                }
                string tempInfoPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_local_delete/{fileId}_temp.txt";
                FileHelper.Write(tempInfoPath, tempPath);
                
                return tempPath;
            }
            
            if (isDelete)
            {
                string tempInfoPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_delete/{fileId}.txt";
                FileHelper.Write(tempInfoPath, filePath);
            }
            
            return filePath;
        }


        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件主键</param>
        /// <returns></returns>
        public void DeleteFile(string filePath) {
            filePath = GetPath(filePath);
            //删除文件
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }


        private string GetPath(string filePath) {
            filePath = $"{ConfigHelper.GetConfig().FilePath}/Jcode_ANNEXESFILE/{filePath}";
            return filePath;
        }
    }
}
