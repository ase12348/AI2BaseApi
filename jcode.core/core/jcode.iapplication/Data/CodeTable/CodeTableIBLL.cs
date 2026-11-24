using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.orm;
using jcode.iapplication.Data.CodeTable;
using jcode.util;
using SqlSugar;

namespace jcode.iapplication
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present null
    /// 创建人：施赛一
    /// 日 期： 2021-09-18 10:38:26
    /// 描 述： CodeTable
    /// </summary>
    public interface CodeTableIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取CodeTable的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="appId">所属应用主键（为空表示不过滤）</param>
        /// <returns></returns>
        Task<IEnumerable<CodeTableEntity>> GetPageList(Pagination pagination, string keyWord,string dbCode, string appId = null);

        /// <summary>
        /// 获取主表lr_db_codetable的实体
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <returns></returns>
        Task<CodeTableEntity> GetEntity(string f_Id);

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="tableNames">数据库表名</param>
        /// <returns></returns>
        Task<IEnumerable<CodeTableEntity>> GetEntitys(string dbCode, List<string> tableNames);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        Task Delete(String f_Id);

        /// <summary>
        /// 批量删除登记：只删元数据，不动物理表
        /// </summary>
        /// <param name="ids">登记主键，逗号分隔</param>
        Task Deletes(string ids);
        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <param name="lr_db_codetableEntity">数据库表信息lr_db_codetable实体数据</param>
        /// <param name="lr_db_codecolumnsList">数据库表字段信息lr_db_codecolumns实体数据列表</param>
        Task<bool> SaveEntity(string f_Id, CodeTableEntity lr_db_codetableEntity, IEnumerable<CodeColumnsEntity> lr_db_codecolumnsList);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 导入表信息
        /// </summary>
        /// <param name="dbcode">数据库编码</param>
        /// <param name="tableList">数据表信息</param>
        /// <param name="appId">所属应用主键（为空表示不归属应用）</param>
        /// <returns></returns>
        Task ImportTable(string dbcode, List<DbTableInfo> tableList, string appId = null);

        /// <summary>
        /// 创建更新数据库表
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="dropFields">确认过要回收的列名，逗号分隔；不给就只加不减</param>
        /// <returns></returns>
        Task CreateOrUpdateTable(string tableid, string dbcode, string dbName, string dropFields = null);

        /// <summary>
        /// 模型与物理表的列差异：发「更新到表」之前先拿它确认会动到哪些列
        /// </summary>
        /// <param name="tableid">数据表id</param>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        Task<CodeTableColumnDiffDto> GetColumnDiff(string tableid, string dbcode);

        /// <summary>
        /// 数据表模型里已经不该存在的登记行：库里没有这张表，或它是宿主自有表
        /// </summary>
        /// <param name="dbcode">数据库编码</param>
        /// <returns></returns>
        Task<List<CodeTableInvalidDto>> GetInvalidTableList(string dbcode);

        /// <summary>
        /// 创建更新数据库表
        /// </summary>
        /// <param name="dbcode">数据库id</param>
        /// <param name="dbName">数据库名称</param>
        /// <param name="codeTableEntity">数据对象实体</param>
        /// <param name="codeColumnsList">数据对象属性列表</param>
        /// <param name="dropFields">确认过要回收的列名，逗号分隔；不给就只加不减</param>
        /// <returns></returns>
        Task CreateOrUpdateTable(string dbcode, string dbName, CodeTableEntity codeTableEntity, IEnumerable<CodeColumnsEntity> codeColumnsList, string dropFields = null);

        /// <summary>
        /// 迁移数据库表结构
        /// </summary>
        /// <param name="fromDb">原数据库</param>
        /// <param name="toDb">目标数据库</param>
        /// <param name="filterTable">过滤表（不要迁移）</param>
        /// <returns></returns>
        Task MoveDataBase(string fromDb, string toDb, string filterTable = "");
        /// <summary>
        /// 导出数据库表数据
        /// </summary>
        /// <param name="dbcode"></param>
        /// <returns></returns>
        Task<string> ExportDatabase(string dbcode);

        /// <summary>
        /// 项目初始化的时候导入数据库
        /// </summary>
        /// <param name="path">sqlite数据库地址</param>
        /// <param name="code">目标库</param>
        /// <param name="isUpdateData">是否更新数据</param>
        /// <returns></returns>
        Task ImportDatabaseBySqllite(string path, string code, bool isUpdateData = true);
        #endregion
        
        /// <summary>
        /// 更新数据表数据
        /// </summary>
        /// <param name="tableDataUpdateModel">配置数据</param>
        Task UpdateTable(TableDataUpdateModel tableDataUpdateModel);
        
        
    }
}
