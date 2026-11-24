using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：附件管理
    /// </summary>
    public class AnnexesFileService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取实体列表
        /// </summary>
        /// <param name="folderId">文件夹值</param>
        /// <returns></returns>
        public Task<IEnumerable<AnnexesFileEntity>> GetList(string folderId)
        {
            return this.BaseRepository().FindList<AnnexesFileEntity>(t=>t.F_FolderId == folderId, "F_CreateDate");
        }
        
        /// <summary>
        /// 获取全部文件数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<AnnexesFileEntity>> GetAllList()
        {
            return this.BaseRepository().FindAll<AnnexesFileEntity>("F_CreateDate");
        }
        /// <summary>
        /// 获取附件名称集合
        /// </summary>
        /// <param name="folderId">主键值</param>
        /// <returns></returns>
        public async Task<string> GetFileNames(string folderId)
        {
            string res = "";
            IEnumerable<AnnexesFileEntity> list = await GetList(folderId);
            foreach (var item in list)
            {
                if (!string.IsNullOrEmpty(res))
                {
                    res += ",";
                }
                res += item.F_FileName;
            }
            return res;
        }
        /// <summary>
        /// 获取附件实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<AnnexesFileEntity> GetEntity(string keyValue)
        {
            var entity = await this.BaseRepository().FindEntityByKey<AnnexesFileEntity>(keyValue);
            if (entity == null) {
                entity = await this.BaseRepository(false,true).FindEntityByKey<AnnexesFileEntity>(keyValue);
            }
            return entity;
        }

        public async Task<AnnexesFileEntity> GetMyEntity(string keyValue)
        {
            var entity = await this.BaseRepository().FindEntityByKey<AnnexesFileEntity>(keyValue);
            return entity;
        }

        /// <summary>
        /// 获取超过指定时间且已登记归属信息的附件
        /// </summary>
        /// <param name="deadline">创建时间上限</param>
        /// <param name="maxRows">单次最大处理条数</param>
        /// <returns></returns>
        public Task<IEnumerable<AnnexesFileEntity>> GetOrphanCandidates(DateTime deadline, int maxRows)
        {
            var pagination = new Pagination { page = 1, rows = maxRows, sidx = "F_CreateDate", sord = "asc" };
            return this.BaseRepository().FindList<AnnexesFileEntity>(
                t => t.F_CreateDate != null && t.F_CreateDate < deadline
                     && t.F_TableName != null && t.F_TableName != ""
                     && t.F_TableField != null && t.F_TableField != "",
                pagination);
        }

        /// <summary>
        /// 获取超过保留时长仍未转正的暂存附件
        /// </summary>
        /// <param name="deadline">创建时间上限</param>
        /// <param name="suffix">暂存附件夹后缀</param>
        /// <param name="maxRows">单次最大处理条数</param>
        /// <returns></returns>
        public Task<IEnumerable<AnnexesFileEntity>> GetDraftCandidates(DateTime deadline, string suffix, int maxRows)
        {
            var pagination = new Pagination { page = 1, rows = maxRows, sidx = "F_CreateDate", sord = "asc" };
            // 后缀匹配用原生 LIKE 片段下发，不依赖表达式树对 EndsWith 的翻译；
            // 条件仍是参数化的，租户过滤由仓储自动追加
            return this.BaseRepository().FindList<AnnexesFileEntity>(
                t => t.F_CreateDate != null && t.F_CreateDate < deadline,
                pagination,
                "F_FolderId LIKE @draftSuffix",
                new { draftSuffix = $"%{suffix}" });
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存数据实体
        /// </summary>
        /// <param name="keyValue">附件夹主键</param>
        /// <param name="annexesFileEntity">附件实体数据</param>
        public async Task SaveEntity(string keyValue, AnnexesFileEntity annexesFileEntity)
        {
            annexesFileEntity.F_CreateDate = DateTime.Now;

            if (string.IsNullOrEmpty(keyValue))
            {
                await this.BaseRepository().Insert(annexesFileEntity);
            }
            else {
                annexesFileEntity.F_Id = keyValue;
                await this.BaseRepository().Update(annexesFileEntity);
            }
        }
        /// <summary>
        /// 删除附件
        /// </summary>
        /// <param name="fileId">文件主键</param>
        public async Task DeleteEntity(string fileId)
        {
            await this.BaseRepository().Delete<AnnexesFileEntity>(fileId);
        }

        /// <summary>
        /// 回写附件归属信息
        /// </summary>
        /// <param name="folderId">附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        public async Task UpdateOwner(string folderId, string dbCode, string tableName, string tableField)
        {
            await this.BaseRepository().Update<AnnexesFileEntity>(
                new { F_DbCode = dbCode, F_TableName = tableName, F_TableField = tableField },
                t => t.F_FolderId == folderId);
        }

        /// <summary>
        /// 将暂存附件转正到真实附件夹，并同步登记归属信息
        /// </summary>
        /// <param name="draftFolderId">暂存附件夹主键</param>
        /// <param name="folderId">真实附件夹主键</param>
        /// <param name="dbCode">关联数据库</param>
        /// <param name="tableName">关联表名</param>
        /// <param name="tableField">关联字段名</param>
        public async Task PromoteDraft(string draftFolderId, string folderId, string dbCode, string tableName, string tableField)
        {
            await this.BaseRepository().Update<AnnexesFileEntity>(
                new { F_FolderId = folderId, F_DbCode = dbCode, F_TableName = tableName, F_TableField = tableField },
                t => t.F_FolderId == draftFolderId);
        }
        #endregion
    }
}
