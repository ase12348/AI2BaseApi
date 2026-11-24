using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.18
    /// 描 述：文件帮助类
    /// </summary>
    public static class FileHelper
    {
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">文件地址（绝对地址）</param>
        /// <param name="content">内容</param>
        /// <param name="append">是否新增内容</param>
        public static void Write(string path, string content, bool append = false)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            StreamWriter sw = new StreamWriter(path, append, System.Text.Encoding.GetEncoding("utf-8"));
            sw.Write(content);
            sw.Close();
            sw.Dispose();
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">文件地址（绝对地址）</param>
        /// <param name="buffer">内容</param>
        public static void Write(string path, byte[] buffer)
        {
            FileInfo fi = new FileInfo(path);
            var di = fi.Directory;
            if (!di.Exists)
            {
                di.Create();
            }
            FileStream fs = fi.Create();
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();
        }


        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="path">文件地址（绝对地址）</param>
        /// <param name="formFile">表单提交文件对象</param>
        public static void Write(string path, IFormFile formFile)
        {
            byte[] bytes = new byte[formFile.Length];
            MemoryStream ms = new MemoryStream();
            formFile.CopyTo(ms);
            ms.Flush();
            ms.Position = 0;
            ms.Read(bytes, 0, bytes.Length);
            ms.Close();
            ms.Dispose();

            Write(path, bytes);
        }

        /// <summary>
        /// 读取根目录下面的文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        public static byte[] ReadRoot(string filePath) {
            string rootPath = ConfigHelper.GetValue<string>("baseDir") + "/wwwroot";
            string path = rootPath + filePath;
            return File.ReadAllBytes(path);
        }
        
        /// <summary>
        /// 读取缓存文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="buffer">文件数据</param>
        /// <returns></returns>
        public static void WriteRoot(string fileName, byte[] buffer)
        {
            string rootPath = ConfigHelper.GetValue<string>("baseDir") + "/wwwroot";
            if (!Directory.Exists(rootPath)) {
                Directory.CreateDirectory(rootPath);
            }
            string path = rootPath + fileName;
            FileInfo file = new FileInfo(path);
            FileStream fs = file.Create();
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();
        }
        
        
        
        /// <summary>
        /// 读取缓存文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static byte[] ReadCache(string fileName)
        {
            string rootPath = ConfigHelper.GetConfig().FilePath + "/JCODE_CACHE/";
            string path = rootPath + fileName;
            return File.ReadAllBytes(path);
        }
        
        

        /// <summary>
        /// 读取缓存文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <param name="buffer">文件数据</param>
        /// <returns></returns>
        public static void WriteCache(string fileName, byte[] buffer)
        {
            string rootPath = ConfigHelper.GetConfig().FilePath + "/JCODE_CACHE/";
            if (!Directory.Exists(rootPath)) {
                Directory.CreateDirectory(rootPath);
            }
            string path = rootPath + fileName;
            FileInfo file = new FileInfo(path);
            
            FileStream fs = file.Create();
            fs.Write(buffer, 0, buffer.Length);
            fs.Close();
            fs.Dispose();
        }


        /// <summary>
        /// 读取缓存文件
        /// </summary>
        /// <param name="fileName">文件路径</param>
        /// <returns></returns>
        public static void RemoveCache(string fileName)
        {
            string rootPath = ConfigHelper.GetConfig().FilePath + "/JCODE_CACHE/";
            string path = rootPath + fileName;
            File.Delete(path);
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath">文件绝对路径</param>
        /// <returns></returns>
        public static byte[] Read(string filePath)
        {
            return File.ReadAllBytes(filePath);
        }

        /// <summary>
        /// 获取文件的传输类型
        /// </summary>
        /// <param name="fileExt">文件扩展名</param>
        /// <returns></returns>
        public static string getContentType(string fileExt) {
            string contentType;
            switch (fileExt?.ToLower())
            {
                case "jpg":
                case "jpeg":
                case "gif":
                case "png":
                case "webp":
                    contentType = "image/" + fileExt.ToLower();
                    break;
                case "bmp":
                    contentType = "application/x-bmp";
                    break;
                case "pdf":
                    contentType = "application/" + fileExt.ToLower();
                    break;
                case "txt":
                    contentType = "text/plain;charset=utf-8";
                    break;
                case "csv":
                    contentType = "text/csv";
                    break;
                case "svg":
                    contentType = "image/svg+xml";
                    break;
                case "html":
                    contentType = "text/html"; 
                    break;
                case "wav":
                case "mp3":
                case "mp4":
                    contentType = "audio/" + fileExt.ToLower();
                    break;
                default:
                    contentType = "application/octet-stream";
                    break;
            }

            return contentType;
        }

        #region 扩展方法
        /// <summary>
        /// 32位处理key 
        /// </summary>
        /// <param name="key">原字节</param>
        /// <returns></returns>
        private static string GetAesKey(string key)
        {
            var len = key.Length;
            if (len < 32)
            {
                for (var i = 0; i < 32 - len; i++)
                {
                    key += "0";
                }
                return key;
            }
            else
            {
                return key[..32];
            }
        }
        /// <summary> 
        /// 加密附件数据-【字节数组】-ADD BY SSY 20230403
        /// </summary> 
        /// <param name="inputByteArray">需要加密的字节数组</param> 
        /// <param name="sKey">秘钥</param>
        /// <param name="vi"></param> 
        /// <returns></returns> 
        public static byte[] EncryptByte(byte[] inputByteArray, string sKey, string vi = "1234567890000000")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(GetAesKey(sKey));
            byte[] ivArray = Encoding.UTF8.GetBytes(vi);
            byte[] toEncryptArray = inputByteArray;

            SymmetricAlgorithm des = Aes.Create();
            des.IV = ivArray;
            des.Key = keyArray;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;
            ICryptoTransform cTransform = des.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return resultArray;
        }
        /// <summary> 
        /// 解密附件数据 -【字节数组】-ADD BY SSY 20230403
        /// </summary> 
        /// <param name="inputByteArray">需要解密的文件流</param> 
        /// <param name="sKey">秘钥</param>
        /// <param name="vi"></param> 
        /// <returns></returns> 
        public static byte[] DecryptByte(byte[] inputByteArray, string sKey, string vi = "1234567890000000")
        {
            byte[] keyArray = Encoding.UTF8.GetBytes(GetAesKey(sKey));
            byte[] ivArray = Encoding.UTF8.GetBytes(vi);
            byte[] toEncryptArray = inputByteArray;

            SymmetricAlgorithm des = Aes.Create();
            des.IV = ivArray;
            des.Key = keyArray;
            des.Mode = CipherMode.CBC;
            des.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = des.CreateDecryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return resultArray;
        }
        /// <summary>
        /// 将最后一组字节加密后返回全部
        /// </summary>
        /// <param name="inputByteArray">文件流</param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static byte[] NewEncryptByte(byte[] inputByteArray, string fileId)
        {
            var len = 1024*512; // 默认加密字节数据
            if (inputByteArray.Length < len)
            {
                len = inputByteArray.Length;
            }
            var otherLen = inputByteArray.Length - len;
            
            //获取开头数组
            var beginByte = new byte[otherLen];
            //源缓冲区-基于零的字节偏移到src中-目标缓冲区-基于零的字节偏移到dst中-要复制的字节数
            Buffer.BlockCopy(inputByteArray, 0, beginByte, 0,  otherLen);
            //获取最后一组数组
            var endByte = new byte[len];
            Buffer.BlockCopy(inputByteArray, otherLen, endByte, 0, len);//获取最后一组数组
            //生成解密后的字节数组
            var encryptByte = EncryptByte(endByte, fileId);
            var lenByte = BitConverter.GetBytes(encryptByte.Length);
            var newAllByte = new byte[encryptByte.Length +  otherLen + lenByte.Length];
            beginByte.CopyTo(newAllByte, 0);
            encryptByte.CopyTo(newAllByte, beginByte.Length);
            lenByte.CopyTo(newAllByte, beginByte.Length + encryptByte.Length);
            return newAllByte;
        }
        /// <summary>
        /// 将最后一组字节解密后返回全部
        /// </summary>
        /// <param name="inputByteArray"></param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static byte[] NewDecryptByte(byte[] inputByteArray, string fileId)
        {
            try
            {
                var lenByte = new byte[4];
                Buffer.BlockCopy(inputByteArray, inputByteArray.Length - 4, lenByte, 0, 4); // 最后四个字节，获取加密长度
                var encryptLen = BitConverter.ToInt32(lenByte);
                var encryptByte = new byte[encryptLen];
                Buffer.BlockCopy(inputByteArray, inputByteArray.Length - encryptLen -  4, encryptByte, 0, encryptByte.Length);
                var decryptByte = DecryptByte(encryptByte, fileId);
                
                var beginByte = new byte[inputByteArray.Length - encryptLen -  4];
                Buffer.BlockCopy(inputByteArray, 0, beginByte, 0, beginByte.Length);
                
                var newAllByte = new byte[decryptByte.Length + inputByteArray.Length - encryptLen -  4];
                beginByte.CopyTo(newAllByte,0);
                decryptByte.CopyTo(newAllByte, beginByte.Length);
                return newAllByte;
            }
            catch (Exception)
            {
                return inputByteArray;
            }
        }
        /// <summary>
        /// 将最后一组字节解密后返回全部
        /// </summary>
        /// <param name="filePath">文件地址</param>
        /// <param name="fileId"></param>
        /// <returns></returns>
        public static void NewDecryptByte(string filePath, string fileId)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open)) {
                using (BinaryReader br = new BinaryReader(fs)) {
                    try
                    {
                        var len = fs.Length;
                        fs.Seek(len - 4, SeekOrigin.Begin);  //移动位置，读取信息长度
                        var encryptLen = br.ReadInt32();
                        fs.Seek(len - encryptLen - 4, SeekOrigin.Begin);
                        var encryptByte = br.ReadBytes(encryptLen); //读取信息
                        var decryptByte = DecryptByte(encryptByte, fileId);
                        fs.SetLength(len - encryptLen - 4);  //重新设置文件大小，移除之前添加的额外数据
                        fs.Seek(fs.Length, SeekOrigin.Begin); 
                        fs.Write(decryptByte);
                    }
                    catch (Exception)
                    {
                        
                    }
                }
            }
        }
        #endregion
    }
}
