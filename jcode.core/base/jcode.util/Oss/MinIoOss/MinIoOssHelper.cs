using Minio;
using Minio.DataModel;
using Minio.DataModel.Args;
using Minio.DataModel.Encryption;
using Minio.DataModel.Result;
using System;
using System.IO;
using System.Security.Cryptography;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.10.31
    /// 描 述：MinIoOSS帮助类
    /// </summary>
    public class MinIoOssHelper
    {
        private readonly string _bucketName, _accesskey, _secretkey, _endpoint,_region;
        //Minio 7 的链式配置方法（WithEndpoint/WithCredentials 等）都是 IMinioClient 的扩展方法，只能以接口类型持有
        private readonly IMinioClient _minioClient;

        /// <summary>
        /// 初始化
        /// </summary>
        public MinIoOssHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            _endpoint = setting.MinIoOssEndpoint;
            _bucketName = setting.OssBucketName;
            _accesskey = setting.MinIoOssAccessKey;
            _secretkey = setting.MinIoOssSecretKey;
            _region = setting.MinIoOssRegion;

            _minioClient = new MinioClient()
                .WithEndpoint(_endpoint)
                .WithCredentials(_accesskey, _secretkey);

            if (!string.IsNullOrEmpty(_region))
            {
                _minioClient = _minioClient.WithRegion(_region);
            }

            if (setting.MinIoIsSSL)
            {
                _minioClient = _minioClient.WithSSL();
            }

            _minioClient = _minioClient.Build();

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
                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(BucketName);
                // 查询存储桶如果不存在，如果存储桶存在则返回true，否则返回false
                bool found =  _minioClient.BucketExistsAsync(bucketExistsArgs).GetAwaiter().GetResult();
                if (!found)//不存在则创建存储桶
                {
                    MakeBucketArgs makeBucketArgs = new MakeBucketArgs();
                    makeBucketArgs.WithBucket(BucketName);
                    _minioClient.MakeBucketAsync(makeBucketArgs).GetAwaiter().GetResult();
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
                BucketExistsArgs bucketExistsArgs = new BucketExistsArgs();
                bucketExistsArgs.WithBucket(BucketName);
                // 查询存储桶如果不存在，如果存储桶存在则返回true，否则返回false
                bool found = _minioClient.BucketExistsAsync(bucketExistsArgs).GetAwaiter().GetResult();
                if (found)//若存在则删除存储桶
                {
                    RemoveBucketArgs removeBucketArgs = new RemoveBucketArgs();
                    removeBucketArgs.WithBucket(BucketName);
                    _minioClient.RemoveBucketAsync(removeBucketArgs).GetAwaiter().GetResult();
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
        public ListAllMyBucketsResult GetBuckets()
        {
            try
            {
                var list = _minioClient.ListBucketsAsync().GetAwaiter().GetResult();
                if (list.Buckets.Count>0)
                {
                    return list;
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
        /// 删除文件-通过对象名称
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <returns></returns>
        public bool DeleteObject(string objectname)
        {
            try
            {
                RemoveObjectArgs rmArgs = new RemoveObjectArgs()
                                        .WithBucket(_bucketName)
                                        .WithObject(objectname);
                _minioClient.RemoveObjectAsync(rmArgs).GetAwaiter().GetResult();
                return true;
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

        #region 上传指定的文件到对应的MinIoOss存储桶--【分段上传】
        /// <summary>
        /// 上传指定的文件到对应的MinIoOss存储桶--【分段上传】
        /// </summary>
        /// <param name="objectname ">对象名称</param>
        /// <param name="filePath">上传的本地文件路径，需要指定到具体的文件名</param>
        /// 单个对象的最大大小限制在5TB。putObject在对象大于5MiB时，自动使用multiple parts方式上传。
        /// 这样，当上传失败时，客户端只需要上传未成功的部分即可（类似断点上传）。上传的对象使用MD5SUM签名进行完整性验证。
        /// <returns></returns>
        public bool MultipartUpload(string objectname, string filePath)
        {
            try
            {
                using (FileStream filestream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    IServerSideEncryption sse = null;
                    PutObjectArgs putObjectArgs = new PutObjectArgs()
                                                .WithBucket(_bucketName)
                                                .WithObject(objectname)
                                                .WithStreamData(filestream)
                                                .WithObjectSize(filestream.Length)
                                                .WithContentType("application/octet-stream")
                                                .WithServerSideEncryption(sse);
                    _minioClient.PutObjectAsync(putObjectArgs).ConfigureAwait(false).GetAwaiter().GetResult();
                    filestream.Close();
                    filestream.Dispose();
                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 上传指定的文件到对应的MinIoOss存储桶--【上传二进制数据】
        /// <summary>
        /// 上传指定的文件到对应的MinIoOss存储桶--【上传二进制数据】
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <param name="bytes">二进制数据</param>
        /// <returns></returns>
        public bool UploadObject(string objectname, byte[] bytes)
        {
            try
            {
                var filestream = new MemoryStream(bytes);
                IServerSideEncryption sse = null;
                PutObjectArgs putObjectArgs = new PutObjectArgs()
                                            .WithBucket(_bucketName)
                                            .WithObject(objectname)
                                            .WithStreamData(filestream)
                                            .WithObjectSize(filestream.Length)
                                            .WithContentType("application/octet-stream")
                                            .WithServerSideEncryption(sse);
                _minioClient.PutObjectAsync(putObjectArgs).GetAwaiter().GetResult();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }
        #endregion

        #region 上传指定的文件到对应的MinIoOss存储桶--【文件流上传】
        /// <summary>
        /// 上传指定的文件到对应的MinIoOss存储桶--【文件流上传】
        /// </summary>
        /// <param name="objectname ">对象名称</param>
        /// <param name="content">指定上传文件的文件流</param>
        /// <returns></returns>
        public bool UploadStreamObject(string objectname, Stream content)
        {
            try
            {
                Aes aesEncryption = Aes.Create();
                aesEncryption.KeySize = 256;
                aesEncryption.GenerateKey();
                var ssec = new SSEC(aesEncryption.Key);
                PutObjectArgs putObjectArgs = new PutObjectArgs()
                                            .WithBucket(_bucketName)
                                            .WithObject(objectname)
                                            .WithStreamData(content)
                                            .WithContentType("application/octet-stream")
                                            .WithServerSideEncryption(ssec);
                _minioClient.PutObjectAsync(putObjectArgs).GetAwaiter().GetResult();
                return true;
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
        /// 从指定的OSS存储空间中获取指定的文件
        /// </summary>
        /// <param name="objectname">对象名称</param>
        /// <param name="hostDownloadPath">保存的本地文件夹路径</param>
        /// <returns></returns>
        public bool WriteObject(string objectname, string hostDownloadPath)
        {
            try
            {
                StatObjectArgs statObjectArgs = new StatObjectArgs()
                                                .WithBucket(_bucketName)
                                                .WithObject(objectname);
                _minioClient.StatObjectAsync(statObjectArgs).GetAwaiter().GetResult();
                GetObjectArgs getObjectArgs = new GetObjectArgs()
                                            .WithBucket(_bucketName)
                                            .WithObject(objectname)
                                            .WithCallbackStream((stream) =>
                                            {
                                                MemoryStream ms = new MemoryStream();
                                                stream.CopyTo(ms);
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
                                            });
                //获取文件数据
                ObjectStat objectStat= _minioClient.GetObjectAsync(getObjectArgs).GetAwaiter().GetResult();
                return true;
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
            MemoryStream ms = new MemoryStream();
            if (string.IsNullOrEmpty(bucketName))
            {
                bucketName = _bucketName;
            }
            //检查对象是否存在，不存在则抛出异常
            StatObjectArgs statObjectArgs = new StatObjectArgs()
                                        .WithBucket(bucketName)
                                        .WithObject(objectname);
            _minioClient.StatObjectAsync(statObjectArgs).GetAwaiter().GetResult();
            //对象存在，则或者文件源
            GetObjectArgs getObjectArgs = new GetObjectArgs()
                                        .WithBucket(bucketName)
                                        .WithObject(objectname)
                                        .WithCallbackStream((stream) =>
                                        {
                                            stream.CopyTo(ms);
                                        });
            ObjectStat objectStat = _minioClient.GetObjectAsync(getObjectArgs).GetAwaiter().GetResult();
            if (ms.Length > 0)
            {
                byte[] bytes = new byte[ms.Length];
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
