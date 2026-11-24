using System;
using System.Collections.Generic;
using jcode.orm;
using jcode.util;

namespace jcode.application;

public class DbService:RepositoryFactory
{
    public string FindTable(string sql,string strSqlParams)
    {
        var sqlParams = new Dictionary<string, object>();
        if (!string.IsNullOrEmpty(strSqlParams))
        {
            sqlParams = strSqlParams.ToObject<Dictionary<string, object>>();
        }

        var table = this.BaseRepository().FindTable(sql, sqlParams).GetAwaiter().GetResult();
        return table.ToJson();
    }
}