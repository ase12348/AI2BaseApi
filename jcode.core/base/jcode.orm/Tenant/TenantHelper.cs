using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;

namespace jcode.orm
{
    public class TenantHelper
    {
        /// <summary>
        /// 设置参数,添加租户id
        /// </summary>
        /// <param name="pars">参数</param>
        public static SugarParameter[] SetParams(SugarParameter[] pars) {
            var tenantNo = ContextHelper.GetItem("TenantNo") as string;
            if (!string.IsNullOrEmpty(tenantNo))
            {

                if (pars != null)
                {
                    bool isAdd = false;
                    foreach (var par in pars)
                    {
                        if (par.ParameterName.ToLower().IndexOf("f_tenantid") != -1)
                        {
                            if (par.ParameterName.ToLower() == "@f_tenantid")
                            {
                                isAdd = true;
                            }

                            if ((par.Value == null || string.IsNullOrEmpty(par.Value.ToString())))
                            {
                                isAdd = true;
                                par.Value = tenantNo;
                                //break;考虑批量插入问题
                            }
                        }

                    }

                    if (!isAdd)
                    {
                        var parList = new List<SugarParameter>(pars);
                        parList.Add(new SugarParameter("@F_TenantId", tenantNo));
                        pars = parList.ToArray();
                    }
                }
                else
                {
                    var parList = new List<SugarParameter>();
                    parList.Add(new SugarParameter("@F_TenantId", tenantNo));
                    pars = parList.ToArray();
                }
            }
            else {
                throw (new Exception("多租户表隔离模式下,获取不到租户编码！"));
            }
            return pars;
        }


        /// <summary>
        /// 设置sql语句
        /// </summary>
        /// <param name="sql">sql</param>
        /// <returns></returns>
        public static string SetSqlTenantId(string sql)
        {
            var tenantNo = ContextHelper.GetItem("TenantNo") as string;
            if (!string.IsNullOrEmpty(tenantNo))
            {
                sql = sql.Replace("{JCODE_SASSID}", $"AND t.F_TenantId = '{tenantNo}'");
                sql = sql.Replace("{JCODE_SASSID_NOTT}", $"AND F_TenantId = '{tenantNo}'");
                sql = sql.Replace("{JCODE_SASSID_NOTA}", $"F_TenantId = '{tenantNo}'");
            }
            else {
                throw (new Exception("多租户表隔离模式下,获取不到租户编码！"));
            }
                
            return sql;
        }

        /// <summary>
        /// 设置sql语句
        /// </summary>
        /// <param name="sql">sql</param>
        /// <param name="pars">参数</param>
        /// <returns></returns>
        public static string SetSql(string sql, SugarParameter[] pars)
        {
            sql = System.Text.RegularExpressions.Regex.Replace(sql, "[a-z,A-Z,0-9,.]*{JCODE_SASSID_NOTA}", " 1=1 ");
            sql = sql.Replace("{JCODE_SASSID}", "");
            sql = sql.Replace("{JCODE_SASSID_NOTT}", "");
            return sql;
        }

        /// <summary>
        /// 添加租户参数
        /// </summary>
        /// <param name="dbParameters">参数</param>
        /// <param name="isTenant">是否开启租户模式</param>
        public static void AddParam(List<SugarParameter> dbParameters, bool isTenant)
        {
            if (isTenant && dbParameters != null) {
                if (dbParameters.FindIndex(t => t.ParameterName.ToLower().IndexOf("f_tenantid") != -1) == -1)
                {
                    dbParameters.Add(new SugarParameter("@F_TenantId", null));
                }

                
            }
        }

        /// <summary>
        /// 检查sql语句并添加参数
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="isTenant">是否开启租户模式</param>
        public static string CheckTenantId(string sql, bool isTenant) {
            if (isTenant)
            {
                var sql2 = sql.ToLower().Replace("f_tenantid,", "").Replace("f_tenantid ,", "");

                if (sql2.IndexOf("f_tenantid") != -1)
                {
                    throw (new Exception("多租户表隔离模式下，不能在SQL语句中出现f_tenantid！"));
                }


                if (sql.IndexOf("{JCODE_SASSID") == -1 && sql.IndexOf("{JCODE_SASSID_NOTT}") == -1)
                {
                    throw (new Exception("多租户模式下，语句里没有添加租户过滤标记{JCODE_SASSID}"));
                }
            }

            return sql;
        }
    }
}
