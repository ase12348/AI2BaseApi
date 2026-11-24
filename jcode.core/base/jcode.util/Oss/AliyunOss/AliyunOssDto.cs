using System.IO;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.09.05
    /// 描 述：阿里云OSS对象存储实体类
    /// </summary>
    public class AliyunOssDto
    {
        /// <summary>
        /// 上传文件
        /// </summary>
        public Stream file { get; set; }
        /// <summary>
        /// 文件名
        /// </summary>
        public string filename { get; set; }
    }

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.09.05
    /// 描 述：阿里云OSS对象存储分页参数
    /// </summary>
    public class AliOssPaginationInputDto
    {
        /// <summary>
        /// 每页行数
        /// </summary>
        public int rows { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        public int page { get; set; }
        /// <summary>
        /// 排序列
        /// </summary>
        public string sidx { get; set; }
        /// <summary>
        /// 排序类型
        /// </summary>
        public string sord { get; set; }
        /// <summary>
        /// 如果本次没有返回全部结果，响应请求中将包含NextMarker元素，用于标明接下来请求的Marker值
        /// </summary>
        public string nextMarker { get; set; }
        /// <summary>
        /// 存储空间名称
        /// </summary>
        public string bucketName { get; set; }
    }

    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.09.5
    /// 描 述：阿里云OSS请求STS参数
    /// </summary>
    public class PolicyModel
    {
        /// <summary>
        /// 授权语句
        /// </summary>
        public Statement[] Statement { get; set; }
        /// <summary>
        /// 版本号
        /// </summary>
        public string Version { get; set; }
    }
    /// <summary>
    /// 阿里云OSS请求STS参数-授权语句
    /// </summary>
    public class Statement
    {
        /// <summary>
        /// 操作名称
        /// </summary>
        public string[] Action { get; set; }
        /// <summary>
        /// 允许/禁止
        /// </summary>
        public string Effect { get; set; }
        /// <summary>
        /// 操作对象
        /// </summary>
        public string Resource { get; set; }
    }

}
