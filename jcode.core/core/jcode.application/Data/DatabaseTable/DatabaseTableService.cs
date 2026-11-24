using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：数据库表管理
    /// </summary>
    public class DatabaseTableService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 数据表列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public List<DbTableInfo> GetTableList(string code, bool isCache = false)
        {
            return this.BaseRepository(code).GetTableInfoList(isCache);
        }
        /// <summary>
        /// 获取数据库表字段信息
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <param name="isCache">是否读取存储区</param>
        /// <returns></returns>
        public List<DbColumnInfo> GetColumnInfosByTableName(string code, string tableName, bool isCache = false)
        {
            return this.BaseRepository(code).GetColumnInfosByTableName(tableName, isCache);
        }

        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表明</param>
        /// <param name="field">字段</param>
        /// <param name="logic">逻辑</param>
        /// <param name="keyword">关键字</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public Task<DataTable> GetTableDataList(string code, string tableName, string field, string logic, string keyword, Pagination pagination)
        {
            string sqlStr = "SELECT * FROM " + tableName + " where 1=1 {JCODE_SASSID_NOTT}";//SQL语句
            StringBuilder whereStr = new StringBuilder();
            if (!string.IsNullOrEmpty(keyword) && !string.IsNullOrEmpty(logic) && logic != "Null" && logic != "NotNull")
            {
                whereStr.Append(" AND " + field + " ");
                switch (logic)
                {
                    case "Equal":           //等于
                        whereStr.Append(" = ");
                        break;
                    case "NotEqual":        //不等于
                        whereStr.Append(" <> ");
                        break;
                    case "Greater":         //大于
                        whereStr.Append(" > ");
                        break;
                    case "GreaterThan":     //大于等于
                        whereStr.Append(" >= ");
                        break;
                    case "Less":            //小于
                        whereStr.Append(" < ");
                        break;
                    case "LessThan":        //小于等于
                        whereStr.Append(" >= ");
                        break;
                    case "Like":            //包含
                        whereStr.Append(" like ");
                        keyword = "%" + keyword + "%";
                        break;
                    default:
                        break;
                }
                whereStr.Append("@keyword ");
            }
            if (logic == "Null")
            {
                whereStr.Append(" AND " + field + " is null ");
            }
            else if (logic == "NotNull")
            {
                whereStr.Append(" AND " + field + " is not null ");
            }
            
            

            return this.BaseRepository(code).FindTable(sqlStr, null, pagination," 1 = 1 " + whereStr.ToString(), new { keyword });
        }
        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="tableName">表名</param>
        /// <returns></returns>
        public Task<DataTable> GetTableDataList(string code, string tableName)
        {
            StringBuilder strSql = new StringBuilder();
            strSql.Append(" SELECT * FROM " + tableName + " where 1=1 {JCODE_SASSID_NOTT} ");
            return this.BaseRepository(code).FindTable(strSql.ToString());
        }

        /// <summary>
        /// 数据库表数据列表
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="strSql">表名</param>
        /// <returns></returns>
        public Task<IEnumerable<string>> GetSqlColName(string code, string strSql)
        {
            return this.BaseRepository(code).GetSqlColName(strSql);
        }

        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断数据表字段重复
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="tableName">表名</param>
        /// <param name="keyName">主键名</param>
        /// <param name="filedsJson">数据字段</param>
        /// <param name="isNotSass">是否关闭开启</param>
        /// <returns></returns>
        public async Task<bool> ExistFiled(string keyValue, string tableName, string keyName, string filedsJson,bool isNotSass = false)
        {
            StringBuilder strSql = new StringBuilder();
            var tableNameList = tableName.Split(".");
            var dbCode = string.Empty;
            if (tableNameList.Length == 2) {
                dbCode = tableNameList[0];
                tableName = tableNameList[1];
            }
            

            strSql.Append("select * from " + tableName + " t where 1=1 {JCODE_SASSID} ");
            
            
            
            if (!string.IsNullOrEmpty(keyValue))
            {
                strSql.Append(" AND [" + keyName + "] !=@keyValue ");
            }

            /*if (ConfigHelper.GetConfig().OpenAppCodeFilter)
            {
                strSql.Append(" AND [F_AppCode] =@JcodeAppCode ");
            }*/

            var args = filedsJson.ToJObject();
            string argsstr = string.Empty;
            foreach (var item in args.Properties())
            {
                if (!string.IsNullOrEmpty(item.Value.ToString()))
                {
                    argsstr += " AND [" + item.Name + "] = '" + item.Value + "'";
                    
                }
            }
            //若是查重的数据字段都为空，直接返回true
            if (string.IsNullOrEmpty(argsstr))
            {
                return true;
            }
            else
            {
                strSql.Append(argsstr);
            }
            
            var appCode = ConfigHelper.GetConfig().AppCode;
            var dt = await this.BaseRepository(dbCode).FindTable(strSql.ToString(), new { keyValue,JcodeAppCode = appCode });
            if (dt.Rows.Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        #endregion
    }
}
