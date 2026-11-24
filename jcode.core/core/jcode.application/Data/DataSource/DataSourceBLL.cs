using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Jint;
using SqlSugar.TDengine;

namespace jcode.application
{
  /// <summary>
  /// 版 本 极简搭AI应用开发基座
  /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
  /// 创建人：tobin
  /// 日 期：2019.09.19
  /// 描 述：数据源
  /// </summary>
  public class DataSourceBLL : BLLBase, DataSourceIBLL, BLL
  {
    private readonly DataSourceService dataSourceService = new DataSourceService();

    #region 获取数据
    /// <summary>
    /// 获取分页数据
    /// </summary>
    /// <param name="pagination">分页参数</param>
    /// <param name="keyword">关键字</param>
    /// <param name="category">分类</param>
    /// <param name="appId">所属应用主键</param>
    /// <returns></returns>
    public Task<IEnumerable<DataSourceEntity>> GetPageList(Pagination pagination, string keyword, string category, string appId = null)
    {
      return dataSourceService.GetPageList(pagination, keyword, category, appId);
    }
    /// <summary>
    /// 获取列表数据
    /// </summary>
    /// <returns></returns>
    public Task<IEnumerable<DataSourceEntity>> GetList()
    {
      return dataSourceService.GetList();
    }
    /// <summary>
    /// 获取实体
    /// </summary>
    /// <param name="code">编号</param>
    /// <returns></returns>
    public Task<DataSourceEntity> GetEntityByCode(string code)
    {
      return dataSourceService.GetEntityByCode(code);
    }
    #endregion

    #region 提交数据
    /// <summary>
    /// 删除数据源
    /// </summary>
    /// <param name="keyValue">主键</param>
    public async Task DeleteEntity(string keyValue)
    {
      await dataSourceService.DeleteEntity(keyValue);
    }
    /// <summary>
    /// 保存（新增、修改）
    /// </summary>
    /// <param name="keyValue">主键值</param>
    /// <param name="dataSourceEntity">数据源实体</param>
    /// <returns></returns>
    public Task<bool> SaveEntity(string keyValue, DataSourceEntity dataSourceEntity)
    {
      if (!string.IsNullOrEmpty(dataSourceEntity.F_Sql))
      {
        dataSourceEntity.F_Sql = AESHelper.AesDecrypt(dataSourceEntity.F_Sql, "jcode");//"12345678901234567890123456789012"
      }
      dataSourceEntity.F_Sql = this.SqlFiltersNotSelect(dataSourceEntity.F_Sql);
      return dataSourceService.SaveEntity(keyValue, dataSourceEntity);
    }
    #endregion

    #region 扩展方法
    /// <summary>
    /// 获取数据源的数据
    /// </summary>
    /// <param name="code">数据源编码</param>
    /// <param name="sidx">排序字段</param>
    /// <param name="paramsJson">查询条件集合</param>
    /// <returns></returns>
    public async Task<DataTable> GetDataTable(string code,string sidx, string paramsJson = "")
    {
      var userInfo = await this.CurrentUser();
      string whereSql = string.Empty;
      DataSourceEntity entity = await GetEntityByCode(code);
      if (userInfo.F_SecurityLevel != 1)
      {
        whereSql = await this.GetDataAuthoritySql(entity.F_Id);
      }
      DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
      DataSourceRes dbSource = await GetViewRes(model, paramsJson, userInfo);
      if (!string.IsNullOrEmpty(sidx))
      {
        dbSource.Order = sidx;
      }
      return await dataSourceService.GetDataViews(dbSource, whereSql);
    }
    
    /// <summary>
    /// 获取数据源的数据(分页)
    /// </summary>
    /// <param name="code">数据源编码</param>
    /// <param name="pagination">分页参数</param>
    /// <param name="paramsJson">查询条件</param>
    /// <param name="field">字段</param>
    /// <param name="values">值</param>
    /// <param name="whereSql">条件语句</param>
    /// <returns></returns>
    public async Task<DataTable> GetDataTable(string code, Pagination pagination, string paramsJson,string field, string values, string whereSql = null)
    {
      var userInfo = await this.CurrentUser();
      whereSql = this.SqlFilters(whereSql);

      if (!string.IsNullOrEmpty(field) && !string.IsNullOrEmpty(values))
      {
        string sql = $" {field} in ('{values.Replace(",", "','")}') ";
        if (string.IsNullOrEmpty(whereSql))
        {
          whereSql = sql;
        }
        else
        {
          whereSql += " AND " + sql;
        }
      }

     



      DataSourceEntity entity = await GetEntityByCode(code);
      if (userInfo.F_SecurityLevel != 1)
      {
        var authWhereSql = await this.GetDataAuthoritySql(entity.F_Id);
        if (string.IsNullOrEmpty(whereSql))
        {
          whereSql = authWhereSql;
        }
        else if (!string.IsNullOrEmpty(authWhereSql))
        {
          whereSql = $"({whereSql}) AND ({authWhereSql})";
        }
      }
      DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
      DataSourceRes dbSource = await GetViewRes(model, paramsJson, userInfo);
      return await dataSourceService.GetDataViewPage(dbSource, pagination, whereSql);
    }

    /// <summary>
    /// 获取数据源的数据(分页)，预览查看数据，不做数据权限过滤
    /// </summary>
    /// <param name="entity">数据源编码</param>
    /// <param name="pagination">分页参数</param>
    /// <param name="paramsJson">查询条件</param>
    /// <param name="whereSql">条件语句</param>
    /// <returns></returns>
    public async Task<DataTable> GetDataTable(DataSourceEntity entity, Pagination pagination, string paramsJson, string whereSql = null)
    {
      var userInfo = await this.CurrentUser();
      whereSql = this.SqlFilters(whereSql);
      entity.F_Sql = AESHelper.AesDecrypt(entity.F_Sql, "jcode");

      DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
      DataSourceRes dbSource = await GetViewRes(model, paramsJson, userInfo);
      return await dataSourceService.GetDataViewPage(dbSource, pagination, whereSql);
    }
    
    /// <summary>
    /// 获取数据源的数据（根据指定字段和值）
    /// </summary>
    /// <param name="code">数据源编码</param>
    /// <param name="field">字段</param>
    /// <param name="values">值</param>
    /// <param name="paramsJson">值</param>
    /// <returns></returns>
    public async Task<DataTable> GetDataTableByValues(string code, string field, string values,string paramsJson = "")
    {
      var userInfo = await this.CurrentUser();
      DataSourceEntity entity = await GetEntityByCode(code);
      string sql = $" {field} in ('{values.Replace(",", "','")}') ";

      DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
      DataSourceRes dbSource = await GetViewRes(model, paramsJson, userInfo);
      return await dataSourceService.GetDataViews(dbSource, sql);
    }
    
    /// <summary>
    /// 获取sql的列
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetDataColName(string code)
    {
      return dataSourceService.GetDataColName(code);
    }

    /// <summary>
    /// 获取sql的列
    /// </summary>
    /// <param name="dbCode">数据库编码</param>
    /// <param name="sql">sql语句</param>
    /// <returns></returns>
    public Task<IEnumerable<string>> GetDataColName(string dbCode, string sql)
    {
      return dataSourceService.GetDataColName(dbCode, AESHelper.AesDecrypt(sql, "jcode"));
    }

    /// <summary>
    /// 获取视图列信息
    /// </summary>
    /// <param name="code">编码</param>
    /// <returns></returns>
    public async Task<List<DataSourceColumns>> GetViewColumns(string code)
    {
      DataSourceEntity entity = await GetEntityByCode(code);
      if (entity == null)
      {
        return new List<DataSourceColumns>();
      }
      DataSourceModel model = entity.F_Sql.ToObject<DataSourceModel>();
      return model.Columns;
    }



    #endregion

    #region 新版
    /// <summary>
    /// 获取列信息
    /// </summary>
    /// <param name="dt">数据集合</param>
    /// <returns></returns>
    public List<DataSourceColumns> GetDataColumnsInfo(DataTable dt)
    {
      var res = new List<DataSourceColumns>();
      foreach (DataColumn col in dt.Columns)
      {
        res.Add(new DataSourceColumns()
        {
          Prop = col.ColumnName,
          Label = col.ColumnName,
          CsType = col.DataType.Name
        });
      }

      return res;
    }
    /// <summary>
    /// 获取数据视图
    /// </summary>
    /// <param name="model">配置信息</param>
    /// <param name="paramsObject">参数信息</param>
    /// <param name="userInfo">用户信息</param>
    /// <returns></returns>
    private async Task<DataSourceRes> GetViewRes(DataSourceModel model, string paramsObject, UserEntity userInfo, string param = "")
    {
      var res = new DataSourceRes();
      res.ParamsDic = GetParams(model, paramsObject, userInfo, param);
      res.Order = GetSqlByOrder(model);

      var configSql = model.Sqls[0];
      res.DbCode = configSql.DbCode;
      switch (model.Type)
      {
        case "table": // 执行表数据
          res.Sql = $"select * from {configSql.DbTable} where " + " {JCODE_SASSID_NOTA} ";
          break;
        case "code":
          // 测试后端执行js方法
          res.Data = await JsDataView(model, res.ParamsDic);
          break;
        default:
          if (model.Sqls.Count > 1)
          {
            await AddChildrenSqlParams(model, res.ParamsDic);
          }
          res.Sql = configSql.Sql;
          
          
          // 对参数做动态处理
          string pattern = @"\[\[\s*([^\]]+)\s*\]\]";
          string pattern2 = @"@([^\s]+)";
          Regex regex = new Regex(pattern);
          Regex regex2 = new Regex(pattern2);
          var matches = regex.Matches(res.Sql);

          foreach (Match match in matches)
          {
            string whereContent = match.Value;
            var matches2 = regex2.Matches(whereContent);
            if (matches2.Count > 0)
            {
                var name = matches2[0].Value.Replace("@", "").Replace("]]", "").Replace(",", "");


                if (!res.ParamsDic.ContainsKey(name))
                {
                    res.Sql = res.Sql.Replace(whereContent, " 1=1 ");
                }
                else {
                    res.Sql = res.Sql.Replace(whereContent, whereContent.Replace("[[", "").Replace("]]", ""));
                }
            }
          }
          
          //重新构建sql语句-当参数为空时查询全部信息
          if (!res.ParamsDic.ContainsKey("param"))//存在参数
          {
            res.Sql = res.Sql.Replace("= @param", " is not null");
            res.Sql = res.Sql.Replace("=@param", " is not null");
            res.Sql = res.Sql.Replace("@param", "''");
          }
          
          break;
      }
      
      


      return res;
    }



    /// <summary>
    /// 执行js脚本
    /// </summary>
    /// <param name="model">设置数据</param>
    /// <param name="paramsDic">参数信息</param>
    /// <returns></returns>
    private async Task<DataTable> JsDataView(DataSourceModel model, Dictionary<string, object> paramsDic)
    {
      string code = model.Sqls[0].Sql;
      Dictionary<string, DataTable> paramsData = new Dictionary<string, DataTable>();
      foreach (var dataSourceInput in model.DataSourceInputs)
      {
        // 参数传递
        Dictionary<string, object> selectParamsDic = new Dictionary<string, object>();
        foreach (var key in paramsDic.Keys)
        {
          if (key.StartsWith($"{dataSourceInput.Code}_"))
          {
            selectParamsDic.Add(key,paramsDic[key]);
          }
        }
        var data = await GetDataTable(dataSourceInput.Code,"",selectParamsDic.ToJson());
        paramsData.Add(dataSourceInput.Code, data);
      }
      paramsDic.Add("data", paramsData);

      

      var jsonData = paramsDic.ToJson();

      var engine = new Engine(options =>
      {
        // options.AllowClr(typeof(DbService).Assembly);
      });
      
      // IJsEngine engine = JsEngineSwitcher.Current.CreateDefaultEngine(); //new JurassicJsEngine();
      engine.SetValue("db",new DbService());
      engine.SetValue("inputStrParams", jsonData);
      string jsCode = "function jcodeFindTable(sql,sqlParams){let strSqlParams = '';if(sqlParams){strSqlParams = JSON.stringify(sqlParams);}  let dbRes = db.FindTable(sql,strSqlParams); return JSON.parse(dbRes); };" +
                      "function fn(strJcode) { let jcode = JSON.parse(strJcode);  " + 
                      " jcode.findTable = jcodeFindTable; "+
                      code + " }; " +
                      "let resStr;function fnStr(strJcode){ let res = fn(strJcode);" +
                      "resStr = JSON.stringify(res); };" +
                      "fnStr(inputStrParams);";
      engine.Execute(jsCode);
      var result = engine.Evaluate("resStr").ToObject()?.ToString();

      //var fnStr = engine.GetValue("fnStr");
      //var result = engine.Invoke(fnStr,jsonData).ToObject();
      if (!string.IsNullOrEmpty(result))
      {
        return result.ToObject<DataTable>();
      }

      return new DataTable();
    }


    /// <summary>
    /// 获取子查询条件参数（跨库才需要）
    /// </summary>
    /// <param name="model"></param>
    /// <param name="paramsDic"></param>
    /// <returns></returns>
    private async Task AddChildrenSqlParams(DataSourceModel model, Dictionary<string, object> paramsDic)
    {
      // 先进行子查询查询，用来作为参数给父级查询
      var childSqls = model.Sqls.FindAll(t => t.Type == "2");
      foreach (var itemSql in childSqls)
      {
        var dt = await dataSourceService.BaseRepository(itemSql.DbCode).FindTable(itemSql.Sql, paramsDic);
        List<string> childParams = new List<string>();
        foreach (DataRow dr in dt.Rows)
        {
          childParams.Add(dr[0].ToString());
        }

        if (!paramsDic.ContainsKey(itemSql.ParamName))
        {
          paramsDic.Add(itemSql.ParamName, childParams.ToArray());
        }
      }
    }


    /// <summary>
    /// 给sql添加排序语句
    /// </summary>
    /// <param name="model"></param>
    /// <returns></returns>
    private string GetSqlByOrder(DataSourceModel model)
    {
      string res = "";
      if (model.Orders != null)
      {
        foreach (var item in model.Orders)
        {
          if (!string.IsNullOrEmpty(res))
          {
            res += ",";
          }

          res += item.Field;
          if (item.IsDESC)
          {
            res += " DESC";
          }
        }
      }

      return res;
    }

    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="model">设置模型</param>
    /// <param name="paramsObject">参数对象</param>
    /// <param name="userInfo">用户信息</param>
    /// <returns></returns>
    private Dictionary<string, object> GetParams(DataSourceModel model, string paramsObject, UserEntity userInfo, string param)
    {
      Dictionary<string, object> paramsDic = new Dictionary<string, object>();
      if (!string.IsNullOrEmpty(paramsObject))
      {
        paramsDic = paramsObject.ToObject<Dictionary<string, object>>();
      }
      // 获取参数
      if (model.QueryFields != null)
      {
        foreach (var item in model.QueryFields)
        {
            if (!paramsDic.ContainsKey(item.Name))
            {
                if (!string.IsNullOrEmpty(item.Default))
                {
                    paramsDic.Add(item.Name, GetDbParameter(item.Default, item.Type));
                }
            }
            else if (!paramsDic[item.Name].ObjToStringNoTrim().IsEmpty())
            {
                paramsDic[item.Name] = GetDbParameter(paramsDic[item.Name].ToString(), item.Type);
            }
            /*else {
                paramsDic.Remove(item.Name);
            }*/
        }
      }
      
      if (!paramsDic.ContainsKey("param") && !string.IsNullOrEmpty(param)) // 
      {
        paramsDic.Add("param", param);
      }

      if (!paramsDic.ContainsKey("userId"))
      {
        paramsDic.Add("userId", userInfo.F_UserId);
      }

      if (!paramsDic.ContainsKey("userAccount"))
      {
        paramsDic.Add("userAccount", userInfo.F_Account);
      }

      if (!paramsDic.ContainsKey("companyId"))
      {
        paramsDic.Add("companyId", userInfo.F_CompanyId);
      }

      if (!paramsDic.ContainsKey("departmentId"))
      {
        paramsDic.Add("departmentId", userInfo.F_DepartmentId);
      }

      return paramsDic;
    }
    private object GetDbParameter(string value, string csType)
    {
      string defualtValue = "";
      string _csType = "";
      switch (csType)
      {
        case "int":
          _csType = "System.Int32";
          defualtValue = "0";
          break;
        case "DateTime":
          _csType = "System.DateTime";
          defualtValue = "1000-01-01 01:00:00";
          break;
        case "like":
          _csType = "";
          if (string.IsNullOrEmpty(value))
          {
            value = "%%";
          }
          else
          {
            value = $"%{value}%";
          }
          break;
      }
      
      if (string.IsNullOrEmpty(value))
      {
        value = defualtValue;
      }

      return string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value, System.Type.GetType(_csType));
    }
    #endregion
    
    #region 数据转化
    /// <summary>
    /// 将datatable里的数据转换成显示值通过数据源
    /// </summary>
    /// <param name="dt">数据表</param>
    /// <param name="cols">需要转化的列</param>
    /// <returns></returns>
    public async Task<DataTable> ConvertDataTable(DataTable dt, List<DataSourceCol> cols)
    {
      Dictionary<string, Dictionary<string, string>> dic = new Dictionary<string, Dictionary<string, string>>();
      foreach (var col in cols)
      {
        var dbSource = await GetDataTable(col.Code,"");
        Dictionary<string, string> valueToLabel = new Dictionary<string, string>();
        foreach (DataRow row in dbSource.Rows)
        {
          if (!valueToLabel.ContainsKey(row[col.Valuekey].ToString()))
          {
            valueToLabel.Add(row[col.Valuekey].ToString(), row[col.LabelKey].ToString());
          }
        }
        dic.Add(col.Code, valueToLabel);
        dt.Columns.Add($"{col.Prop}_dataSource", typeof(string));
      }

      foreach (DataRow row in dt.Rows)
      {
        foreach (var col in cols)
        {
          if (dic[col.Code].ContainsKey(row[col.Prop].ToString()))
          {
            row[$"{col.Prop}_dataSource"] = dic[col.Code][row[col.Prop].ToString()];
          }
          else
          {
            row[$"{col.Prop}_dataSource"] = row[col.Prop].ToString();
          }
        }
      }

      foreach (var col in cols)
      {
        dt.Columns.Remove(dt.Columns[col.Prop]);
        dt.Columns[$"{col.Prop}_dataSource"].ColumnName = col.Prop;
      }

      return dt;
    }
    #endregion
    

  }
}
