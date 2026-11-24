using jcode.iapplication;
using jcode.ilanguage;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace jcode.language
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.22
    /// 描 述：语言映射
    /// </summary>
    public class LGMapService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="code">语言类型编码</param>
        /// <returns></returns>
        public Task<IEnumerable<LGMapEntity>> GetListByTypeCode(string code)
        {
            return this.BaseRepository().FindList<LGMapEntity>(t=>t.F_TypeCode == code);
        }
        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param>
        /// <param name="types">语言类型列表</param>
        /// <returns></returns>
        public async Task<DataTable> GetPageList(Pagination pagination, string keyword, IEnumerable<LGTypeEntity> types)
        {
            var typeList = (List<LGTypeEntity>)types;
            DataTable objDataTable = new DataTable("Table");
            objDataTable.Columns.Add("f_code", typeof(string));
            int intFieldCount = typeList.Count;
            for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
            {
                objDataTable.Columns.Add(typeList[intCounter].F_Code.ToLower(), typeof(string));
            }


            var list = (List<LGMapEntity>)await this.BaseRepository().FindAll<LGMapEntity>(pagination.sidx);
            var mainList = list.FindAll(t => t.F_TypeCode == typeList[0].F_Code);
            var otherList = list.FindAll(t => t.F_TypeCode != typeList[0].F_Code);
            if (!string.IsNullOrWhiteSpace(keyword)) {
                mainList = mainList.FindAll(t => t.F_Name.Contains(keyword));
            }
            pagination.records = mainList.Count;
            var begin = (pagination.page - 1) * pagination.rows;
            if (begin >= pagination.records || mainList.Count == 0) {
                return objDataTable;
            }
            var end = begin + pagination.rows;
            if (end > mainList.Count) {
                end = mainList.Count;
            }
            

            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in otherList) {
                var key = $"{item.F_Code}_{item.F_TypeCode}";
                if (!dic.ContainsKey(key)) {
                    dic.Add(key, item.F_Name);
                }
            }

            for (var i = begin; i < end; i++) {
                var main = mainList[i];
                DataRow row = objDataTable.NewRow();
                row["f_code"] = main.F_Code;
                foreach (var lgType in typeList) {
                    if (lgType.F_Code == typeList[0].F_Code)
                    {
                        row[lgType.F_Code.ToLower()] = main.F_Name;
                    }
                    else if(dic.ContainsKey($"{main.F_Code}_{lgType.F_Code}"))
                    {
                        row[lgType.F_Code.ToLower()] = dic[$"{main.F_Code}_{lgType.F_Code}"];
                    }
                }
                objDataTable.Rows.Add(row);
            }
            return objDataTable;
        }


        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="nameList">需要翻译的主语言数据</param>
        /// <param name="types">语言类型列表</param>
        /// <returns></returns>
        public async Task<DataTable> GetList(List<string> nameList, IEnumerable<LGTypeEntity> types)
        {
            /*var typeList = (List<LGTypeEntity>)types;
            var strSql = new StringBuilder();
            strSql.Append("SELECT ");
            strSql.Append($"t.F_Code, t.F_Name as '{typeList[0].F_Code}' ");
            for (var i = 1; i < typeList.Count; i++)
            {
                strSql.Append($", m{i}.F_Name as '{typeList[i].F_Code}' ");
            }
            strSql.Append(" FROM LR_Lg_Map t ");

            for (var j = 1; j < typeList.Count; j++)
            {
                strSql.Append($"LEFT JOIN LR_Lg_Map m{j} ON t.F_Code = m{j}.F_Code AND m{j}.F_TypeCode='{typeList[j].F_Code}' AND  m{j}" + ".{JCODE_SASSID_NOTA} ");
            }
            strSql.Append($" WHERE t.F_TypeCode = '{typeList[0].F_Code}' ");
            strSql.Append(" {JCODE_SASSID}");
            strSql.Append(" AND t.F_Name in (@nameList)");
            
            return this.BaseRepository().FindTable(strSql.ToString(), new { nameList = nameList.ToArray() });*/

            var typeList = (List<LGTypeEntity>)types;
            DataTable objDataTable = new DataTable("Table");
            objDataTable.Columns.Add("f_code", typeof(string));
            int intFieldCount = typeList.Count;
            for (int intCounter = 0; intCounter < intFieldCount; ++intCounter)
            {
                objDataTable.Columns.Add(typeList[intCounter].F_Code.ToLower(), typeof(string));
            }


            var list = (List<LGMapEntity>)await this.BaseRepository().FindAll<LGMapEntity>();
            var mainList = list.FindAll(t => t.F_TypeCode == typeList[0].F_Code && nameList.Contains(t.F_Name));
            var otherList = list.FindAll(t => t.F_TypeCode != typeList[0].F_Code);
            if (mainList.Count == 0) {
                return objDataTable;
            }

            Dictionary<string, string> dic = new Dictionary<string, string>();
            foreach (var item in otherList)
            {
                var key = $"{item.F_Code}_{item.F_TypeCode}";
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, item.F_Name);
                }
            }

            for (var i = 0; i < mainList.Count; i++)
            {
                var main = mainList[i];
                DataRow row = objDataTable.NewRow();
                row["f_code"] = main.F_Code;
                foreach (var lgType in typeList)
                {
                    if (lgType.F_Code == typeList[0].F_Code)
                    {
                        row[lgType.F_Code.ToLower()] = main.F_Name;
                    }
                    else if (dic.ContainsKey($"{main.F_Code}_{lgType.F_Code}"))
                    {
                        row[lgType.F_Code.ToLower()] = dic[$"{main.F_Code}_{lgType.F_Code}"];
                    }
                }
                objDataTable.Rows.Add(row);
            }
            return objDataTable;
        }

        

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        public async Task DeleteEntity(string code)
        {
            await this.BaseRepository().Delete<LGMapEntity>(t=>t.F_Code == code);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="list">翻译集合</param>
        /// <param name="code">一组语言编码</param>
        /// <returns></returns>
        public async Task SaveEntity(List<LGMapEntity> list, string code)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                var mainLg = list.Find(t => t.F_TypeCode == "chinese");

                if (string.IsNullOrEmpty(code)) {
                    var main = await db.FindEntity<LGMapEntity>(t => t.F_Name == mainLg.F_Name && t.F_TypeCode == mainLg.F_TypeCode);
                    if (main != null)
                    {
                        // 删除翻译一样的数据
                        code = main.F_Code;
                    }
                }
                else
                {
                    var old = await db.FindEntity<LGMapEntity>(t => t.F_Name == mainLg.F_Name && t.F_TypeCode == mainLg.F_TypeCode && code != t.F_Code);
                    if (old != null)
                    {
                        throw new Exception("#NotLog#翻译项重复【" +old.F_Name+"】" );
                    }
                }



                if (!string.IsNullOrEmpty(code))
                {
                    await db.Delete<LGMapEntity>(t => t.F_Code == code);
                }
                else
                {
                    code = Guid.NewGuid().ToString();
                }

                foreach (var item in list) {
                    item.F_Code = code;
                    if (item.F_TypeCode == "traditionalchinese" && string.IsNullOrEmpty(item.F_Name)) {
                        // 如果是繁体就自动翻译
                        item.F_Name = Str.GetTraditional(mainLg.F_Name);
                    }

                    await db.Insert(item);
                }

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }

        #endregion

    }
}
