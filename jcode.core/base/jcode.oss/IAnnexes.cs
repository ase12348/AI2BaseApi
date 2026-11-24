namespace jcode.oss
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2022-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.09.20
    /// 描 述：定义附件上传接口接口
    /// 修改：tobin 2022/10/08
    /// </summary>
    public interface IAnnexes
    {
        /// <summary>
        /// 上传附件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunk">分片序号</param>
        /// <param name="bytes">文件二进制数据</param>
        bool UploadFileChunk(string fileId, int chunk, byte[] bytes);

        /// <summary>
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        void RemoveFileChunk(string fileId, int chunks);

        /// <summary>
        /// 合并上传附件的分片数据
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="chunks">文件分片数</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <param name="isNotUpload">不上传（不是本地存储时有效）</param>
        /// <returns></returns>
        string MergeFileChunk(string fileId, string filePath, string fileEex, int chunks,int? isEncrypt,bool isNotUpload = false);

        /// <summary>
        /// 上传单个文件
        /// </summary>
        /// <param name="fileId">文件id</param>
        /// <param name="filePath">文件地址</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="content">文件内容</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <returns></returns>
        long UploadFile(string fileId, string filePath, string fileEex, byte[] content,int? isEncrypt);

        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="filePath">上传后的文件地址</param>
        /// <param name="myFilePath">当前文件地址</param>
        long UploadFile(string filePath, string myFilePath);

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileEex">文件扩展名</param>
        /// <param name="isEncrypt">文件是否加密</param>
        /// <param name="isDelete">是否删除</param>
        /// <returns></returns>
        string DownFile(string fileId, string filePath, string fileEex,int? isEncrypt,bool isDelete = false);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="filePath">文件主键</param>
        /// <returns></returns>
        void DeleteFile(string filePath);
    }
}
