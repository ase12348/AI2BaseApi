using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present null
    /// 创建人：施赛一
    /// 日 期： 2021-09-18 10:38:26
    /// 描 述： CodeTable数据库执行类
    /// </summary>
    public class CodeTableService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取CodeTable的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeTableEntity>> GetPageList(Pagination pagination, string keyWord,string dbCode,string appId = null)
        {
            var expression = LinqExtensions.True<CodeTableEntity>();
            if (!string.IsNullOrEmpty(keyWord)) {
                expression = expression.And(t => t.F_ClassName.Contains(keyWord) || t.F_TableName.Contains(keyWord) || t.F_Description.Contains(keyWord));
            }
            if (!string.IsNullOrEmpty(dbCode))
            {
                expression = expression.And(t => t.F_DbId == dbCode);
            }
            if (!string.IsNullOrEmpty(appId))
            {
                expression = expression.And(t => t.F_AppId == appId);
            }

            return this.BaseRepository().FindList(expression,pagination);
        }

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeTableEntity>> GetList(string dbCode)
        {
            return this.BaseRepository().FindList<CodeTableEntity>(t=>t.F_DbId == dbCode);
        }

        /// <summary>
        /// 获取主表lr_db_codetable的实体
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <returns></returns>
        public Task<CodeTableEntity> GetEntity(String f_Id)
        {
            return this.BaseRepository().FindEntityByKey<CodeTableEntity>(f_Id);
        }
        
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">虚拟类</param>
        /// <returns></returns>
        public Task<CodeTableEntity> GetEntity(string keyValue,CodeTableEntity entity)
        {
            var expression = LinqExtensions.True<CodeTableEntity>();
            if (!string.IsNullOrEmpty(keyValue))
            {
                expression = expression.And(t => t.F_Id != keyValue);
            }
            expression = expression.And(t => t.F_DbId == entity.F_DbId && t.F_TableName == entity.F_TableName);
            return this.BaseRepository().FindEntity(expression);
        }

        /// <summary>
        /// 获取实体集合
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="tableNames">数据库表名</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeTableEntity>> GetEntitys(string dbCode, List<string> tableNames)
        {
            var expression = LinqExtensions.True<CodeTableEntity>();
            expression = expression.And(t => t.F_DbId == dbCode && tableNames.Contains(t.F_TableName));
            return this.BaseRepository().FindList(expression);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        /// <param name="CodeTableEntity">数据库表信息lr_db_codetable实体数据</param>
        public async Task SaveEntity(string f_Id,CodeTableEntity CodeTableEntity)
        {
            if(!string.IsNullOrEmpty(f_Id)) // 更新
            {
                CodeTableEntity.F_Id = f_Id;
                // 更新实体是全列覆盖：请求里没带应用归属时沿用原值，避免把 F_AppId 洗成 NULL
                if (string.IsNullOrEmpty(CodeTableEntity.F_AppId))
                {
                    var old = await this.BaseRepository().FindEntityByKey<CodeTableEntity>(f_Id);
                    CodeTableEntity.F_AppId = old?.F_AppId;
                }
                await this.BaseRepository().Update(CodeTableEntity);
            }
            else // 新增
            {
                if(string.IsNullOrEmpty(CodeTableEntity.F_Id))
                {
                    CodeTableEntity.F_Id = Guid.NewGuid().ToString();
                }
                await this.BaseRepository().Insert(CodeTableEntity);
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">唯一标识码(GUID)</param>
        public async Task Delete(string f_Id)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<CodeTableEntity>(f_Id);
                await db.Delete<CodeColumnsEntity>(t => t.F_CodeTableId == f_Id);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 批量删除登记：只删元数据两张表的行，物理表一张都不动
        /// </summary>
        /// <param name="ids">登记主键，逗号分隔</param>
        public async Task Deletes(string ids)
        {
            var idList = SplitIds(ids);
            if (idList.Count == 0)
            {
                throw new Exception("没有要删除的数据表登记");
            }

            var db = this.BaseRepository().BeginTrans();
            try
            {
                foreach (var f_Id in idList)
                {
                    await db.Delete<CodeTableEntity>(f_Id);
                    await db.Delete<CodeColumnsEntity>(t => t.F_CodeTableId == f_Id);
                }
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        /// <summary>
        /// 逗号分隔的登记主键去重成列表
        /// </summary>
        /// <param name="ids">逗号分隔的主键</param>
        /// <returns></returns>
        public static List<string> SplitIds(string ids)
        {
            if (string.IsNullOrEmpty(ids))
            {
                return new List<string>();
            }
            var res = new List<string>();
            foreach (var item in ids.Split(','))
            {
                var id = item.Trim();
                if (id.Length > 0 && !res.Contains(id))
                {
                    res.Add(id);
                }
            }
            return res;
        }
        #endregion
        

    }
}
