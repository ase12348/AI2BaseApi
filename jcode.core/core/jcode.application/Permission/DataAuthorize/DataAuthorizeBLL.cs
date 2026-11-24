using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.25
    /// 描 述：数据权限
    /// </summary>
    public class DataAuthorizeBLL : DataAuthorizeIBLL, BLL
    {
        private readonly DataAuthorizeService service = new DataAuthorizeService();

        private readonly UserIBLL _userIBLL;
        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly CompanyIBLL _companyIBLL;
        private readonly DepartmentIBLL _departmentIBLL;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="companyIBLL"></param>
        /// <param name="departmentIBLL"></param>
        public DataAuthorizeBLL(UserIBLL userIBLL, UserRelationIBLL userRelationIBLL, CompanyIBLL companyIBLL, DepartmentIBLL departmentIBLL)
        {
            _userIBLL = userIBLL;
            _userRelationIBLL = userRelationIBLL;
            _companyIBLL = companyIBLL;
            _departmentIBLL = departmentIBLL;
        }


        #region 获取数据
        /// <summary>
        /// 获取数据权限对应关系数据列表
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="objectIds">用户或角色主键</param>
        /// <returns></returns>
        public Task<IEnumerable<DataAuthorizeEntity>> GetList(string code, string[] objectIds)
        {
            return service.GetList(code, objectIds);
        }
        /// <summary>
        /// 获取数据权限列表（分页）
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="objectId">用户或角色主键</param>
        /// <param name="type">1.普通权限 2.自定义表单权限</param>
        /// <returns></returns>
        public Task<IEnumerable<DataAuthorizeEntity>> GetPageList(Pagination pagination, string keyword, string objectId, int? type)
        {
            return service.GetPageList(pagination, keyword, objectId, type);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DataAuthorizeEntity> GetEntity(string keyValue)
        {
            return service.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await service.DeleteEntity(keyValue);
        }
        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DataAuthorizeEntity entity)
        {
            await service.SaveEntity(keyValue, entity);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取数据权限设置sql
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="isForm">是否是自定义表单功能</param>
        /// <returns></returns>
        public async Task<string> GetWhereSql(string code,bool isForm = false)
        {
            var userInfo = await _userIBLL.GetEntity();
            if (userInfo.F_SecurityLevel == 1)
            {
                return "";
            }
            

            var numIndex = new string[26] {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N",
            "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };



            // 获取数据权限配置信息
            List<string> roleIds = await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 1);
            List<string> objectIds = new List<string>();
            objectIds.Add(userInfo.F_UserId);
            objectIds.AddRange(roleIds);
            var list = new List<DataAuthorizeEntity>();
            if (isForm)
            {
                list = (List<DataAuthorizeEntity>)await service.GetList(code, objectIds.ToArray(),  ContextHelper.GetItem("device").ToString() == "pc");
            }
            else
            {
                list = (List<DataAuthorizeEntity>)await service.GetList(code, objectIds.ToArray());
            }

            

            var strRes = new StringBuilder();
            int sqlNum = 0;
            foreach (var item in list) {

                if (sqlNum > 0)
                {
                    strRes.Append(" OR ( ");
                }
                else
                {
                    strRes.Append(" ( ");
                }

                string strSql = "";
                var itemFormula = item.F_Formula.ToObject<Formula>();
                if (!string.IsNullOrEmpty(itemFormula.formula))
                {
                    strSql = itemFormula.formula;
                    var ccount = itemFormula.Conditions.Count;
                    for (int i = ccount; i > 0; i--)
                    {
                        var conditionItem = itemFormula.Conditions[i - 1];
                        conditionItem.index = i;
                        strSql = strSql.Replace("{@jcode" + i , "XXX");
                        strSql = strSql.Replace( i + "jcode@}", "YYY");
                        strSql = strSql.Replace("" + i, "{@jcode" + i + "jcode@}");
                        strSql = strSql.Replace("XXX"  , "{@jcode" + i);
                        strSql = strSql.Replace("YYY"  , i + "jcode@}" );
                    }

                    if (itemFormula.Formulas != null && itemFormula.Formulas.Count > 0) {
                        for (int i = 0; i < itemFormula.Formulas.Count; i++)
                        {
                            strSql = strSql.Replace(numIndex[i], "{@jcode" + numIndex[i] + "jcode@}");
                        }
                    }
                }
                else
                {
                    // 默认公式
                    for (int i = 1; i < itemFormula.Conditions.Count + 1; i++)
                    {

                        var conditionItem = itemFormula.Conditions[i - 1];
                        conditionItem.index = i;

                        if (string.IsNullOrEmpty(conditionItem.Group)) {
                            if (strSql != "")
                            {
                                strSql += " AND ";
                            }
                            strSql += " {@jcode" + i + "jcode@} ";
                        }
                    }

                    if (itemFormula.Formulas != null && itemFormula.Formulas.Count > 0)
                    {
                        for (int i = 0; i < itemFormula.Formulas.Count; i++)
                        {

                            if (itemFormula.Conditions.FindIndex(t => t.Group == numIndex[i]) != -1) {
                                if (strSql != "")
                                {
                                    strSql += " AND ";
                                }
                                strSql += " {@jcode" + numIndex[i] + "jcode@} ";
                            }
                        }
                    }

                }


                // 分组公式处理
                if (itemFormula.Formulas != null && itemFormula.Formulas.Count > 0)
                {
                    

                    for (int i = 0; i < itemFormula.Formulas.Count; i++)
                    {
                        var groupSql = new StringBuilder();

                        var groupList = itemFormula.Conditions.FindAll(t => t.Group == numIndex[i]);
                        if (groupList.Count > 0) {

                            groupSql.Append($" { groupList[0].RelationField} in ( SELECT {groupList[0].Field} FROM {groupList[0].Table} WHERE ");

                            // 初始化分组公式
                            var groupSqlWhere = "";
                            if (string.IsNullOrEmpty(itemFormula.Formulas[i].Value))
                            {
                                foreach (var groupItem in groupList)
                                {
                                    if (groupSqlWhere != "")
                                    {
                                        groupSqlWhere += " AND ";
                                    }
                                    groupSqlWhere += " {@jcode" + groupItem.index + "jcode@} ";
                                }
                            }
                            else {
                                groupSqlWhere = itemFormula.Formulas[i].Value;
                                foreach (var groupItem in groupList) {
                                    groupSqlWhere = groupSqlWhere.Replace("" + groupItem.index, "{@jcode" + groupItem.index + "jcode@}");
                                }
                            }




                            foreach (var groupItem in groupList) {
                                var strone = new StringBuilder();


                                if (groupItem.F_Symbol == 8)
                                {
                                    strone.Append($" ({groupItem.Cfield} ");
                                }
                                else {
                                    strone.Append($" {groupItem.Cfield} ");
                                }
                                   

                                string value = await GetValue(groupItem.F_FiledValueType, groupItem.F_FiledValue);

                                switch (groupItem.F_Symbol)
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
                                        strone.Append($" != {value} or {groupItem.Cfield} is null ) ");
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

                                groupSqlWhere = groupSqlWhere.Replace("{@jcode" + groupItem.index + "jcode@}", strone.ToString());
                            }
                            groupSql.Append(groupSqlWhere);
                            groupSql.Append(" ) ");



                            strSql = strSql.Replace("{@jcode" + numIndex[i] + "jcode@}", groupSql.ToString());
                        }
                    }
                }


                int num = 1;
                foreach (var conditionItem in itemFormula.Conditions)
                {
                    if (string.IsNullOrEmpty(conditionItem.Group)) {
                        var strone = new StringBuilder();
                        if (conditionItem.Type == "glbd")
                        {
                            strone.Append($" { conditionItem.RelationField} in ( SELECT {conditionItem.Field} FROM {conditionItem.Table} WHERE {conditionItem.Cfield}  ");
                        }
                        else
                        {
                            if (conditionItem.F_Symbol == 8)
                            {
                                strone.Append($" ( {conditionItem.F_FieldId}");
                            }
                            else
                            {
                                strone.Append($" {conditionItem.F_FieldId}");
                            }

                        }


                        string value = await GetValue(conditionItem.F_FiledValueType, conditionItem.F_FiledValue);

                        switch (conditionItem.F_Symbol)
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

                                if (conditionItem.Type == "glbd")
                                {
                                    strone.Append($" or {conditionItem.Cfield} is null  ");
                                }
                                else
                                {
                                    strone.Append($" or { conditionItem.F_FieldId} is null ) ");
                                }


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
                        if (conditionItem.Type == "glbd")
                        {
                            strone.Append(" ) ");
                        }
                        else
                        {
                            strone.Append(" ");
                        }

                        strSql = strSql.Replace("{@jcode" + num + "jcode@}", strone.ToString());
                    }

                    
                    num++;
                }
                strRes.Append(strSql);
                strRes.Append(" ) ");

                sqlNum++;
            }

            return strRes.ToString();
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        private async Task<string> GetValue(int? type, string value)
        {
            var userInfo = await _userIBLL.GetEntity();
            //1.文本2.登录者ID3.登录者账号4.登录者公司5.登录者部门6.登录者岗位7.登录者角色
            string text;
            switch (type)
            {
                case 1:// 文本
                    text = $"'{value}'";
                    break;
                case 10:// 文本
                    text = value;
                    break;
                case 2:// 登录者ID
                    text = $"'{userInfo.F_UserId}'";
                    break;
                case 3:// 登录者账号
                    text = $"'{userInfo.F_Account}'";
                    break;
                case 4:// 登录者公司
                    text = $"'{userInfo.F_CompanyId}'";
                    break;
                case 41:// 登录者公司及下属公司
                    text = $"'{string.Join(",", await _companyIBLL.GetSubNodes(userInfo.F_CompanyId))}'";
                    break;
                case 42:// 固定公司
                    text = $"'{value}'";
                    break;
                case 5:// 登录者部门
                    text = $"'{userInfo.F_DepartmentId}'";
                    break;
                case 51:// 登录者部门及下属部门
                    var departmentList = new List<string>();
                    departmentList.AddRange(await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 3));
                    departmentList.AddRange(await _departmentIBLL.GetSubNodes(userInfo.F_DepartmentId));
                    text = $"'{string.Join(",", departmentList)}'";
                    break;
                case 52:// 固定部门
                    text = $"'{value}'";
                    break;
                case 6:// 登录者岗位
                    text = $"'{string.Join(",", await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 2))}'";
                    break;
                case 7:// 登录者角色
                    text = $"'{string.Join(",", await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 1))}'";
                    break;
                default:
                    text = $"'{value}'";
                    break;
            }
            return text;
        }

        /// <summary>
        /// 公式
        /// </summary>
        private class Formula {
            /// <summary>
            /// 设置公式
            /// </summary>
            public string formula { get; set; }
            /// <summary>
            /// 设置条件
            /// </summary>
            public List<Condition> Conditions { get; set; }
            /// <summary>
            /// 分组公式
            /// </summary>
            public List<Group> Formulas { get; set; }
        }
        /// <summary>
        /// 设置条件
        /// </summary>
        private class Condition
        {
            /// <summary>
            /// 编号
            /// </summary>
            public int index { get; set; }
            /// <summary>
            /// 字段ID
            /// </summary>
            /// <returns></returns>
            public string F_FieldId { get; set; }
            /// <summary>
            /// 比较符1.等于2.包含3.包含于4.不等于5.不包含6.不包含于
            /// </summary>
            /// <returns></returns>
            public int? F_Symbol { get; set; }
            /// <summary>
            /// 字段值类型1.文本2.登录者ID3.登录者账号4.登录者公司5.登录者部门6.登录者岗位7.登录者角色
            /// </summary>
            public int? F_FiledValueType { get; set; }
            /// <summary>
            /// 字段值
            /// </summary>
            /// <returns></returns>
            public string F_FiledValue { get; set; }

            /// <summary>
            /// 类型 glbd关联表单数据类型，需要特殊处理
            /// </summary>
            public string Type { get; set; }
            /// <summary>
            /// 表名称
            /// </summary>
            public string Table { get; set; }
            /// <summary>
            /// 比较字段
            /// </summary>
            public string Cfield { get; set; }
            /// <summary>
            /// 外键
            /// </summary>
            public string Field { get; set; }
            /// <summary>
            /// 关联表字段
            /// </summary>
            public string RelationField { get; set; }
            /// <summary>
            /// 分组编号
            /// </summary>
            public string Group { get; set; }
        }

        /// <summary>
        /// 分组公式
        /// </summary>
        private class Group {
            public string Value { get; set; }
        }
        #endregion
    }
}
