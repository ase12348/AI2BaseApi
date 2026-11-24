using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2022.03.20
    /// 描 述：数据库操作记录
    /// </summary>
    public class DbHistoryBLL : BLLBase, DbHistoryIBLL, BLL
    {
        private readonly DbHistoryService dbHistoryService = new DbHistoryService();


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbHistoryEntity>> GetPageList(Pagination pagination, DbHistoryEntity queryParams)
        {
            return dbHistoryService.GetPageList(pagination, queryParams);
        }


        /// <summary>
        /// 保存数据表的变更历史
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="dbName">数据库信息</param>
        /// <param name="tableName">数据表名</param>
        /// <param name="oldCols">旧数据表信息</param>
        /// <param name="newCols">新数据表信息</param>
        /// <returns></returns>
        public async Task SaveEntity(string code, string dbName, string tableName, List<DbColumnInfo> oldCols, List<DbColumnInfo> newCols)
        {
            await dbHistoryService.SaveEntity(code, dbName, tableName, oldCols, newCols);
        }
    }
}
