using AlibabaCloud.SDK.Sts20150401;
using AlibabaCloud.SDK.Sts20150401.Models;
using Aliyun.OSS;
using Newtonsoft.Json;
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
    /// 日 期：2022.09.05
    /// 描 述：阿里云OSS帮助类
    /// </summary>
    public class AliyunOssHelper
    {
        private readonly string _endpoint, _bucketName, _regionID, _accessId, _accessSecret, _policyResource, _roleArn, _ossDf_endpoint;
        private readonly OssClient _ossClient;
        private readonly int _durationSeconds;
        /// <summary>
        /// 初始化
        /// </summary>
        public AliyunOssHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _accessId = setting.OssAccessKeyId;
            _accessSecret = setting.OssAccessKeySecret;
            _endpoint = setting.OssEndpoint;
            _bucketName = setting.OssBucketName;
            _regionID = setting.OssRegionId;
            _durationSeconds = setting.OssDurationSeconds ?? 3600;
            _policyResource = setting.OssPolicyResource;
            _roleArn = setting.OssRoleArn;
            _ossDf_endpoint = setting.OssDf_endpoint;
            _ossClient = new OssClient(_ossDf_endpoint, _accessId, _accessSecret);
        }
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="accessId"></param>
        /// <param name="accessSecret"></param>
        /// <param name="securityToken"></param>
        public AliyunOssHelper(string accessId, string accessSecret, string securityToken)
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _ossDf_endpoint = setting.OssDf_endpoint;
            _bucketName = setting.OssBucketName;
            _ossClient = new OssClient(_ossDf_endpoint, accessId, accessSecret, securityToken);
        }

        #region 文件夹管理

        #region 创建一个文件夹
        /// <summary>
        /// 创建一个文件夹 
        /// </summary>
        /// <param name="folderKey">yourfolder/以/结尾</param>
        public void CreateFolder(string folderKey)
        {
            // 初始化OssClient
            // Note: key treats as a folder and must end with a slash.
            //const string yourfolder = "yourfolder/";
            // put object with zero bytes stream.
            using (MemoryStream ms = new MemoryStream())
            {
                _ossClient.PutObject(_bucketName, folderKey, ms);
            }
        }
        #endregion

        #region 删除文件夹

        #endregion

        #region 更新文件夹

        #endregion

        #endregion

        #region 文件管理

        #region 列举文件
        /// <summary>
        /// 列举文件
        /// </summary>
        /// <param name="ossPaginationInputDto"></param>
        /// <returns></returns>
        public ObjectListing GetObjectList(AliOssPaginationInputDto ossPaginationInputDto)
        {
            try
            {
                var objects = new List<string>();
                ObjectListing result = null;
                string nextMarker = string.Empty;

                if (string.IsNullOrEmpty(ossPaginationInputDto.bucketName))
                {
                    ossPaginationInputDto.bucketName = _bucketName;
                }
                if (!string.IsNullOrEmpty(ossPaginationInputDto.nextMarker))
                {
                    nextMarker = ossPaginationInputDto.nextMarker;
                }
                do
                {
                    var listObjectsRequest = new ListObjectsRequest(ossPaginationInputDto.bucketName)
                    {
                        Marker = nextMarker,//下一次列举文件的起点
                        MaxKeys = ossPaginationInputDto.rows//每页列举的文件个数
                    };
                    // 列举文件。
                    result = _ossClient.ListObjects(listObjectsRequest);
                    if (result.HttpStatusCode == HttpStatusCode.OK)
                    {
                        return result;
                    }
                } while (result.IsTruncated);//指明列举文件是否被截断

                return null;
            }
            catch (Exception)
            {
                return null;
                throw;
            }
        }
        #endregion

        #region 删除对象
        /// <summary>
        /// 删除对象
        /// </summary>
        /// <param name="objKey">对象key 路径/名称</param>
        public bool DeleteObject(string objKey)
        {
            var result=_ossClient.DeleteObject(_bucketName, objKey);
            if (result.HttpStatusCode == HttpStatusCode.OK)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region 批量删除对象
        /// <summary>
        /// 批量删除对象
        /// </summary>
        /// <param name="objKeylist"></param>
        public void DeleteObjects(IList<string> objKeylist)
        {
            if (objKeylist?.Count > 0)
            {
                var deleteObjectsRequest = new DeleteObjectsRequest(_bucketName, objKeylist);
                var result = _ossClient.DeleteObjects(deleteObjectsRequest);
                // return result;
            }
        }
        #endregion

        #region 复制对象
        /// <summary>
        /// 复制对象
        /// </summary>
        /// <param name="sourceKey">源Key路径</param>
        /// <param name="destinationKey">新Key路径</param>
        public void CopyObject(string sourceKey, string destinationKey)
        {
            var copyobjectrequest = new CopyObjectRequest(_bucketName, sourceKey, _bucketName, destinationKey);
            var copyObjectResult = _ossClient.CopyObject(copyobjectrequest);
        }
        #endregion

        #endregion

        #region 上传文件

        #region 上传指定的文件到指定的OSS的存储空间【分片上传】
        /// <summary>
        /// 上传指定的文件到指定的OSS的存储空间
        /// </summary>
        /// <param name="key">文件的在OSS上保存的名称</param>
        /// <param name="hostFilePath">填写本地文件的完整路径。如果未指定本地路径，则默认从示例程序所属项目对应本地路径中上传文件。</param>
        public bool MultipartUpload(string key, string hostFilePath)
        {
            // 初始化分片上传，返回uploadId。
            var uploadId = "";
            try
            {
                // 定义上传的文件及所属Bucket的名称。您可以在InitiateMultipartUploadRequest中设置ObjectMeta，但不必指定其中的ContentLength。
                var request = new InitiateMultipartUploadRequest(_bucketName, key);
                var result = _ossClient.InitiateMultipartUpload(request);
                uploadId = result.UploadId;
            }
            catch (Exception)
            {
                throw;
            }
            // 计算分片总数。
            var partSize = 100 * 1024;
            var fi = new FileInfo(hostFilePath);
            var fileSize = fi.Length;
            var partCount = fileSize / partSize;
            if (fileSize % partSize != 0)
            {
                partCount++;
            }
            // 开始分片上传。PartETags是保存PartETag的列表，OSS收到用户提交的分片列表后，会逐一验证每个分片数据的有效性。当所有的数据分片通过验证后，OSS会将这些分片组合成一个完整的文件。
            var partETags = new List<PartETag>();
            try
            {
                using (var fs = File.Open(hostFilePath, FileMode.Open))
                {
                    for (var i = 0; i < partCount; i++)
                    {
                        var skipBytes = (long)partSize * i;
                        // 定位到本次上传的起始位置。
                        fs.Seek(skipBytes, 0);
                        // 计算本次上传的分片大小，最后一片为剩余的数据大小。
                        var size = partSize < fileSize - skipBytes ? partSize : fileSize - skipBytes;
                        var request = new UploadPartRequest(_bucketName, key, uploadId)
                        {
                            InputStream = fs,
                            PartSize = size,
                            PartNumber = i + 1
                        };
                        // 调用UploadPart接口执行上传功能，返回结果中包含了这个数据片的ETag值。
                        var result = _ossClient.UploadPart(request);
                        partETags.Add(result.PartETag);
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            // 完成分片上传。
            try
            {
                var completeMultipartUploadRequest = new CompleteMultipartUploadRequest(_bucketName, key, uploadId);
                foreach (var partETag in partETags)
                {
                    completeMultipartUploadRequest.PartETags.Add(partETag);
                }
                var result = _ossClient.CompleteMultipartUpload(completeMultipartUploadRequest);
                if (result.HttpStatusCode == HttpStatusCode.OK)
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

        #region 上传指定的文件到指定的OSS的存储空间【简单上传】
        /// <summary>
        /// 上传指定的文件到指定的OSS的存储空间
        /// </summary>
        /// <param name="key">文件的在OSS上保存的名称</param>
        /// <param name="hostFilePath">指定上传文件的本地路径</param>
        public bool PutObject(string key, string hostFilePath)
        {
            try
            {
                var result = _ossClient.PutObject(_bucketName, key, hostFilePath);
                if (result.HttpStatusCode == HttpStatusCode.OK)
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

        #region 上传指定的文件到指定的OSS的存储空间【文件流】
        /// <summary>
        /// 上传指定的文件到指定的OSS的存储空间
        /// </summary>
        /// <param name="key">文件的在OSS上保存的名称</param>
        /// <param name="content">指定上传文件的文件流</param>
        public bool PutObject(string key, Stream content)
        {
            try
            {
                var result = _ossClient.PutObject(_bucketName, key, content);
                if (result.HttpStatusCode == HttpStatusCode.OK)
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

        #region 从指定的OSS存储空间中获取指定的文件
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <param name="hostDownloadPath">本地存储下载文件的目录</param>
        /// <param name="bucketName">存储空间名称</param>
        public bool WriteObject(string key, string bucketName, string hostDownloadPath)
        {
            try
            {
                if (string.IsNullOrEmpty(bucketName))
                {
                    bucketName = _bucketName;
                }

                var result = _ossClient.GetObject(bucketName, key);
                MemoryStream ms = new MemoryStream();
                result.Content.CopyTo(ms);
                ms.Flush();
                ms.Position = 0;
                byte[] buf = new byte[1024];
                using (var fs = File.Open(hostDownloadPath, FileMode.OpenOrCreate))
                {
                    var len = 0;
                    while ((len = ms.Read(buf, 0, 1024)) != 0)
                    {
                        fs.Write(buf, 0, len);
                    }
                }

                ms.Close();
                ms.Dispose();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <param name="hostDownloadPath">本地存储下载文件的目录</param>
        public bool WriteObject(string key, string hostDownloadPath)
        {
            return WriteObject(key, _bucketName, hostDownloadPath);
        }


        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <param name="bucketName">存储空间名称</param>
        public byte[] GetObject(string key,string bucketName)
        {
            try
            {
                if (string.IsNullOrEmpty(bucketName))
                {
                    bucketName = _bucketName;
                }

                var result = _ossClient.GetObject(bucketName, key);

                //将从OSS读取到的文件转为二进制流
                if (result.HttpStatusCode == HttpStatusCode.OK)
                {
                    byte[] bytes = new byte[result.Content.Length];
                    MemoryStream ms = new MemoryStream();
                    result.Content.CopyTo(ms);
                    ms.Flush();
                    ms.Position = 0;
                    ms.Read(bytes, 0, bytes.Length);
                    ms.Close();
                    ms.Dispose();
                    return bytes;
                }
                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

       


       
        /// <summary>
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        public byte[] GetObject(string key)
        {
            
            return GetObject(key,_bucketName);
        }




        #endregion

        #endregion

        #region 存储空间

        #region 列举存储空间
        /// <summary>
        /// 获取存储空间列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Bucket> GetListBuckets()
        {
            try
            {
                var buckets = _ossClient.ListBuckets();

                return buckets;
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #endregion

        #region 通用方法

        #region 获取OSS sts
        /// <summary>
        /// 获取OSS sts 
        /// </summary>
        /// <param name="roleSessionName"></param>
        /// <param name="policyAction">是删除 还是上传的 动作</param>
        /// <param name="policyResourcePath">文件路径</param>
        public (AssumeRoleResponse rs, string bucketName, string regionID) GetSTS(string roleSessionName, string[] policyAction, string policyResourcePath)
        {
            var policyStruct = new PolicyModel
            {
                Statement = new Statement[] {
                    new Statement{
                        Action=policyAction,
                        Effect="Allow",
                        //Resource=$"acs:oss:*:*:fat-sucaidaku/{xxx}/{DateTime.Now.Year}/*"
                        Resource=$"{_policyResource}/{policyResourcePath}/*"
                    }
                },
                Version = "1"
            };
            string policy = JsonConvert.SerializeObject(policyStruct);
            Client client = CreateClient(_accessId, _accessSecret, _endpoint);
            AssumeRoleRequest assumeRoleRequest = new AssumeRoleRequest
            {
                DurationSeconds = _durationSeconds,
                Policy = policy,
                //RoleArn = "acs:ram::1857116294116236:role/fat-sucaidaku-ossram",
                RoleArn = _roleArn,
                RoleSessionName = roleSessionName,  //自定义角色会话名称，用来区分不同的令牌
            };
            // 复制代码运行请自行打印 API 的返回值

            return (client.AssumeRole(assumeRoleRequest), _bucketName, _regionID);
        }
        #endregion

        #region 创建获取STS临时凭证的对象
        /// <summary>
        /// 创建获取STS临时凭证的对象
        /// </summary>
        /// <param name="accessKeyId"></param>
        /// <param name="accessKeySecret"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public Client CreateClient(string accessKeyId, string accessKeySecret, string endpoint)
        {
            AlibabaCloud.OpenApiClient.Models.Config config = new AlibabaCloud.OpenApiClient.Models.Config
            {
                // 您的AccessKey ID
                AccessKeyId = accessKeyId,
                // 您的AccessKey Secret
                AccessKeySecret = accessKeySecret,
            };
            // 访问的域名
            config.Endpoint = endpoint; //"sts.cn-shanghai.aliyuncs.com";
            return new Client(config);
        }
        #endregion、

        #region 获取文件的签名URL
        /// <summary>
        /// 获取文件的签名URL
        /// </summary>
        /// <param name="fileKey">文件路径</param>
        /// <param name="expiration">DateTime.Now.AddHours(1)</param>
        /// <param name="isPreviewImg">是否是预览图片</param>
        /// <param name="imageResize">图片略缩图参数</param>
        /// <param name="isAttachment">下载的请求头标识</param>
        /// <returns></returns>
        public Uri GetGeneratePresignedUri(string fileKey, DateTime expiration, bool isPreviewImg = true, string imageResize = "image/resize,m_fill,h_240,w_320", bool isAttachment = true)
        {
            try
            {
                string imgProcess = null;
                if (isPreviewImg)
                    imgProcess = imageResize; //"image/resize,p_20";

                var req = new GeneratePresignedUriRequest(_bucketName, fileKey, SignHttpMethod.Get)
                {
                    Expiration = expiration,
                    Process = imgProcess,
                };
                if (isAttachment)
                {
                    req.ResponseHeaders.ContentDisposition = "attachment; filename =" + fileKey;
                }
                else
                {
                    req.ResponseHeaders.ContentDisposition = "inline";
                }
                //req.ContentType = "image/jpg";
                return _ossClient.GeneratePresignedUri(req);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region 是否存在
        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        /// <returns></returns>
        public bool IsExistObject(string key)
        {
            try
            {
                return _ossClient.DoesObjectExist(_bucketName, key);
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region 获取文件大小 KB
        /// <summary>
        /// 获取文件大小 KB
        /// </summary>
        /// <param name="key">要获取的文件在OSS上的名称</param>
        public int GetObjectFileSize(string key)
        {
            try
            {

                var size = 0;
                var result = _ossClient.GetObject(_bucketName, key);
                if (result != null)
                {
                    using (var requestStream = result.Content)
                    {
                        byte[] buf = new byte[1024];
                        using (var fs = new MemoryStream())
                        {
                            var len = 0;
                            while ((len = requestStream.Read(buf, 0, 1024)) != 0)
                            {
                                fs.Write(buf, 0, len);
                            }
                            size = (int)fs.Length;
                        }
                    }
                }
                return size / 1024;

            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion

        #endregion
    }

}
