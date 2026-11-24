using OBS;
using OBS.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.10.24
    /// 描 述：华为云OSS帮助类
    /// </summary>
    public class HuaWeiOssHelper
    {
        private readonly string _bucketName, _accesskey, _secretaccesskey, _endpoint;
        private readonly ObsClient _obsClient;
        private readonly ObsConfig config;
        /// <summary>
        /// 初始化
        /// </summary>
        public HuaWeiOssHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _bucketName = setting.OssBucketName;
            _accesskey = setting.HuaWeiOssAK;
            _secretaccesskey = setting.HuaWeiOssSK;
            _endpoint = setting.HuaWeiOssEndpoint;
            //初始化配置参数
            config = new ObsConfig();
            config.Endpoint = _endpoint;
            // 创建ObsClient实例
            _obsClient = new ObsClient(_accesskey, _secretaccesskey, config);
        }

        #region 存储捅

        #region 创建存储桶
        /// <summary>
        /// 创建存储桶
        /// </summary>
        /// <param name="BucketName">存储桶名称</param>
        /// <returns></returns>
        public bool PutBucket(string BucketName)
        {
            try
            {
                CreateBucketRequest request = new CreateBucketRequest
                {
                    BucketName = "bucketname",
                };
                CreateBucketResponse response = _obsClient.CreateBucket(request);
                if (response.StatusCode== HttpStatusCode.OK)
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
        /// <param name="BucketName">存储桶名称</param>
        /// <returns></returns>
        public bool DeleteBucket(string BucketName)
        {
            try
            {
                DeleteBucketRequest request = new DeleteBucketRequest
                {
                    BucketName = "bucketname",
                };
                DeleteBucketResponse response = _obsClient.DeleteBucket(request);
                if (response.StatusCode == HttpStatusCode.OK)
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
        public IList<ObsBucket> GetBuckets()
        {
            try
            {
                ListBucketsRequest request = new ListBucketsRequest();
                ListBucketsResponse response = _obsClient.ListBuckets(request);
                request.IsQueryLocation = true;
                if (response.StatusCode==HttpStatusCode.OK)
                {
                    return response.Buckets;
                }
                return null;
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
        /// <param name="objectname">对象名称</param>
        /// <returns></returns>
        public bool DeleteObject(string objectname)
        {
            try
            {
                DeleteObjectRequest request = new DeleteObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname,
                };
                DeleteObjectResponse response = _obsClient.DeleteObject(request);
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
                // 1. 初始化分段上传任务
                InitiateMultipartUploadRequest initiateRequest = new InitiateMultipartUploadRequest
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname
                };
                InitiateMultipartUploadResponse initResponse = _obsClient.InitiateMultipartUpload(initiateRequest);
                // 2. 上传段
                long contentLength = new FileInfo(filePath).Length;
                long partSize = 15 * (long)Math.Pow(2, 20); // 每个段大小15 MB
                List<UploadPartResponse> uploadResponses = new List<UploadPartResponse>();
                long filePosition = 0;
                for (int i = 1; filePosition < contentLength; i++)
                {
                    UploadPartRequest uploadRequest = new UploadPartRequest
                    {
                        BucketName = _bucketName,
                        ObjectKey = objectname,
                        UploadId = initResponse.UploadId,
                        PartNumber = i,
                        PartSize = partSize,
                        Offset = filePosition,
                        FilePath = filePath
                    };
                    uploadResponses.Add(_obsClient.UploadPart(uploadRequest));
                    filePosition += partSize;
                }
                // 3.合并段

                CompleteMultipartUploadRequest completeRequest = new CompleteMultipartUploadRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname,
                    UploadId = initResponse.UploadId,
                };
                completeRequest.AddPartETags(uploadResponses);
                CompleteMultipartUploadResponse completeUploadResponse = _obsClient.CompleteMultipartUpload(completeRequest);
                if (completeUploadResponse.StatusCode==HttpStatusCode.OK)
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
                Stream stream = new MemoryStream(bytes);
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname,
                    InputStream = stream,
                };
                PutObjectResponse response = _obsClient.PutObject(request);
                if (response.StatusCode == HttpStatusCode.OK)
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
                PutObjectRequest request = new PutObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname,
                    InputStream = content,
                };
                PutObjectResponse response = _obsClient.PutObject(request);
                if (response.StatusCode == HttpStatusCode.OK)
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
        /// <param name="objectname">对象名称</param>
        /// <param name="hostDownloadPath">保存的本地文件夹路径</param>
        /// <returns></returns>
        public bool WriteObject(string objectname,string hostDownloadPath)
        {
            try
            {
                GetObjectRequest request = new GetObjectRequest()
                {
                    BucketName = _bucketName,
                    ObjectKey = objectname,
                };
                GetObjectResponse response = _obsClient.GetObject(request);
                using (response)
                {
                    if (!File.Exists(hostDownloadPath))
                    {
                        // 将对象的数据流写入文件中
                        response.WriteResponseStreamToFile(hostDownloadPath);
                    }
                }
                if (response.StatusCode==HttpStatusCode.OK)
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
            if (string.IsNullOrEmpty(bucketName))
            {
                bucketName = _bucketName;
            }
            GetObjectRequest request = new GetObjectRequest()
            {
                BucketName = bucketName,
                ObjectKey = objectname,
            };
            GetObjectResponse response = _obsClient.GetObject(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                byte[] bytes = new byte[response.ContentLength];
                MemoryStream ms = new MemoryStream();
                response.OutputStream.CopyTo(ms);
                ms.Flush();
                ms.Position = 0;
                ms.Read(bytes, 0, bytes.Length);
                ms.Close();
                ms.Dispose();
                return bytes;
            }
            return null;
        }
        #endregion
    }

}
