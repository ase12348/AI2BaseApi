using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;
using SqlSugar;

namespace jcode.iapplication;

/// <summary>
/// 公式计算
/// </summary>
public class FormulaHelper
{
    /// <summary>
    /// 获取sql语句
    /// </summary>
    /// <param name="conditions">条件</param>
    /// <param name="formula">公式</param>
    /// <returns></returns>
    public static async Task<string> GetSql(List<Condition> conditions,string formula)
    {
        string strSql = "";
        if (!string.IsNullOrEmpty(formula))
        {
            strSql = formula;
            var ccount = conditions.Count;
            for (int i = ccount; i > 0; i--)
            {
                var conditionItem = conditions[i - 1];
                conditionItem.Index = i;
                strSql = strSql.Replace("{@jcode" + i , "XXX");
                strSql = strSql.Replace( i + "jcode@}", "YYY");
                strSql = strSql.Replace("" + i, "{@jcode" + i + "jcode@}");
                strSql = strSql.Replace("XXX"  , "{@jcode" + i);
                strSql = strSql.Replace("YYY"  , i + "jcode@}" );
            }
        }
        else
        {
            // 默认公式
            for (int i = 1; i < conditions.Count + 1; i++)
            {
                var conditionItem = conditions[i - 1];
                conditionItem.Index = i;
                if (strSql != "")
                {
                    strSql += " AND ";
                }
                strSql += " {@jcode" + i + "jcode@} ";
            }
        }
        int num = 1;
        foreach (var conditionItem in conditions)
        {
      
            var strone = new StringBuilder();
            if (conditionItem.Symbol == 8)
            {
                strone.Append($" ( {conditionItem.FieldId}");
            }
            else
            {
                strone.Append($" {conditionItem.FieldId}");
            }


            string value = await GetValue(conditionItem.FiledValueType, conditionItem.FiledValue);

            switch (conditionItem.Symbol)
            {
                case 1:// 等于
                    strone.Append($" = {value}");
                    break;
                case 2:// 大于
                    strone.Append($" > {value}");
                    break;
                case 3:// 大于等于
                    strone.Append($" >= {value}");
                    break;
                case 4:// 小于
                    strone.Append($" < {value}");
                    break;
                case 5:// 小于等于
                    strone.Append($" <= {value}");
                    break;
                case 6:// 包含
                    value = value.Replace("'", "");
                    strone.Append($" like '%{value}%'");
                    break;
                case 7:// 包含于
                    value = value.Replace(",", "','");
                    strone.Append($" in ({value})");
                    break;
                case 8:// 不等于
                    strone.Append($" != {value}");
                    strone.Append($" or { conditionItem.FieldId} is null ) ");
                    break;
                case 9:// 不包含
                    value = value.Replace("'", "");
                    strone.Append($" not like '%{value}%'");
                    break;
                case 10:// 不包含于
                    value = value.Replace(",", "','");
                    strone.Append($" not in ({value})");
                    break;
                default:
                    break;
            }
            strone.Append(" ");
            strSql = strSql.Replace("{@jcode" + num + "jcode@}", strone.ToString());
            num++;
        }
        return strSql;
    } 
    
    /// <summary>
    /// 获取参数
    /// </summary>
    /// <param name="parameterName"></param>
    /// <param name="value"></param>
    /// <param name="csType"></param>
    /// <returns></returns>
    public static SugarParameter GetDbParameter(string parameterName, string value, string csType)
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
        if (_csType == "System.TimeSpan")
        {
            return new SugarParameter(parameterName, TimeSpan.Parse(value));
        }
        
        if (_csType == "System.Guid")
        {
            if (string.IsNullOrEmpty(value))
            {
                return new SugarParameter(parameterName, null);
            }

            return new SugarParameter(parameterName, Guid.Parse(value));
        }
        
        return new SugarParameter(parameterName, string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value, System.Type.GetType(_csType)));
    }
    
    /// <summary>
    /// 获取数据
    /// </summary>
    /// <param name="type">数据类型</param>
    /// <param name="value">数据值</param>
    /// <returns></returns>
    private static async Task<string> GetValue(int? type, string value)
    {
        var userBLL = IocManager.Instance.GetService<UserIBLL>();
        var _companyIBLL = IocManager.Instance.GetService<CompanyIBLL>();
        var _userRelationIBLL = IocManager.Instance.GetService<UserRelationIBLL>();
        var _departmentIBLL = IocManager.Instance.GetService<DepartmentIBLL>();
        
        //1.文本(字串)10.文本（数字）2.登录者ID3.登录者账号4.登录者公司41.登录者公司及下属公司42.固定公司5.登录者部门51.登录者部门及下属部门52.固定部门6.登录者岗位7.登录者角色
        string text;
        switch (type)
        {
            case 1:// 文本
                text = $"'{value}'";
                break;
            case 10:// 文本
                text = value;
                break;
            case 2: // 登录者ID
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{userInfo.F_UserId}'";
                }
               
                break;
            case 3: // 登录者账号
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{userInfo.F_Account}'";
                }
                break;
            case 4:// 登录者公司
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{userInfo.F_CompanyId}'";
                }
                break;
            case 41: // 登录者公司及下属公司
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{string.Join(",", await _companyIBLL.GetSubNodes(userInfo.F_CompanyId))}'";
                }
                break;
            case 42:// 固定公司
                text = $"'{value}'";
                break;
            case 5:// 登录者部门
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{userInfo.F_DepartmentId}'";
                }
                break;
            case 51: // 登录者部门及下属部门
                {
                    var userInfo = await userBLL.GetEntity();
                    var departmentList = new List<string>();
                    departmentList.AddRange(await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 3));
                    departmentList.AddRange(await _departmentIBLL.GetSubNodes(userInfo.F_DepartmentId));
                    text = $"'{string.Join(",", departmentList)}'";
                }
                break;
            case 52:// 固定部门
                text = $"'{value}'";
                break;
            case 6: // 登录者岗位
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{string.Join(",", await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 2))}'";
                }
                
                break;
            case 7: // 登录者角色
                {
                    var userInfo = await userBLL.GetEntity();
                    text = $"'{string.Join(",", await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 1))}'";
                }
                break;
            default:
                text = $"'{value}'";
                break;
        }
        return text;
    }
}