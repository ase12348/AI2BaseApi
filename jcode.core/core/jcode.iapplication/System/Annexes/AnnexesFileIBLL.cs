using jcode.autofac;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：附件管理
    /// </summary>
    public interface AnnexesFileIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="keyValue">附件夹主键</param>
        /// <returns></returns>
        Task<IEnumerable<AnnexesFileEntity>> GetList(string keyValue);
        /// <summary>
        /// 获取附件名称集合
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task<string> GetFileNames(string keyValue);
        /// <summary>
        /// 获取附件实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<AnnexesFileEntity> GetEntity(string keyValue);

        /// <summary>
        /// 获取附件实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<AnnexesFileEntity> GetEntityByFolderId(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据实体
        /// </summary>
        /// <param name="keyValue">附件夹主键</param>
        /// <param name="annexesFileEntity">附件实体数据</param>
        Task SaveEntity(string keyValue, AnnexesFileEntity annexesFileEntity);
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        Task DeleteEntity(string fileId);
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        Task DeleteEntityByFolderId(string folderId);

        /// <summary>
        /// 按附件夹主键批量删除附件（同时删除存储的物理文件）
        /// </summary>
        /// <param name="folderIds">附件夹主键集合</param>
        Task DeleteByFolderIds(IEnumerable<string> folderIds);

        /// <summary>
        /// 回写附件归属信息，供孤儿附件清理时反查业务表
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段名</param>
        Task BindOwner(string folderId, string dbCode, string tableName, string tableField);

        /// <summary>
        /// 将暂存附件夹中的附件转正到真实附件夹，表单保存成功后调用
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        Task CommitDraft(string folderId, string dbCode, string tableName, string tableField);

        /// <summary>
        /// 丢弃暂存附件夹中的附件（记录与物理文件一并删除），表单取消关闭时调用
        /// </summary>
        /// <param name="folderId">真实附件夹主键</param>
        Task DiscardDraft(string folderId);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <param name="isDb">是否必须存储到数据库</param>
        /// <returns></returns>
        Task<AnnexesFileEntity> SaveAnnexes(string folderId, string fileGuid, string fileName, int chunks, string dbCode, string tableName, string tableField,bool isDb = false);

        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="bytes">文件数据</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <param name="isEdit">是否可编辑</param>
        /// <returns></returns>
        Task<bool> SaveAnnexes(string folderId, string fileGuid, string fileName, byte[] bytes, string dbCode, string tableName, string tableField, bool isEdit = false);

        /// <summary>
        /// 保存附件
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="path">文件地址</param>
        /// <param name="dbCode">关联表数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联表字段</param>
        /// <returns></returns>
        Task<bool> SaveAnnexes(string folderId, string fileGuid, string path, string dbCode, string tableName, string tableField);
        /// <summary>
        /// 保存附件（支持大文件分片传输）
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="fileName">文件名称</param>
        /// <param name="chunks">文件总共分多少片</param>
        /// <returns></returns>
        string SaveAnnexes(string fileGuid, string fileName, int chunks);
        /// <summary>
        /// 保存分片附件
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunk">分片文件序号</param>
        /// <param name="bytes">文件流</param>
        bool SaveChunkAnnexes(string fileGuid, int chunk, byte[] bytes);
        /// <summary>
        /// 移除文件分片数据
        /// </summary>
        /// <param name="fileGuid">文件主键</param>
        /// <param name="chunks">文件分片数</param>
        void RemoveChunkAnnexes(string fileGuid, int chunks);


        /// <summary>
        /// 获取下载文件实体
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        Task<AnnexesFileEntity> GetDownFileEntity(string fileId);
        
        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        Task<string> DownFile(string fileId);


        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="data">文件</param>
        /// <returns></returns>
        string DownFile(AnnexesFileEntity data);

        /// <summary>
        /// 删除临时文件
        /// </summary>
        void DeleteTempFile();
        #endregion

        /// <summary>
        /// 获取预览文件地址
        /// </summary>
        /// <param name="fileId"></param>
        /// <param name="isNotCache"></param>
        /// <returns></returns>
        Task<string> ToPreview(string fileId,int? isNotCache);

        /// <summary>
        /// 获取文件预览方式，前端据此选择渲染器
        /// </summary>
        /// <param name="fileId">文件ID</param>
        /// <returns></returns>
        Task<AnnexesFilePreviewMetaDto> GetPreviewMeta(string fileId);

        /// <summary>
        /// 获取文件的html格式，目前只支持  excel word ppt
        /// </summary>
        /// <param name="fileId"></param>
        /// <returns></returns>
        Task<string> ToHtml(string fileId);


        /// <summary>
        /// 存储转化
        /// </summary>
        Task TransformStorage();

        /// <summary>
        /// 清理孤儿附件：仅处理已登记归属、超过保留期且业务表中已无引用的附件
        /// </summary>
        /// <returns>实际删除的附件数量</returns>
        Task<int> CleanOrphanFiles();

        /// <summary>
        /// 回收遗留的暂存附件：表单关闭后未被转正、超过保留时长的暂存附件
        /// </summary>
        /// <returns>实际删除的附件数量</returns>
        Task<int> CleanDraftFiles();
    }
}
