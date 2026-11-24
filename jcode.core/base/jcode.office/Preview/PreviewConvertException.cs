using System;

namespace jcode.office
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.10.26
    /// 描 述：文件预览转换异常，Code 用于定位失败原因
    /// </summary>
    public class PreviewConvertException : Exception
    {
        /// <summary>
        /// 失败原因码，见 <see cref="PreviewErrorCode"/>
        /// </summary>
        public string Code { get; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="code">失败原因码</param>
        /// <param name="message">面向用户的提示</param>
        public PreviewConvertException(string code, string message) : base(message)
        {
            Code = code;
        }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="code">失败原因码</param>
        /// <param name="message">面向用户的提示</param>
        /// <param name="inner">内部异常</param>
        public PreviewConvertException(string code, string message, Exception inner) : base(message, inner)
        {
            Code = code;
        }
    }

    /// <summary>
    /// 预览失败原因码
    /// </summary>
    public static class PreviewErrorCode
    {
        /// <summary>
        /// 文件不存在或已丢失
        /// </summary>
        public const string FileMissing = "file_missing";
        /// <summary>
        /// 该格式不支持预览
        /// </summary>
        public const string Unsupported = "unsupported";
        /// <summary>
        /// 服务器未安装或未开启 LibreOffice
        /// </summary>
        public const string LibreOfficeMissing = "lo_missing";
        /// <summary>
        /// LibreOffice 转换超时
        /// </summary>
        public const string LibreOfficeTimeout = "lo_timeout";
        /// <summary>
        /// LibreOffice 转换失败
        /// </summary>
        public const string LibreOfficeFailed = "lo_failed";
        /// <summary>
        /// NPOI 解析表格失败
        /// </summary>
        public const string NpoiFailed = "npoi_failed";
    }
}
