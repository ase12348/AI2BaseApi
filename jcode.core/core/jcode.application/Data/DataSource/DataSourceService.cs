using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据源
    /// </summary>
    public class DataSourceService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">关键字</param>
        /// <param name="category">分类</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的数据，为空时不过滤</param>
        /// <returns></returns>
        public Task<IEnumerable<DataSourceEntity>> GetPageList(Pagination pagination, string keyword,string category,string appId = null)
        {
            var exp = Expressionable.Create<DataSourceEntity>()
                .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Name.Contains(keyword) || t.F_Code.Contains(keyword))
            .AndIF(!string.IsNullOrEmpty(category), t => t.F_Category == category)
            .AndIF(!string.IsNullOrEmpty(appId), t => t.F_AppId == appId);
            return this.BaseRepository().FindList(exp.ToExpression(), pagination);
        }
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DataSourceEntity>> GetList()
        {
            return this.BaseRepository().FindAll<DataSourceEntity>();
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<DataSourceEntity> GetEntityByCode(string code)
        {
            return this.BaseRepository().FindEntity<DataSourceEntity>(t=>t.F_Code == code);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DataSourceEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntity<DataSourceEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据源
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteEntity(string keyValue)
        {
            DataSourceEntity entity = new DataSourceEntity()
            {
                F_Id = keyValue,
            };
            await this.BaseRepository().Delete(entity);
        }
        /// <summary>
        /// 保存（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dataSourceEntity">数据源实体</param>
        /// <returns></returns>
        public async Task<bool> SaveEntity(string keyValue, DataSourceEntity dataSourceEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                DataSourceEntity entity = await this.BaseRepository()
                    .FindEntity<DataSourceEntity>(t=>t.F_Code == dataSourceEntity.F_Code && t.F_Id != keyValue);
                if (entity != null)
                {
                    return false;
                }
                dataSourceEntity.F_Id = keyValue;
                dataSourceEntity.F_ModifyDate = DateTime.Now;
                dataSourceEntity.F_ModifyUserId = this.GetUserId();
                dataSourceEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(dataSourceEntity,true);
            }
            else
            {
                DataSourceEntity entity = await GetEntityByCode(dataSourceEntity.F_Code);
                if (entity != null)
                {
                    return false;
                }
                dataSourceEntity.F_Id = Guid.NewGuid().ToString();
                dataSourceEntity.F_CreateDate = DateTime.Now;
                dataSourceEntity.F_CreateUserId = this.GetUserId();
                dataSourceEntity.F_CreateUserName = this.GetUserName();
                dataSourceEntity.F_ModifyDate = DateTime.Now;
                dataSourceEntity.F_ModifyUserId = this.GetUserId();
                dataSourceEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Insert(dataSourceEntity);
            }
            return true;
        }
        #endregion

        #region 扩展方法
        
        /// <summary>
        /// 获取sql的列
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDataColName(string code)
        {
            DataSourceEntity entity = await GetEntityByCode(code);
            if (entity == null)
            {
                return new List<string>();
            }
            
            DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
            List<string> res = new List<string>();
            foreach (var col in model.Columns) {
                res.Add(col.Prop);
            }
            return res;
        }

        /// <summary>
        /// 获取sql语句的列
        /// </summary>
        /// <param name="dbCode"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetDataColName(string dbCode,string sql)
        {
            /*sql = sql.Replace("@userId", "''");
            sql = sql.Replace("@userAccount", "''");
            sql = sql.Replace("@companyId", "''");
            sql = sql.Replace("@departmentId", "''");
            sql = sql.Replace("@param", "''");*/
            
            //sql = System.Text.RegularExpressions.Regex.Replace(sql, "in[\\s]+@[a-z,A-Z,0-9]+", "= ''");
            sql = System.Text.RegularExpressions.Regex.Replace(sql, "@[a-z,A-Z,0-9]+", "''");
            return await this.BaseRepository(dbCode).GetSqlColName(sql);
        }

        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="dbSource">数据源对象</param>
        /// <param name="whereSql">查询语句</param>
        /// <returns></returns>
        public async Task<DataTable> GetDataViews(DataSourceRes dbSource,string whereSql)
        {
            if (!string.IsNullOrEmpty(dbSource.Sql))
            {
                //重新构建sql语句-当参数为空时查询全部信息
                if (!dbSource.ParamsDic.ContainsKey("param"))//存在参数
                {
                    dbSource.Sql = dbSource.Sql.Replace("= @param", " is not null");
                    dbSource.Sql = dbSource.Sql.Replace("=@param", " is not null");
                }

                return await this.BaseRepository(dbSource.DbCode).FindTable(dbSource.Sql, whereSql, dbSource.Order, dbSource.ParamsDic);
            }
            else
            {
                DataTable res = dbSource.Data;

                if (!string.IsNullOrEmpty(whereSql))
                {
                    DataRow[] drs = res.Select(whereSql);
                    DataTable nDt = res.Clone();
                    foreach (DataRow dr in drs)
                    {
                        nDt.ImportRow(dr);
                    }
                    return nDt;
                }

                if (!string.IsNullOrEmpty(dbSource.Order))
                {
                    var dv = res.DefaultView;
                    dv.Sort = dbSource.Order;
                    res = dv.ToTable();
                }
                
                return res;
            }
        }
        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="dbSource">数据源对象</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="whereSql">查询语句</param>
        /// <returns></returns>
        public async Task<DataTable> GetDataViewPage(DataSourceRes dbSource, Pagination pagination, string whereSql)
        {
            if (!string.IsNullOrEmpty(dbSource.Sql))
            {
                if (!string.IsNullOrEmpty(dbSource.Order))// string.IsNullOrEmpty(pagination.sidx) &&
                {
                    pagination.sidx = dbSource.Order;
                }

                if (dbSource.Sql.ToUpper().Contains("WITH"))//WITH查询-不分页
                {
                    return await this.BaseRepository(dbSource.DbCode).FindTable(dbSource.Sql, whereSql, dbSource.Order, dbSource.ParamsDic);
                }
                else
                {
                    return await this.BaseRepository(dbSource.DbCode).FindTable(dbSource.Sql, dbSource.ParamsDic, pagination, whereSql);
                }
                
            }
            else {
                
                
                DataTable res = dbSource.Data;

                if (res.Rows.Count == 0)
                {
                    return res;
                }

                if (!string.IsNullOrEmpty(whereSql))
                {
                    DataRow[] drs = res.Select(whereSql);
                    DataTable nDt = res.Clone();
                    foreach (DataRow dr in drs)
                    {
                        nDt.ImportRow(dr);
                    }
                    return nDt;
                }

                if (!string.IsNullOrEmpty(dbSource.Order))
                {
                    var dv = res.DefaultView;
                    dv.Sort = dbSource.Order;
                    res = dv.ToTable();
                }
                else if (!string.IsNullOrEmpty(pagination.sidx))
                {
                    var dv = res.DefaultView;
                    dv.Sort = pagination.sidx;
                    res = dv.ToTable();
                }

                


                pagination.records = res.Rows.Count;

                if (res.Rows.Count <= pagination.rows)
                {
                    return res;
                }
                else
                {
                    DataTable newDt = res.Clone();
                    int bnum = (pagination.page - 1) * pagination.rows;
                    int endnum = bnum + pagination.rows;
                    if (endnum > res.Rows.Count)
                    {
                        endnum = res.Rows.Count;
                    }
                    for (var i = bnum; i < endnum; i++)
                    {
                        newDt.ImportRow(res.Rows[i]);
                    }

                    return newDt;

                }
            }
        }



        #endregion
        
        
        /// <summary>
        /// 获取数据库表列表
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <returns>表列表</returns>
        public List<DbTableInfo> GetTableList(string dbCode)
        {
            try
            {
                if (string.IsNullOrEmpty(dbCode) || dbCode == "lrsystemdb")
                {
                    return this.BaseRepository().GetTableInfoList();
                }
                else
                {
                    return this.BaseRepository(dbCode).GetTableInfoList();
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取表列表失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 获取表字段信息
        /// </summary>
        /// <param name="dbCode">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <returns>字段列表</returns>
        public List<DbColumnInfo> GetColumnInfos(string dbCode, string tableName)
        {
            try
            {
                if (string.IsNullOrEmpty(dbCode) || dbCode == "lrsystemdb")
                {
                    return this.BaseRepository().GetColumnInfosByTableName(tableName);
                }
                else
                {
                    return this.BaseRepository(dbCode).GetColumnInfosByTableName(tableName);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"获取表字段失败: {ex.Message}");
            }
        }


    }
}
