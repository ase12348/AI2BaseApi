using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Qiniu.Http;
using Qiniu.Util;
using Qiniu.Storage;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2024.03.22
    /// 描 述：七牛OSS帮助类
    /// </summary>
    public class QiniuOssHelper
    {
        private readonly string _bucketName;
        private readonly string _domain;
        private readonly Mac mac; // 上传密钥
        private readonly Config config; // 上传配置
        private readonly PutPolicy putPolicy; // 上传策略
        private readonly ResumableUploader target;
        
        /// <summary>
        /// 初始化
        /// </summary>
        public QiniuOssHelper()
        {
            
            var setting = ConfigHelper.GetConfig(); //获取配置
            _bucketName = setting.OssBucketName;
            _domain = setting.QiniuOssDomain;
            mac = new Mac(setting.QiniuOssAccessKey,  setting.QiniuOssSecretKey);
            
            config = new Config();
            // 设置上传区域 华东ZONE_CN_East/华北ZONE_CN_North/华南ZONE_CN_South/北美ZONE_US_North/东南亚ZONE_AS_Singapore
            switch (setting.QiniuZone)
            {
                case "ZONE_CN_East":
                    config.Zone = Zone.ZONE_CN_East;
                    break;
                case "ZONE_CN_North":
                    config.Zone = Zone.ZONE_CN_North;
                    break;
                case "ZONE_CN_South":
                    config.Zone = Zone.ZONE_CN_South;
                    break;
                case "ZONE_US_North":
                    config.Zone = Zone.ZONE_US_North;
                    break;
                case "ZONE_AS_Singapore":
                    config.Zone = Zone.ZONE_AS_Singapore;
                    break;
            }
            // 设置 http 或者 https 上传
            config.UseHttps = true;
            config.UseCdnDomains = true;
            config.ChunkSize = ChunkUnit.U512K;
            
            // 设置上传策略
            putPolicy = new PutPolicy();
            // 设置要上传的目标空间
            putPolicy.Scope = setting.OssBucketName;
            // 上传策略的过期时间(单位:秒)
            putPolicy.SetExpires(3600);
            
            target = new ResumableUploader(config);
        }
        
         

        #region 文件管理

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <returns></returns>
        public bool DeleteObject(string objectname)
        {
            try
            {
                BucketManager bucketManager = new BucketManager(mac, config);
                HttpResult deleteRet = bucketManager.Delete(_bucketName, objectname);
                Console.WriteLine("delete error: " + deleteRet.ToString());
                if (deleteRet.Code != (int)HttpCode.OK)
                {
                    Console.WriteLine("delete error: " + deleteRet.ToString());
                    return false;
                }
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #endregion

        #region 上传文件

        #region 上传指定的文件到对应的华为云Oss存储桶--【分段上传】
        /// <summary>
        /// 上传指定的文件到对应的华为云Oss存储桶--【分段上传】
        /// </summary>
        /// <param name="objectname ">对象名称</param>
        /// <param name="filePath">上传的本地文件路径，需要指定到具体的文件名</param>
        /// <returns></returns>
        public bool MultipartUpload(string objectname, string filePath)
        {
            try
            {
                string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                
                
                PutExtra extra = new PutExtra();
                //设置断点续传进度记录文件
                extra.ResumeRecordFile = ResumeHelper.GetDefaultRecordKey(filePath, objectname);
                // Console.WriteLine("record file:" + extra.ResumeRecordFile);
                extra.ResumeRecordFile = objectname + ".progress";
                //指定分片上传版本，默认分片上传 v1
                extra.Version = "v2";
                // 分片上传 v2 指定 part 大小，默认大小为4MB 分片大小范围为1 MB - 1 GB
                extra.PartSize = 4 * 1024 * 1024;
                // 指定分片并发大小，默认为1 
                extra.BlockUploadThreads = 4;
                
                HttpResult result = target.UploadFile(filePath, objectname, token, extra);
                
                if (result.Code == 200)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 上传指定的文件到对应的华为云Oss存储桶--【上传二进制数据】
        /// <summary>
        /// 上传指定的文件到对应的华为云Oss存储桶--【上传二进制数据】
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <param name="bytes">二进制数据</param>
        /// <returns></returns>
        public bool UploadObject(string objectname, byte[] bytes)
        {
            try
            {
                string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                Stream stream = new MemoryStream(bytes);
                HttpResult result = target.UploadStream(stream, objectname, token,null);
                if (result.Code == 200)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 上传指定的文件到对应的华为云Oss存储桶--【文件流上传】
        /// <summary>
        /// 上传指定的文件到对应的华为云Oss存储桶--【文件流上传】
        /// </summary>
        /// <param name="objectname ">对象名称</param>
        /// <param name="content">指定上传文件的文件流</param>
        /// <returns></returns>
        public bool UploadStreamObject(string objectname, Stream content)
        {
            try
            {
                string token = Auth.CreateUploadToken(mac, putPolicy.ToJsonString());
                HttpResult result = target.UploadStream(content, objectname, token,null);
                if (result.Code == 200)
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #endregion

        #region 下载文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public async Task<byte[]> GetUrlContent(string url)  
        {  
            using (var client = new HttpClient())  
            using (var result = await client.GetAsync(url))  
                return result.IsSuccessStatusCode ? await result.Content.ReadAsByteArrayAsync() : null;  
        }  
        
        
        
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <param name="hostDownloadPath">保存的本地文件夹路径</param>
        /// <returns></returns>
        public bool WriteObject(string objectname,string hostDownloadPath)
        {
            try
            {
                var result =  GetObject(objectname);  
                if (result != null)  
                {  
                    FileHelper.Write(hostDownloadPath,result);
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }

        }

        #endregion

        #region 下载文件-返回二进制数据
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        public byte[] GetObject(string key)
        {

            return GetObject(key, _bucketName);
        }

        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="objectname">要获取的文件在OSS上的名称</param>
        /// <param name="bucketName">存储空间名称</param>
        public byte[] GetObject(string objectname, string bucketName)
        {
            string key =$"{bucketName}/{objectname}";
            string privateUrl = DownloadManager.CreatePrivateUrl(mac, _domain, key, 3600);
            var result =  GetUrlContent(privateUrl).GetAwaiter().GetResult();  
            return result;
        }
        #endregion
    }

}
