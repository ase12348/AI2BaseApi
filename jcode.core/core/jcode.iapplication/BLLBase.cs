using System;
using jcode.autofac;
using jcode.orm;
using jcode.util;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：业务基类
    /// </summary>
    public class BLLBase
    {


        #region 基础信息
        private List<string> roleIds;
        private List<string> postIds;
        private ConcurrentDictionary<string, object> codeDic;

        /// <summary>
        /// 获取当前用户信息
        /// </summary>
        /// <param name="userId">用户id</param>
        /// <returns></returns>
        public async Task<UserEntity> CurrentUser(string userId = null)
        {
            var userBLL = IocManager.Instance.GetService<UserIBLL>();
            if (string.IsNullOrEmpty(userId))
            {
                return await userBLL.GetEntity();
            }
            else
            {
                return await userBLL.GetEntity(userId);
            }
        }
        
        /// <summary>
        /// 获取公司信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<CompanyEntity> GetCompany(string id)
        {
            var companyIBLL = IocManager.Instance.GetService<CompanyIBLL>();
            return companyIBLL.GetEntity(id);
        }

        /// <summary>
        /// 获取部门信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<DepartmentEntity> GetDepartment(string id)
        {
            var departmentIBLL = IocManager.Instance.GetService<DepartmentIBLL>();
            return departmentIBLL.GetEntity(id);
        }
        
        
        /// <summary>
        /// 获取当前部门
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetDepartmentId()
        {
            var postId =await GetPostId();
            if (!string.IsNullOrEmpty(postId))
            {
                var postIBLL = IocManager.Instance.GetService<PostIBLL>();
                var postEntity = await postIBLL.GetEntity(postId);
                return postEntity.F_DepartmentId;
            }
            return (await CurrentUser()).F_DepartmentId;
        }
        
        /// <summary>
        /// 获取当前公司
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetCompanyId()
        {
            var postId =await GetPostId();
            if (!string.IsNullOrEmpty(postId))
            {
                var postIBLL = IocManager.Instance.GetService<PostIBLL>();
                var postEntity = await postIBLL.GetEntity(postId);
                return postEntity.F_CompanyId;
            }
            return (await CurrentUser()).F_CompanyId;
        }

        /// <summary>
        /// 获取当前登录者的角色id集合
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> CurrentUserRoleIds()
        {
            if (roleIds == null)
            {
                var _userInfo = await CurrentUser();
                var userRelationIBLL = IocManager.Instance.GetService<UserRelationIBLL>();
                roleIds = await userRelationIBLL.GetObjectIds(_userInfo.F_UserId, 1);
            }

            return roleIds;
        }
        /// <summary>
        /// 获取当前登录者的角色id集合
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        public Task<List<string>> CurrentUserRoleIds(string userId)
        {
            var userRelationIBLL = IocManager.Instance.GetService<UserRelationIBLL>();
            return userRelationIBLL.GetObjectIds(userId, 1);
        }


        /// <summary>
        /// 获取当前登录者的岗位id集合
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> CurrentUserPostIds()
        {
            if (postIds == null)
            {
                var userRelationIBLL = IocManager.Instance.GetService<UserRelationIBLL>();
                postIds = await userRelationIBLL.GetObjectIds(GetUserId(), 2);
            }

            return postIds;
        }

        /// <summary>
        ///  获取当前登录者的岗位id集合
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        public Task<List<string>> CurrentUserPostIds(string userId)
        {
            var userRelationIBLL = IocManager.Instance.GetService<UserRelationIBLL>();
            return userRelationIBLL.GetObjectIds(userId, 2);
        }
        
        

        
        

        
        
        


        /// <summary>
        /// 获取登录者用户名称
        /// </summary>
        /// <returns></returns>
        public string GetUserName()
        {
            return ContextHelper.GetItem("userName") as string;
        }
        
        /// <summary>
        /// 获取登录者用户Id
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            return ContextHelper.GetItem("userId") as string;
        }
        
        /// <summary>
        /// 获取登录者用户账号
        /// </summary>
        /// <returns></returns>
        public string GetUserAccount()
        {
            return ContextHelper.GetItem("account") as string;
        }
        
        
        /// <summary>
        /// 获取登录者当前岗位
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetPostId()
        {
            var postList = await CurrentUserPostIds();
            if (postList.Count > 1) // 大于1个岗位才需要设置主主岗所在部门
            {
                var postId = ContextHelper.GetItem("postId") as string;
                if (!string.IsNullOrEmpty(postId) && postList.IndexOf(postId) > -1 )
                {
                   return  postId;
                }
                else
                {
                    postId = postList[0];
                    ContextHelper.SetItem("postId", postId);
                    
                    return postId;
                }
            }
            ContextHelper.SetItem("postId", "");
            return "";
        }
        
        /// <summary>
        ///  获取默认岗位
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <returns></returns>
        public async Task<string> GetPostId(string userId)
        {
            var postList = await CurrentUserPostIds(userId);
            if (postList.Count > 1) // 大于1个岗位才需要设置主主岗所在部门
            {
                return postList[0];
            }
            return "";
        }
        
        /// <summary>
        /// 获取租户号
        /// </summary>
        /// <returns></returns>
        public string GetTenantNo()
        {
            return ContextHelper.GetItem("TenantNo") as string;
        }

        /// <summary>
        /// 获取租户名称
        /// </summary>
        /// <returns></returns>
        public string GetTenantName()
        {
            return ContextHelper.GetItem("TenantName") as string;
        }
        
        /// <summary>
        /// 获取租户公司名
        /// </summary>
        /// <returns></returns>
        public string GetTenantCompany()
        {
            return ContextHelper.GetItem("TenantCompany") as string;
        }

        /// <summary>
        /// 判断是否是超级管理员
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            var res = GetSecurityLevel();

            if (res == "1")
            {
                return true;
            }

            return false;
        }

        #endregion

        #region 数据权限
        /// <summary>
        /// 获取数据权限设置sql查询语句
        /// </summary>
        /// <param name="code">数据权限编码</param>
        /// <param name="isForm">是否是自定义表单功能</param>
        /// <returns></returns>
        public async Task<string> GetDataAuthoritySql(string code,bool isForm = false)
        {
            var dataAuthorizeIBLL = IocManager.Instance.GetService<DataAuthorizeIBLL>();
            return await dataAuthorizeIBLL.GetWhereSql(code, isForm);
        }
        #endregion

        #region 单据编码
        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="code">编码规则编码</param>
        /// <param name="formData">表单值</param>
        /// <returns></returns>
        public Task<string> GetRuleCode(string code,string formData = "")
        {
            var codeRuleIBLL = IocManager.Instance.GetService<CodeRuleIBLL>();
            return codeRuleIBLL.GetBillCode(code,formData);
        }
        /// <summary>
        /// 获取编码
        /// </summary>
        /// <param name="value">字段值</param>
        /// <returns></returns>
        public async Task<object> GetRuleCodeEx(object value)
        {

            var code = value;
            if (value != null && value.ToString().IndexOf("jcode_code_") != -1)
            {
                if (codeDic == null)
                {
                    codeDic = new ConcurrentDictionary<string, object>();
                }
                var codeList = value.ToString().Replace("jcode_code_", "").Split("|");
                if (codeDic.ContainsKey(codeList[1]))
                {
                    codeDic.TryGetValue(codeList[1], out code);
                }
                else
                {
                    code = await this.GetRuleCode(codeList[0],codeList.Length == 3 ? codeList[2]:"");
                    codeDic.TryAdd(codeList[1], code);
                }

            }
            return code;
        }



        #endregion

        #region 数据响应
        /// <summary>
        /// 返回成功
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="info">信息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public ResponseDto<T> Success<T>(string info, T data) where T : class
        {
            return new ResponseDto<T> { code = ResponseCode.success, info = info, data = data };
        }

        /// <summary>
        /// 返回成功
        /// </summary>
        /// <param name="info">信息</param>
        /// <returns></returns>
        public ResponseDto Success(string info)
        {
            return new ResponseDto { code = ResponseCode.success, info = info, data = new { } };
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        /// <typeparam name="T">数据类型</typeparam>
        /// <param name="info">信息</param>
        /// <param name="data">数据</param>
        /// <returns></returns>
        public ResponseDto<T> Fail<T>(string info, T data = null) where T : class
        {
            return new ResponseDto<T> { code = ResponseCode.fail, info = info, data = data };
        }
        #endregion

        #region  安全人员和密级
        /// <summary>
        /// 获取用户安全等级
        /// </summary>
        /// <returns></returns>
        public string GetSecurityLevel()
        {
            return ContextHelper.GetItem("SecurityLevel") as string;
        }
        
        
        /// <summary>
        /// 获取当前用户密级
        /// </summary>
        /// <returns></returns>
        public int GetSecretLevel()
        {
            var secretLevel = ContextHelper.GetItem("SecretLevel") as string;
            if (string.IsNullOrEmpty(GetUserId()))
            {
                // 任务调度
                return 4;
            }

            if (string.IsNullOrEmpty(secretLevel))
            {
                return 0;
            }
            else
            {
                return  Int32.Parse(secretLevel);
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string SqlFilters(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            //去除执行SQL语句的命令关键字
            source = Regex.Replace(source, "/add", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", "( ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " /add ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, "xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, "0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }

        /// <summary>
        /// 不过滤select关键字
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public string SqlFiltersNotSelect(string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                return source;
            }

            //去除执行SQL语句的命令关键字
            // source = Regex.Replace(source, "select", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "/add", " ", RegexOptions.IgnoreCase);
            // source = Regex.Replace(source, "\\+", " ", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, "\\(", "( ", RegexOptions.IgnoreCase);
            
            source = Regex.Replace(source, " insert ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " update ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " delete ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " drop ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " truncate ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " declare ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " xp_cmdshell ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " net user ", "", RegexOptions.IgnoreCase);
            //去除执行存储过程的命令关键字 
            source = Regex.Replace(source, " exec ", "", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " execute ", "", RegexOptions.IgnoreCase);
            //去除系统存储过程或扩展存储过程关键字
            source = Regex.Replace(source, " xp_", "x p_", RegexOptions.IgnoreCase);
            source = Regex.Replace(source, " sp_", "s p_", RegexOptions.IgnoreCase);
            //防止16进制注入
            source = Regex.Replace(source, " 0x", "0 x", RegexOptions.IgnoreCase);
            return source;
        }
    }
}
