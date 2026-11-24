using COSXML;
using COSXML.Auth;
using COSXML.Model.Bucket;
using COSXML.Model.Object;
using COSXML.Model.Service;
using COSXML.Model.Tag;
using COSXML.Transfer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.10.14
    /// 描 述：腾讯云OSS帮助类
    /// </summary>
    public class TencentOssHelper
    {
        private readonly string _secretId, _secretKey, _bucketName, _appid, _regionID;
        private readonly int _durationSeconds;
        private readonly CosXmlConfig config;
        private readonly QCloudCredentialProvider cosCredentialProvider;

        /// <summary>
        /// 初始化
        /// </summary>
        public TencentOssHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _secretId = setting.TencentSecretId;
            _secretKey = setting.TencentSecretKey;
            _bucketName = setting.OssBucketName;
            _durationSeconds = setting.TencentOssDurationSeconds;
            _appid = setting.TencentAppid;
            _regionID = setting.TencentOssRegionId;
            config = new CosXmlConfig.Builder()
              .IsHttps(true)  //设置默认 HTTPS 请求
              .SetRegion(_regionID)  //设置一个默认的存储桶地域
              .SetDebugLog(true)  //显示日志
              .Build();  //创建 CosXmlConfig 对象
            cosCredentialProvider = new DefaultQCloudCredentialProvider(
              _secretId, _secretKey, _durationSeconds);//创建访问凭证
        }

        #region 存储捅

        #region 创建存储桶
        /// <summary>
        /// 创建存储桶
        /// </summary>
        /// <param name="BucketName">存储桶名称，此处填入格式必须为 BucketName-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer</param>
        /// <returns></returns>
        public bool PutBucket(string BucketName)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                PutBucketRequest request = new PutBucketRequest(BucketName);
                //执行请求
                PutBucketResult result = cosXml.PutBucket(request);
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        #endregion

        #region 删除存储桶
        /// <summary>
        /// 删除存储桶
        /// </summary>
        /// <param name="BucketName">存储桶名称，此处填入格式必须为 BucketName-APPID, 其中 APPID 获取参考 https://console.cloud.tencent.com/developer</param>
        /// <returns></returns>
        public bool DeleteBucket(string BucketName)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                DeleteBucketRequest request = new DeleteBucketRequest(BucketName);
                //执行请求
                DeleteBucketResult result = cosXml.DeleteBucket(request);
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }

        #endregion

        #region 查询存储桶列表
        /// <summary>
        /// 查询存储桶列表
        /// </summary>
        /// <returns></returns>
        public List<ListAllMyBuckets.Bucket> GetBuckets()
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                GetServiceRequest request = new GetServiceRequest();
                //执行请求
                GetServiceResult result = cosXml.GetService(request);
                //得到所有的 buckets
                List<ListAllMyBuckets.Bucket> allBuckets = result.listAllMyBuckets.buckets;
                return allBuckets;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        #endregion

        #endregion

        #region 文件管理

        #region 删除文件
        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="key">对象键</param>
        /// <returns></returns>
        public bool DeleteObject(string key)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                DeleteObjectRequest request = new DeleteObjectRequest(_bucketName, key);
                //执行请求
                DeleteObjectResult result = cosXml.DeleteObject(request);
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        #endregion

        #endregion

        #region 上传文件

        #region 上传指定的文件到对应的腾讯云Oss存储桶--【分片上传】
        /// <summary>
        /// 上传指定的文件到对应的腾讯云Oss存储桶--【分片上传】
        /// </summary>
        /// <param name="key ">
        /// 对象建
        /// 是对象在存储桶中的唯一标识，可以通俗的理解为文件路径。例如对象键为 doc/picture.jpg 则表示为图片文件 picture.jpg 存放在对象存储（Cloud Object Storage，COS）的 doc 路径（或文件夹）下。</param>
        /// <param name="srcPath">本地文件绝对路径</param>
        /// <returns></returns>
        public async Task<bool> MultipartUpload(string key, string srcPath)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                // 初始化 TransferConfig
                TransferConfig transferConfig = new TransferConfig();
                // 手动设置开始分块上传的大小阈值为10MB，默认值为5MB
                transferConfig.DivisionForUpload = 10 * 1024 * 1024;
                // 手动设置分块上传中每个分块的大小为2MB，默认值为1MB
                transferConfig.SliceSizeForUpload = 2 * 1024 * 1024;

                // 初始化 TransferManager
                TransferManager transferManager = new TransferManager(cosXml, transferConfig);

                // 上传对象
                COSXMLUploadTask uploadTask = new COSXMLUploadTask(_bucketName, key);
                uploadTask.SetSrcPath(srcPath);
                //上传进度回调
                //uploadTask.progressCallback = delegate (long completed, long total)
                //{
                //    Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                //};
                COSXMLUploadTask.UploadTaskResult result = await transferManager.UploadAsync(uploadTask);

                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }
        #endregion

        #region 上传指定的文件到对应的腾讯云Oss存储桶--【上传二进制数据】
        /// <summary>
        /// 上传指定的文件到对应的腾讯云Oss存储桶--【上传二进制数据】
        /// </summary>
        /// <param name="cosPath">是对象在存储桶中的唯一标识，可以通俗的理解为文件路径。例如对象键为 doc/picture.jpg 则表示为图片文件 picture.jpg 存放在对象存储（Cloud Object Storage，COS）的 doc 路径（或文件夹）下。</param>
        /// <param name="bytes">附件的二进制数据</param>
        /// <returns></returns>
        public bool UploadObject(string cosPath, byte[] bytes)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                PutObjectRequest putObjectRequest = new PutObjectRequest(_bucketName, cosPath, bytes);
                // 发起上传
                PutObjectResult result = cosXml.PutObject(putObjectRequest);
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }
        #endregion

        #region 上传指定的文件到对应的腾讯云Oss存储桶--【文件流上传】
        /// <summary>
        /// 上传指定的文件到对应的腾讯云Oss存储桶--【文件流上传】
        /// </summary>
        /// <param name="key ">
        /// 对象建
        /// 是对象在存储桶中的唯一标识，可以通俗的理解为文件路径。例如对象键为 doc/picture.jpg 则表示为图片文件 picture.jpg 存放在对象存储（Cloud Object Storage，COS）的 doc 路径（或文件夹）下。</param>
        /// <param name="srcPath">本地文件绝对路径</param>
        /// <returns></returns>
        public bool UploadStreamObject(string key, string srcPath)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                // 打开只读的文件流对象
                FileStream fileStream = new FileStream(srcPath, FileMode.Open, FileAccess.Read);
                // 组装上传请求，其中 offset sendLength 为可选参数
                long offset = 0L;
                long sendLength = fileStream.Length;
                PutObjectRequest request = new PutObjectRequest(_bucketName, key, fileStream, offset, sendLength);
                //设置进度回调
                //request.SetCosProgressCallback(delegate (long completed, long total)
                //{
                //    Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                //});
                //执行请求
                PutObjectResult result = cosXml.PutObject(request);
                //关闭文件流
                fileStream.Close();
                //对象的 eTag
                string eTag = result.eTag;
                //对象的 crc64ecma 校验值
                string crc64ecma = result.crc64ecma;
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }
        #endregion

        #endregion

        #region 下载文件
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">对象在存储桶中的位置标识符，即称对象键</param>
        /// <param name="localFileName">指定本地保存的文件名</param>
        /// <param name="hostDownloadPath">保存的本地文件夹路径</param>
        /// <returns></returns>
        public bool WriteObject(string key, string localFileName, string hostDownloadPath)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);

                GetObjectRequest request = new GetObjectRequest(_bucketName, key, hostDownloadPath, localFileName);
                ////设置进度回调
                //request.SetCosProgressCallback(delegate (long completed, long total)
                //{
                //    Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                //});

                //判断是否存在附件
                if (ObjectIsExist(key))
                {
                    //执行请求
                    GetObjectResult result = cosXml.GetObject(request);
                    if (result.IsSuccessful())
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件--异步下载
        /// </summary>
        /// <param name="key">对象在存储桶中的位置标识符，即称对象键</param>
        /// <param name="localFileName">指定本地保存的文件名</param>
        /// <param name="hostDownloadPath">保存的本地文件夹路径</param>
        /// <returns></returns>
        public async Task<bool> WriteObjectAsync(string key,string localFileName, string hostDownloadPath)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                // 初始化 TransferConfig
                TransferConfig transferConfig = new TransferConfig();
                // 手动设置高级下载接口的分块阈值为 20MB(默认为20MB), 从5.4.26版本开始支持！
                //transferConfig.DivisionForDownload = 20 * 1024 * 1024;
                // 手动设置高级下载接口的分块大小为 10MB(默认为5MB),不建议此处设置过小的分块值,可能导致频繁重试或下载速度不合预期
                //transferConfig.SliceSizeForDownload = 10 * 1024 * 1024;
                // 初始化 TransferManager
                TransferManager transferManager = new TransferManager(cosXml, transferConfig);
                // 下载对象
                COSXMLDownloadTask downloadTask = new COSXMLDownloadTask(_bucketName, key, hostDownloadPath, localFileName);
                // 手动设置高级下载接口的并发数 (默认为5), 从5.4.26版本开始支持！
                //downloadTask.SetMaxTasks(10);
                //下载进度回调
                //downloadTask.progressCallback = delegate (long completed, long total)
                //{
                //    Console.WriteLine(String.Format("progress = {0:##.##}%", completed * 100.0 / total));
                //};
                COSXMLDownloadTask.DownloadTaskResult result = await transferManager.DownloadAsync(downloadTask);
                if (result.IsSuccessful())
                {
                    return true;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
                throw;
            }

        }

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
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <param name="bucketName">存储空间名称</param>
        public byte[] GetObject(string key, string bucketName)
        {
            if (string.IsNullOrEmpty(bucketName))
            {
                bucketName = _bucketName;
            }
            //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
            CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);

            GetObjectBytesRequest request = new GetObjectBytesRequest(_bucketName, key);

            //判断是否存在附件
            if (ObjectIsExist(key))
            {
                //执行请求
                GetObjectBytesResult result = cosXml.GetObject(request);

                if (result.IsSuccessful())
                {
                    return result.content;
                }
            }
            return null;
        }
        #endregion

        #region 通用方法

        #region 检查对象是否存在
        /// <summary>
        /// 检查对象是否存在
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool ObjectIsExist(string key)
        {
            try
            {
                //初始化 CosXmlServer 服务类。服务类建议在程序中作为单例使用。
                CosXml cosXml = new CosXmlServer(config, cosCredentialProvider);
                DoesObjectExistRequest request = new DoesObjectExistRequest(_bucketName, key);
                //执行请求
                bool exist = cosXml.DoesObjectExist(request);
                return exist;
            }
            catch (Exception)
            {
                return false;
                throw;
            }
        }
        #endregion

        #endregion
    }
}
