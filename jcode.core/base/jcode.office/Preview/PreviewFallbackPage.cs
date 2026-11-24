using System.Net;

namespace jcode.office
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2026.10.26
    /// 描 述：预览失败时的兜底页，直接展示原因并给出下载入口
    /// </summary>
    public static class PreviewFallbackPage
    {
        /// <summary>
        /// 生成兜底页 HTML
        /// </summary>
        /// <param name="fileName">文件名称</param>
        /// <param name="downloadUrl">下载地址（不含 token）</param>
        /// <param name="message">失败原因</param>
        /// <returns></returns>
        public static string Render(string fileName, string downloadUrl, string message)
        {
            string safeFileName = WebUtility.HtmlEncode(fileName ?? "该文件");
            string safeMessage = WebUtility.HtmlEncode(message ?? "该文件暂不支持在线预览");
            string safeDownloadUrl = (downloadUrl ?? string.Empty)
                .Replace("\\", "\\\\")
                .Replace("'", "\\'")
                .Replace("\"", "\\\"");
            string download = string.IsNullOrEmpty(safeDownloadUrl)
                ? string.Empty
                : "<a class=\"jcode-button\" id=\"jcode-download\" href=\"javascript:void(0)\">下载文件</a>"
                    + "</div><script>"
                    + "var token='';try{token=new URLSearchParams(location.search).get('token')||'';}catch(e){}"
                    + "document.getElementById('jcode-download').onclick=function(){"
                    + "var url='" + safeDownloadUrl + "';"
                    + "if(token){url+=(url.indexOf('?')>-1?'&':'?')+'token='+encodeURIComponent(token);}"
                    + "location.href=url;};"
                    + "</script></body></html>";
            return "<!DOCTYPE html><html><head><meta charset=\"utf-8\"/><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"/>"
                + "<style>html,body{height:100%;margin:0;}body{display:flex;align-items:center;justify-content:center;background:#fff;"
                + "font:14px/1.6 -apple-system,'PingFang SC','Microsoft YaHei',sans-serif;color:#1f2329;}"
                + ".jcode-box{max-width:420px;padding:32px 24px;text-align:center;}"
                + ".jcode-icon{width:56px;height:56px;margin:0 auto 16px;border-radius:50%;background:#f2f3f5;position:relative;}"
                + ".jcode-icon:before,.jcode-icon:after{content:'';position:absolute;left:50%;top:50%;background:#8f959e;border-radius:1px;}"
                + ".jcode-icon:before{width:22px;height:2px;margin:-1px 0 0 -11px;}"
                + ".jcode-icon:after{width:2px;height:22px;margin:-11px 0 0 -1px;transform:rotate(45deg);}"
                + ".jcode-title{margin:0 0 8px;font-size:16px;font-weight:600;}"
                + ".jcode-name{margin:0 0 4px;font-size:13px;color:#646a73;word-break:break-all;}"
                + ".jcode-message{margin:0 0 20px;font-size:13px;color:#8f959e;word-break:break-all;}"
                + ".jcode-button{display:inline-block;padding:0 20px;height:32px;line-height:32px;border-radius:6px;"
                + "background:#3370ff;color:#fff;font-size:14px;text-decoration:none;}"
                + ".jcode-button:hover{background:#245bdb;}</style></head><body>"
                + "<div class=\"jcode-box\"><div class=\"jcode-icon\"></div>"
                + "<p class=\"jcode-title\">无法在线预览</p>"
                + "<p class=\"jcode-name\">" + safeFileName + "</p>"
                + "<p class=\"jcode-message\">" + safeMessage + "</p>"
                + (string.IsNullOrEmpty(download)
                    ? "</div></body></html>"
                    : download);
        }
    }
}
