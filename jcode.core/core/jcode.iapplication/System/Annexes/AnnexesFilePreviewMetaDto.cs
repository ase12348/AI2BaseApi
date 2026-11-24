namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.10.26
    /// 描 述：附件预览方式元数据，前端据此选择渲染器
    /// </summary>
    public class AnnexesFilePreviewMetaDto
    {
        /// <summary>
        /// 文件主键
        /// </summary>
        public string FileId { get; set; }
        /// <summary>
        /// 文件名称
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// 文件后缀（小写，不含点）
        /// </summary>
        public string Ext { get; set; }
        /// <summary>
        /// 文件大小（字节）
        /// </summary>
        public long Size { get; set; }
        /// <summary>
        /// 预览方式，见 <see cref="AnnexesFilePreviewMode"/>
        /// </summary>
        public string Mode { get; set; }
        /// <summary>
        /// 是否可以在线预览
        /// </summary>
        public bool CanPreview { get; set; }
        /// <summary>
        /// 不能预览或预览可能失败的原因码，为空表示无异常
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 原因对应的提示文案
        /// </summary>
        public string ReasonMessage { get; set; }
        /// <summary>
        /// 下载地址（相对路径，不含 token）
        /// </summary>
        public string DownloadUrl { get; set; }
    }

    /// <summary>
    /// 附件预览方式
    /// </summary>
    public static class AnnexesFilePreviewMode
    {
        /// <summary>
        /// 浏览器直接显示图片
        /// </summary>
        public const string Image = "image";
        /// <summary>
        /// 服务端已可渲染（pdf/txt/csv/html/转换后的 office），用 iframe 打开预览地址
        /// </summary>
        public const string Iframe = "iframe";
        /// <summary>
        /// 前端 docx-preview 渲染原始文件
        /// </summary>
        public const string Docx = "docx";
        /// <summary>
        /// 前端 pptx-preview 渲染原始文件
        /// </summary>
        public const string Pptx = "pptx";
        /// <summary>
        /// 无法预览，只能下载
        /// </summary>
        public const string Download = "download";
    }
}
