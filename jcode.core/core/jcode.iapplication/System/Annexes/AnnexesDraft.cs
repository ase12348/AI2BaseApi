using System;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：附件暂存夹约定
    /// 表单未点击保存前上传的附件，先挂在 "{附件夹主键}{FolderSuffix}" 这个暂存附件夹下，
    /// 业务数据保存成功后由 CommitDraft 转正到真实附件夹；表单取消或超时由清理任务回收。
    /// 前端 web-pc-vue3 的 uploadDraft.ts 中有同名后缀，两侧必须保持一致。
    /// </summary>
    public static class AnnexesDraft
    {
        /// <summary>
        /// 暂存附件夹后缀
        /// </summary>
        public const string FolderSuffix = "-draft";

        /// <summary>
        /// 由真实附件夹主键得到暂存附件夹主键
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        /// <returns></returns>
        public static string ToDraftFolderId(string folderId)
        {
            if (string.IsNullOrEmpty(folderId) || IsDraftFolderId(folderId))
            {
                return folderId;
            }

            return folderId + FolderSuffix;
        }

        /// <summary>
        /// 是否暂存附件夹主键
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <returns></returns>
        public static bool IsDraftFolderId(string folderId)
        {
            return !string.IsNullOrEmpty(folderId)
                && folderId.Length > FolderSuffix.Length
                && folderId.EndsWith(FolderSuffix, StringComparison.Ordinal);
        }

        /// <summary>
        /// 由暂存附件夹主键得到真实附件夹主键
        /// </summary>
        /// <param name="folderId">暂存附件夹主键</param>
        /// <returns></returns>
        public static string ToRealFolderId(string folderId)
        {
            if (!IsDraftFolderId(folderId))
            {
                return folderId;
            }

            return folderId.Substring(0, folderId.Length - FolderSuffix.Length);
        }
    }
}
