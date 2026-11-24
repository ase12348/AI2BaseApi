using jcode.util;
using System.IO;

namespace jcode.oss
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2022-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.10.08
    /// 描 述：附件操作实现（基于阿里云对象存储实现）
    /// </summary>
    public class AliOssAnnexes: IAnnexes
    {
        private static readonly AliyunOssHelper _aliyunOssHelper = new AliyunOssHelper();
        /// <summary>
        /// 上传附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunk">分片序号</param>
        /// <param name="bytes">文件二进制数据</param>
        public bool UploadFileChunk(string fileId, int chunk, byte[] bytes) {
            MemoryStream ms = new MemoryStream(bytes);
            try
            {
                return _aliyunOssHelper.PutObject($"jcode_chunk_cache/{chunk}_{fileId}", ms);
            }
            finally
            {
                ms.Close();
                ms.Dispose();
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
                _aliyunOssHelper.DeleteObject($"jcode_chunk_cache/{i}_{fileId}");
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
        public string MergeFileChunk( string fileId, string filePath,string fileEex, int chunks,int? isEncrypt, bool isNotUpload = false) {
            long filesize = 0;
            string tmpPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/{fileId}{fileEex}";
            //创建一个FileInfo对象
            FileInfo file = new FileInfo(tmpPath);
            //创建文件
            FileStream fs = file.Create();
            for (int i = 0; i < chunks; i++)
            {
                byte[] bufferByRedis = _aliyunOssHelper.GetObject($"jcode_chunk_cache/{i}_{fileId}");
                if (bufferByRedis == null)
                {
                    return string.Empty;
                }
                if (i== chunks-1 && !isNotUpload && isEncrypt == 1)//对最后一个分片加密
                {
                    //对字节数组的最后一组数据进行加密并且返回全部字节数组
                    var encryptContent = FileHelper.NewEncryptByte(bufferByRedis, fileId);
                    fs.Write(encryptContent, 0, encryptContent.Length);
                }
                else
                {
                    fs.Write(bufferByRedis, 0, bufferByRedis.Length);
                }
                filesize += bufferByRedis.Length;
                _aliyunOssHelper.DeleteObject($"jcode_chunk_cache/{i}_{fileId}");
            }
            //关闭文件流
            fs.Close();
            fs.Dispose();

            try
            {
                if (!isNotUpload) {
                    if (_aliyunOssHelper.MultipartUpload(GetPath(filePath), tmpPath))
                    {
                        return filesize.ToString();
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
                return tmpPath;

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

            string tmpPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/{fileId}{fileEex}";



            //创建一个FileInfo对象
            FileInfo file = new FileInfo(tmpPath);
            //创建文件
            FileStream fs = file.Create();
            //写入二进制流
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
            fs.Dispose();

            try
            {

                if (_aliyunOssHelper.MultipartUpload(GetPath(filePath), tmpPath))
                {
                    return filesize;
                }
                else
                {
                    return -1;
                }
            }
            catch (System.Exception)
            {

                throw;
            }
            finally
            {
                File.Delete(tmpPath);
            }
        }

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">上传后的文件地址</param>
        /// <param name="myFilePath">当前文件地址</param>
        public long UploadFile(string filePath, string myFilePath)
        {
            _aliyunOssHelper.MultipartUpload(GetPath(filePath), myFilePath);
            FileInfo file = new FileInfo(myFilePath);
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
        public string DownFile(string fileId, string filePath, string fileEex,int? isEncrypt, bool isDelete = false) {
            string tempDirectory = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp";
            string tempPath = $"{tempDirectory}/{fileId}{fileEex}";
            if (File.Exists(tempPath))
            {
                return tempPath;
            }

            if (!Directory.Exists(tempDirectory)) {
                Directory.CreateDirectory(tempDirectory);
            }

            if (_aliyunOssHelper.WriteObject(GetPath(filePath), tempPath))
            {
                if (isDelete)
                {
                    string tempInfoPath = $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/temp_delete/{fileId}.txt";
                    FileHelper.Write(tempInfoPath, filePath);
                }

                if (isEncrypt == 1)
                {
                    FileHelper.NewDecryptByte(tempPath,fileId);
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
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public void DeleteFile(string filePath) {
            _aliyunOssHelper.DeleteObject(GetPath(filePath));
        }



        private string GetPath(string filePath)
        {
            return filePath;
        }
    }
}
