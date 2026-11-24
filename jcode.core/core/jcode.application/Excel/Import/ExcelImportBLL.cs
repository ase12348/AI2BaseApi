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
    /// 描 述：Excel数据导入设置
    /// </summary>
    public class ExcelImportBLL : BLLBase, ExcelImportIBLL,BLL
    {
        private readonly ExcelImportService excelImportService = new ExcelImportService();
        private readonly CodeTableIBLL _codeTableIBLL;
        private readonly DataItemIBLL _dataItemIBLL;
        private readonly DataSourceIBLL _dataSourceIBLL;
        private readonly CodeRuleIBLL _codeRuleIBLL;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="codeTableIBLL"></param>
        /// <param name="dataItemIBLL"></param>
        /// <param name="dataSourceIBLL"></param>
        public ExcelImportBLL(CodeTableIBLL codeTableIBLL, DataItemIBLL dataItemIBLL, DataSourceIBLL dataSourceIBLL,CodeRuleIBLL codeRuleIBLL) {
            _codeTableIBLL = codeTableIBLL;
            _dataItemIBLL = dataItemIBLL;
            _dataSourceIBLL = dataSourceIBLL;
            _codeRuleIBLL = codeRuleIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetPageList(Pagination pagination, string keyword, string moduleId)
        {
            return excelImportService.GetPageList(pagination, keyword, moduleId);
        }
        /// <summary>
        /// 获取导入配置列表根据模块ID
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetList(string moduleId)
        {
            return excelImportService.GetList(moduleId);
        }
        /// <summary>
        /// 获取所有导入配置信息
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportEntity>> GetList() {
            return excelImportService.GetList();
        }
        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<ExcelImportEntity> GetEntity(string keyValue)
        {
            return excelImportService.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<ExcelImportEntity> GetEntityByCode(string code)
        {
            return excelImportService.GetEntityByCode(code);
        }


        /// <summary>
        /// 获取配置字段列表
        /// </summary>
        /// <param name="importId">配置信息主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ExcelImportFieldEntity>> GetFieldList(string importId)
        {
            return excelImportService.GetFieldList(importId);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteEntity(string keyValue)
        {
            await excelImportService.DeleteEntity(keyValue);
        }
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体数据</param>
        /// <param name="filedList">字段列表</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, ExcelImportEntity entity, List<ExcelImportFieldEntity> filedList)
        {
            await excelImportService.SaveEntity(keyValue, entity, filedList);
        }
        /// <summary>
        /// 更新配置主表
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        public async Task UpdateEntity(string keyValue, ExcelImportEntity entity)
        {
            await excelImportService.UpdateEntity(keyValue, entity);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// excel 数据导入（未导入数据写入缓存）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="list"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<(DataTable elist, int snum, int fnum)> ImportTable(ExcelImportEntity entity, List<ExcelImportFieldEntity> list, DataTable dt)
        {
            int snum = 0;
            int fnum = 0;
            // 创建一个datatable容器用于保存导入失败的数据
            DataTable failDt = new DataTable();
            dt.Columns.Add("导入错误", typeof(string));
            foreach (DataColumn dc in dt.Columns)
            {
                failDt.Columns.Add(dc.ColumnName, dc.DataType);
            }

            if (dt.Rows.Count > 0)
            {

                string sqlonly = " select * from " + entity.F_DbTable + " where 1=1 {JCODE_SASSID} ";
                // 数据字典数据
                Dictionary<string, List<DataItemDetailEntity>> dataItemMap = new Dictionary<string, List<DataItemDetailEntity>>();
                Dictionary<string, DataTable> dataSourceMap = new Dictionary<string, DataTable>();

                foreach (DataRow dr in dt.Rows) { // 行数据循环
                    try
                    {
                        List<SugarParameter> dbParameters = new List<SugarParameter>();
                        foreach (var col in list) {
                            object paramValue = null;

                            switch (col.F_RelationType)
                            {
                                case 0://无关联
                                    paramValue = dr[col.F_ColName].ToString();
                                    await IsOnlyOne(col, sqlonly, dr[col.F_ColName].ToString(), entity.F_DbId);
                                    break;
                                case 1://GUID
                                    paramValue = Guid.NewGuid().ToString();
                                    break;
                                case 2://数据字典
                                    string dataItemName = "";
                                    if (!dataItemMap.ContainsKey(col.F_DataItemCode))
                                    {
                                        List<DataItemDetailEntity> dataItemList = (List<DataItemDetailEntity>)await _dataItemIBLL.GetDetailList(col.F_DataItemCode);
                                        dataItemMap.Add(col.F_DataItemCode, dataItemList);
                                    }
                                    dataItemName = FindDataItemValue(dataItemMap[col.F_DataItemCode], dr[col.F_ColName].ToString(), col.F_ColName);
                                    paramValue = dataItemName;
                                    await IsOnlyOne(col, sqlonly, dataItemName, entity.F_DbId);
                                    break;
                                case 3://数据表
                                    string v = "";
                                    try
                                    {
                                        //string queryJson = "{" + col.F_DSourceExcelId + ":\"" + dr[col.F_ColName].ToString() + "\"}";
                                        if (!dataSourceMap.ContainsKey(col.F_DSourceId)) {
                                            DataTable sourceDt = await _dataSourceIBLL.GetDataTable(col.F_DSourceId,"");
                                            dataSourceMap.Add(col.F_DSourceId, sourceDt);
                                        }
                                        v = FindDataSourceValue(dataSourceMap[col.F_DSourceId], col.F_DSourceExcelId.ToLower(), dr[col.F_ColName].ToString(),col.F_DSourceDBId.ToLower());
                                        paramValue = v;
                                    }
                                    catch (Exception)
                                    {
                                        throw (new Exception("【" + col.F_ColName + "】 找不到对应的数据"));
                                    }
                                    await IsOnlyOne(col, sqlonly, v, entity.F_DbId);
                                    break;
                                case 4://固定值
                                    paramValue = col.F_Value;
                                    break;
                                case 5://操作人ID
                                    paramValue = this.GetUserId();
                                    break;
                                case 6://操作人名字
                                    paramValue = this.GetUserName();
                                    break;
                                case 7://操作时间
                                    paramValue = DateTime.Now;
                                    break;
                                case 8://单据编码
                                    paramValue =await _codeRuleIBLL.GetBillCode(col.F_EnCode,"");
                                    break;
                            }
                            
                            dbParameters.Add(GetMyDbParameter(col.F_Name,paramValue.ToString(),col.F_CsType));
                        }
                        
                        await excelImportService.BaseRepository(entity.F_DbId).ExecuteInsert(entity.F_DbTable, dbParameters);
                        snum++;
                    }
                    catch (Exception ex)
                    {
                        fnum++;
                        if (entity.F_ErrorType == 0)// 如果错误机制是终止
                        {
                            dr["导入错误"] = $"{ex.Message} 【之后数据未被导入】";
                            failDt.Rows.Add(dr.ItemArray);
                            break;
                        }
                        else
                        {
                            dr["导入错误"] = ex.Message;
                            failDt.Rows.Add(dr.ItemArray);
                        }
                    }
                }
            }
            return (failDt, snum, fnum);
        }
        /// <summary>
        /// excel 数据导入（未导入数据写入缓存）
        /// </summary>
        /// <param name="code">导入模板编码</param>
        /// <param name="dt">导入数据</param>
        /// <returns></returns>
        public async Task<(DataTable elist, int snum, int fnum)> ImportTable(string code, DataTable dt)
        {
            ExcelImportEntity entity = await GetEntityByCode(code);
            List<ExcelImportFieldEntity> list = (List<ExcelImportFieldEntity>)await GetFieldList(entity.F_Id);
            var res = await ImportTable(entity,list,dt);
            return res;

        }

        /// <summary>
        /// 数据字典查找Value
        /// </summary>
        /// <param name="dataItemList">数据字典数据</param>
        /// <param name="itemName">项目名</param>
        /// <param name="colName">列名</param>
        /// <returns></returns>
        private string FindDataItemValue(List<DataItemDetailEntity> dataItemList, string itemName, string colName)
        {
            if (!string.IsNullOrEmpty(itemName)) {
                string label = "";
                string[] values = itemName.Split(",");
                foreach (var v in values) {
                    DataItemDetailEntity dataItem = dataItemList.Find(t => t.F_ItemName == v);
                    if (dataItem != null)
                    {
                        if (!string.IsNullOrEmpty(label)) {
                            label += ",";
                        }
                        label += dataItem.F_ItemValue;
                    }
                    else
                    {
                        throw (new Exception("【" + colName + "】数据字典找不到对应值"));
                    }
                }
                return label;
            }
            return "";
        }


        /// <summary>
        /// 获取数据源数据
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="name">名称</param>
        /// <param name="value">值</param>
        /// <param name="colname">列名</param>
        /// <returns></returns>
        private string FindDataSourceValue(DataTable dt, string name, string value,string colname)
        {
            // 需要考虑兼容多选的情况，用“，”分离
            if (!string.IsNullOrEmpty(value)) {
                string label = "";
                string[] values = value.Split(",");
                foreach (var v in values) {
                    foreach (DataRow dr in dt.Rows)
                    {
                        if (dr[name].ToString() == value)
                        {
                            if (!string.IsNullOrEmpty(label)) {
                                label += ",";
                            }
                            label += dr[colname].ToString();
                        }
                    }
                }
                return label;
            }
            return "";
        }

        /// <summary>
        /// 判断是否数据有重复
        /// </summary>
        /// <param name="col"></param>
        /// <param name="sqlonly"></param>
        /// <param name="value"></param>
        /// <param name="dbId"></param>
        private async Task IsOnlyOne(ExcelImportFieldEntity col, string sqlonly, string value, string dbId)
        {
            if (col.F_OnlyOne == 1)
            {
                var dp =new Dictionary<string,string>();
                sqlonly += "AND "+col.F_Name + " = @" + col.F_Name;
                dp.Add(col.F_Name, value);
                var d = await excelImportService.BaseRepository(dbId).FindTable(sqlonly, dp);
                if (d.Rows.Count > 0)
                {
                    throw new Exception("【" + col.F_ColName + "】此项数据不能重复");
                }
            }
        }
        
        
        private static SugarParameter GetMyDbParameter(string parameterName, string value, string csType)
        {
            string _csType = csType;
            switch (csType)
            {
                case "int":
                    _csType = "System.Int32";
                    break;
                case "long":
                    _csType = "System.Int64";
                    break;
                case "short":
                    _csType = "System.Int16";
                    break;
                case "bool":
                    _csType = "System.Boolean";
                    break;
                case "float":
                    _csType = "System.Single";
                    break;
                case "text":
                    _csType = "";
                    break;
                default:
                    if (!string.IsNullOrEmpty(csType))
                    {
                        _csType = $"System.{Str.FirstUpper(csType)}";
                    }
                    break;
            }

            if (string.IsNullOrEmpty(value))
            {
                return new SugarParameter(parameterName, null);
            }

            if (_csType == "System.TimeSpan")
            {
                return new SugarParameter(parameterName, TimeSpan.Parse(value));
            }
            else if (_csType == "System.Guid")
            {
                return new SugarParameter(parameterName, Guid.Parse(value));
            }
            else
            {
                return new SugarParameter(parameterName, string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value, System.Type.GetType(_csType)));
            }
        }
        #endregion
    }
}
