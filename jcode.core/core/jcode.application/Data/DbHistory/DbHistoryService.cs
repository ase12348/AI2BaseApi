using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class DbHistoryService: ServiceBase
    {
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<DbHistoryEntity>> GetPageList(Pagination pagination, DbHistoryEntity queryParams) {
            var expression = LinqExtensions.True<DbHistoryEntity>();

            if (queryParams.F_Action != null) {
                expression = expression.And(t => t.F_Action == queryParams.F_Action);
            }

            if (!queryParams.F_DbId.IsEmpty())
            {
                expression = expression.And(t => t.F_DbId == queryParams.F_DbId);
            }

            if (!queryParams.F_CreateUserName.IsEmpty()) {
                expression = expression.And(t => t.F_CreateUserName.Contains(queryParams.F_CreateUserName));
            }
            return this.BaseRepository().FindList(expression, pagination);
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
        public async Task SaveEntity(string code,string dbName,string tableName, List<DbColumnInfo> oldCols, List<DbColumnInfo> newCols)
        {
            List<DbHistoryEntity> list = new List<DbHistoryEntity>();

            if (oldCols != null)
            {
    
                // 如果存在获取表的列信息
                foreach (var col in newCols) {
                    var oldCol = oldCols.Find(t => t.DbColumnName == col.DbColumnName);
                    var des = CreateHistory(col, oldCol);

                    if (!string.IsNullOrEmpty(des)) {
                        list.Add(new DbHistoryEntity()
                        {
                            F_Action = 2,
                            F_TableName = tableName,
                            F_DbId = code,
                            F_DbName = dbName,
                            F_Des = des,
                            F_Id = Guid.NewGuid().ToString(),
                            F_CreateTime = DateTime.Now,
                            F_CreateUserId = this.GetUserId(),
                            F_CreateUserName = this.GetUserName(),
                        });
                    }
                }

                


                // 删除列
                var removeList = oldCols.FindAll(t => newCols.FindIndex(t2 => t2.DbColumnName == t.DbColumnName) == -1);
                foreach (var col in removeList)
                {
                    list.Add(new DbHistoryEntity()
                    {
                        F_Action = 2,
                        F_TableName = tableName,
                        F_DbId = code,
                        F_DbName = dbName,
                        F_Des = CreateHistory(null,col),
                        F_Id = Guid.NewGuid().ToString(),
                        F_CreateTime = DateTime.Now,
                        F_CreateUserId = this.GetUserId(),
                        F_CreateUserName = this.GetUserName(),
                    });
                }
            }
            else {
                list.Add(new DbHistoryEntity()
                {
                    F_Action = 1,
                    F_TableName = tableName,
                    F_DbId = code,
                    F_DbName = dbName,
                    F_Des = "新建表",
                    F_Id = Guid.NewGuid().ToString(),
                    F_CreateTime = DateTime.Now,
                    F_CreateUserId = this.GetUserId(),
                    F_CreateUserName = this.GetUserName(),
                });
            }
            await this.BaseRepository().Inserts(list);
        }

        /// <summary>
        /// 创建列的变更信息
        /// </summary>
        /// <param name="newCol"></param>
        /// <param name="oldCol"></param>
        /// <returns></returns>
        private string CreateHistory(DbColumnInfo newCol, DbColumnInfo oldCol) {
            var strSql = new StringBuilder();
            if (oldCol == null && newCol != null)
            {
                // 新增列
                strSql.Append($"新建字段{newCol.ColumnDescription}{newCol.DbColumnName},");
                strSql.Append($"类型{newCol.DataType},");
                strSql.Append($"长度{newCol.Length},");
                strSql.Append($"精度{newCol.DecimalDigits},");
                strSql.Append($"主键{newCol.IsPrimarykey},");
                strSql.Append($"自增键{newCol.IsIdentity},");
                strSql.Append($"为空{newCol.IsNullable},");
            }
            else if (oldCol != null && newCol != null)
            {
                bool isUpdate = false;
                // 更新字段
                strSql.Append($"更新字段{newCol.ColumnDescription}{newCol.DbColumnName},");
                if (newCol.DataType != oldCol.DataType)
                {
                    strSql.Append($"类型{oldCol.DataType}变为{newCol.DataType},");
                    isUpdate = true;
                }

                if (newCol.Length != oldCol.Length)
                {
                    strSql.Append($"长度{oldCol.Length}变为{newCol.Length},");
                    isUpdate = true;
                }

                if (newCol.DecimalDigits != oldCol.DecimalDigits)
                {
                    strSql.Append($"精度{oldCol.DecimalDigits}变为{newCol.DecimalDigits},");
                    isUpdate = true;
                }

                if (newCol.IsPrimarykey != oldCol.IsPrimarykey)
                {
                    strSql.Append($"主键{oldCol.IsPrimarykey}变为{newCol.IsPrimarykey},");
                    isUpdate = true;
                }
                if (newCol.IsIdentity != oldCol.IsIdentity)
                {
                    strSql.Append($"自增键{oldCol.IsIdentity}变为{newCol.IsIdentity},");
                    isUpdate = true;
                }
                if (newCol.IsNullable != oldCol.IsNullable)
                {
                    strSql.Append($"为空{oldCol.IsNullable}变为{newCol.IsNullable},");
                    isUpdate = true;
                }

                if (!isUpdate) {
                    return "";
                }
            }
            if (oldCol != null && newCol == null)
            {
                // 删除
                strSql.Append($"删除字段{oldCol.ColumnDescription}{oldCol.DbColumnName},");
                strSql.Append($"类型{oldCol.DataType},");
                strSql.Append($"长度{oldCol.Length},");
                strSql.Append($"精度{oldCol.DecimalDigits},");
                strSql.Append($"主键{oldCol.IsPrimarykey},");
                strSql.Append($"自增键{oldCol.IsIdentity},");
                strSql.Append($"为空{oldCol.IsNullable},");
            }

            return strSql.ToString();
        }
    }
}
