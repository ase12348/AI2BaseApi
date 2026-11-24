using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TencentCloud.Ic.V20190307.Models;
using TencentCloud.Tcr.V20190924.Models;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：用户(数据库操作)
    /// </summary>
    public class UserService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取用户信息通过账号
        /// </summary>
        /// <param name="account">用户账号</param>
        /// <returns></returns>
        public Task<UserEntity> GetEntityByAccount(string account)
        {
            return this.BaseRepository().FindEntity<UserEntity>(t=>t.F_Account == account && t.F_DeleteMark == 0);
        }

        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetListByKeyValues(string keyValues)
        {
            var keyValueList = keyValues.Split(',');
            return this.BaseRepository().FindList<UserEntity>(t => keyValueList.Contains(t.F_UserId));
        }

        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetListByKeyValues(string[] keyValues)
        {
            return this.BaseRepository().FindList<UserEntity>(t => keyValues.Contains(t.F_UserId));
        }


        /// <summary>
        /// 用户列表(根据用户主键集合) 获取我的委托人
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetListByKeyValues2(string[] keyValues)
        {
            return this.BaseRepository().FindList<UserEntity>(t => t.F_DeleteMark == 0 && t.F_EnabledMark == 1 && keyValues.Contains(t.F_UserId));
        }

        /// <summary>
        /// 用户列表(根据公司主键)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetList(string companyId, string departmentId, string keyword)
        {
            var exp = Expressionable.Create<UserEntity>()
               .And(t => t.F_DeleteMark == 0)
               
               .AndIF(!string.IsNullOrEmpty(companyId), t => t.F_CompanyId == companyId)
               .AndIF(!string.IsNullOrEmpty(departmentId), t => t.F_DepartmentId == departmentId)
               .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) || t.F_Mobile.Contains(keyword))
               .ToExpression();
            return this.BaseRepository().FindList<UserEntity>(exp);
        }

        /// <summary>
        /// 用户列表(根据公司主键)(分页)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="departmentDeepNum">部门层级编码</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">关键词</param>
        /// <param name="userType">人员类别</param>
        /// <param name="salaryLevel">待遇级别</param>
        /// <param name="type">账号状态类型</param>
        /// <param name="isBlack">是否是黑名单</param>
        /// <param name="enabledMark">是否获取有效用户</param>
        /// <param name="gender">性别</param>
        /// <param name="ids">用户ids</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetPageList(
            string companyId, 
            string departmentId, 
            string departmentDeepNum, 
            Pagination pagination, 
            string keyword,
            string userType,
            string salaryLevel,
            int? type,
            int? isBlack, 
            int? enabledMark,
            int? gender, 
            string ids = "")
        {
            var idList =new string[0];
            if (!string.IsNullOrEmpty(ids)) {
                idList = ids.Split(',');
            }

            var departmentChildDeepNum = "";
            if (!string.IsNullOrEmpty(departmentDeepNum))
            {
                departmentChildDeepNum = departmentDeepNum + ".";
            }



            var exp = Expressionable.Create<UserEntity>()
                .And(t => t.F_DeleteMark == 0 || t.F_DeleteMark == null)
                .AndIF(enabledMark == 1, t => t.F_EnabledMark == 1)
                .AndIF(!string.IsNullOrEmpty(companyId), t => t.F_CompanyId == companyId)
                .AndIF(!string.IsNullOrEmpty(departmentId), t => t.F_DepartmentId == departmentId)
                .AndIF(!string.IsNullOrEmpty(departmentDeepNum), t => SqlFunc.Subqueryable<DepartmentEntity>().Where(s =>(s.F_DeepNum == departmentDeepNum  || s.F_DeepNum.Contains(departmentChildDeepNum))&& s.F_DepartmentId==t.F_DepartmentId ).Any())
                .AndIF(!string.IsNullOrEmpty(ids), t => idList.Contains(t.F_UserId))
                .AndIF(!string.IsNullOrEmpty(keyword),
                    t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) ||
                         t.F_Mobile.Contains(keyword))
                .AndIF(!string.IsNullOrEmpty(userType),t=>t.F_UserType == userType)
                .AndIF(!string.IsNullOrEmpty(salaryLevel),t=>t.F_SalaryLevel == salaryLevel)
                .AndIF(type != null && type != 0 && type != 1,t=>t.F_Type == type)
                .AndIF(type == 1,t=>(t.F_Type == 1 || t.F_Type == null))
                .AndIF(type == 0,t=>(t.F_Type != 3 || t.F_Type == null))
                .AndIF(isBlack == 1,t=>t.F_IsBlack == 1)
                .AndIF(isBlack != 1,t=>t.F_IsBlack != 1 || t.F_IsBlack == null)
                .AndIF(gender == 1,t=>t.F_Gender  == 1)
                .AndIF(gender != 1 && gender != null,t=>t.F_Gender  == gender || t.F_Gender == null)
                ;
            
            /*if (IsAdmin())//超级管理员能够看到全部的用户信息
            {
                exp.And(t => t.F_SecurityLevel != 1 || t.F_SecurityLevel == null);//超级账户能看到全部的用户信息
            }
            else
            {
                exp.And(t => t.F_SecurityLevel == null || t.F_SecurityLevel == 0); //其它用户只能看到业务用户数据
            }*/
            
            
            exp.And(t => t.F_SecurityLevel != 1 || t.F_SecurityLevel == null);//超级账户能看到全部的用户信息

            return this.BaseRepository().FindList(exp.ToExpression(), pagination);
        }

        /// <summary>
        /// 用户列表,全部
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetAllList(string keyword)
        {
            var exp = Expressionable.Create<UserEntity>()
              .And(t => t.F_DeleteMark == 0)
              .And(t => t.F_EnabledMark == 1)
              .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Account.Contains(keyword) || t.F_RealName.Contains(keyword) || t.F_Mobile.Contains(keyword))
              .ToExpression();
            return this.BaseRepository().FindList<UserEntity>(exp, "F_CompanyId,F_DepartmentId,F_RealName");
        }
        /// <summary>
        /// 用户列表（导出Excel）
        /// </summary>
        /// <returns></returns>
        public Task<DataTable> GetExportList()
        {
            var strSql = new StringBuilder();
            strSql.Append(@"SELECT t.F_Account
                                  ,t.F_RealName
                                  ,CASE WHEN t.F_Gender=1 THEN '男' ELSE '女' END AS F_Gender
                                  ,t.F_Birthday
                                  ,t.F_Mobile
                                  ,t.F_Telephone
                                  ,t.F_Email
                                  ,t.F_WeChat
                                  ,t.F_MSN
                                  ,o.F_FullName AS F_Company
                                  ,d.F_FullName AS F_Department
                                  ,t.F_Description
                                  ,t.F_CreateDate
                                  ,t.F_CreateUserName
                              FROM LR_Base_User t
                              INNER JOIN LR_Base_Dept d ON t.F_DepartmentId=d.F_DepartmentId
                              INNER JOIN LR_Base_Company o ON t.F_CompanyId=o.F_CompanyId WHERE t.F_DeleteMark = 0  {JCODE_SASSID} ");
            return this.BaseRepository().FindTable(strSql.ToString());
        }
        /// <summary>
        /// 用户实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<UserEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<UserEntity>(keyValue);
        }
        /// <summary>
        /// 获取超级管理员用户列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetAdminList()
        {
            return this.BaseRepository().FindList<UserEntity>(t=>t.F_SecurityLevel == 1);
        }


        /// <summary>
        /// 获取用户信息通过openid
        /// </summary>
        /// <param name="openId">小程序的openid</param>
        /// <param name="type">类型，默认weixin;dingtalk,ali</param>
        /// <returns></returns>
        public Task<UserEntity> GetEntityByOpenId(string openId,string type = "weixin")
        {
            switch (type) {
                case "dingtalk":// 钉钉
                    return this.BaseRepository().FindEntity<UserEntity>(t => t.F_DDOpenId == openId);
                case "ali":     // 支付宝
                    return this.BaseRepository().FindEntity<UserEntity>(t => t.F_AliOpenId == openId);
                case "feishu":  // 飞书ID
                    return this.BaseRepository().FindEntity<UserEntity>(t => t.F_FeishuOpenId == openId);
                default:// 微信
                    return this.BaseRepository().FindEntity<UserEntity>(t=>t.F_WxOpenId == openId);
            }
        }
        
        
        
        /// <summary>
        /// 用户列表
        /// </summary>
        /// <param name="users">用户</param>
        /// <param name="departments">部门</param>
        /// <param name="companys">公司</param>
        /// <param name="secretLevels">密级</param>
        /// <param name="levels">行政级别</param>
        /// <param name="jobs">行政级别</param>
        /// <param name="roles">技术职务</param>
        /// <param name="titles">职称等级</param>
        /// <param name="posts">管理职务</param>
        /// <param name="limitDepartments">限定部门</param>
        /// <param name="outDepartments">排除部门</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserEntity>> GetList(
            List<string> users,
            List<string> departments ,
            List<string> companys,
            List<string> secretLevels ,
            List<string> levels,
            List<string> jobs,
            List<string> roles,
            List<string> titles,
            List<string> posts,
            List<string> limitDepartments,
            List<string> outDepartments)
        {
            if (users.Count == 0 
                && departments.Count == 0  
                && secretLevels.Count == 0  
                && levels.Count == 0
                && jobs.Count == 0
                && roles.Count == 0  
                && titles.Count == 0 
                && posts.Count == 0)
            {
                return new List<UserEntity>();
            }

            var exp = Expressionable.Create<UserEntity>()
                .And(t => t.F_DeleteMark == 0)
                .And(t=>
                    users.Contains(t.F_UserId) 
                    ||departments.Contains(t.F_DepartmentId)
                    ||companys.Contains(t.F_CompanyId)
                    ||secretLevels.Contains(t.F_SecretLevel.ToString())
                    ||levels.Contains(t.F_Level)
                    ||jobs.Contains(t.F_Job)
                    ||roles.Contains(t.F_Role)
                    ||titles.Contains(t.F_Title)
                    ||posts.Contains(t.F_Post)
                )

                .AndIF(limitDepartments.Count>0, t =>limitDepartments.Contains(t.F_DepartmentId))
                .AndIF(outDepartments.Count>0, t =>!outDepartments.Contains(t.F_DepartmentId))
                .ToExpression();
            return await this.BaseRepository().FindList<UserEntity>(exp);
        }
        
        
        /// <summary>
        /// 获取岗位的实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<PostEntity> GetPostEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<PostEntity>(keyValue);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await this.BaseRepository().Delete<UserEntity>(t => t.F_UserId.Equals(keyValue));
        }


        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public UserEntity GetEntity(UserEntity userEntity)
        {
            userEntity.F_CreateUserId = this.GetUserId();
            userEntity.F_CreateUserName = this.GetUserName();

            userEntity.F_UserId = Guid.NewGuid().ToString();
            userEntity.F_CreateDate = DateTime.Now;
            userEntity.F_DeleteMark = 0;
            userEntity.F_EnabledMark = 1;
            userEntity.F_Secretkey = Md5Helper.Encrypt(CommonHelper.CreateNo(), 16).ToLower();
            userEntity.F_Password = Md5Helper.Encrypt(DESEncrypt.Encrypt(userEntity.F_Password, userEntity.F_Secretkey).ToLower(), 32).ToLower();
            return userEntity;
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, UserEntity userEntity)
        {
            //新增
            if (string.IsNullOrEmpty(keyValue))
            {
                userEntity.F_CreateUserId = this.GetUserId();
                userEntity.F_CreateUserName = this.GetUserName();

                userEntity.F_UserId = Guid.NewGuid().ToString();
                userEntity.F_CreateDate = DateTime.Now;
                userEntity.F_DeleteMark = 0;
                userEntity.F_EnabledMark = 1;
                userEntity.F_Secretkey = Md5Helper.Encrypt(CommonHelper.CreateNo(), 16).ToLower();
                userEntity.F_Password = Md5Helper.Encrypt(DESEncrypt.Encrypt(userEntity.F_Password, userEntity.F_Secretkey).ToLower(), 32).ToLower();
                await this.BaseRepository().Insert(userEntity);
            }
            else
            {
                userEntity.F_UserId = keyValue;
                userEntity.F_ModifyUserId = this.GetUserId();
                userEntity.F_ModifyUserName = this.GetUserName();
                userEntity.F_ModifyDate = DateTime.Now;



                var userOldEntity = await this.BaseRepository().FindEntityByKey<UserEntity>(keyValue);

                userEntity.F_CreateUserId = userOldEntity.F_CreateUserId;
                userEntity.F_CreateUserName = userOldEntity.F_CreateUserName;
                userEntity.F_CreateDate = userOldEntity.F_CreateDate;

                if (userEntity.F_SecurityLevel != 5 && userEntity.F_SecurityLevel != 0)
                {
                    userEntity.F_SecurityLevel = userOldEntity.F_SecurityLevel;
                }

               
                userEntity.F_Secretkey = userOldEntity.F_Secretkey;
                userEntity.F_Password = userOldEntity.F_Password;
                userEntity.F_Account = userOldEntity.F_Account;
                await this.BaseRepository().Update(userEntity);
            }
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="userEntity">用户</param>
        /// <param name="password">新密码（MD5 小写）</param>
        public async Task RevisePassword(UserEntity userEntity, string password)
        {
            userEntity.F_LastChangePDate = DateTime.Now;
            userEntity.F_ModifyUserId = this.GetUserId();
            userEntity.F_ModifyUserName = this.GetUserName();
            userEntity.F_ModifyDate = DateTime.Now;
            userEntity.F_Secretkey = Md5Helper.Encrypt(CommonHelper.CreateNo(), 16).ToLower();
            userEntity.F_Password = Md5Helper.Encrypt(DESEncrypt.Encrypt(password, userEntity.F_Secretkey).ToLower(), 32).ToLower();
            userEntity.F_IsNeedChangePwd = 0;
            await this.BaseRepository().Update(userEntity);
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="userEntity">用户</param>
        /// <param name="state">状态：1-启动；0-禁用</param>
        public async Task UpdateState(UserEntity userEntity, int state)
        {
            userEntity.F_ModifyUserId = this.GetUserId();
            userEntity.F_ModifyUserName = this.GetUserName();
            userEntity.F_ModifyDate = DateTime.Now;
            userEntity.F_EnabledMark = state;

            await this.BaseRepository().Update(userEntity);
        }
        /// <summary>
        /// 修改用户信息
        /// </summary>
        /// <param name="userEntity">实体对象</param>
        public async Task UpdateEntity(UserEntity userEntity)
        {
            await this.BaseRepository().Update(userEntity);
        }

        /// <summary>
        /// 保存用户表单-批量（新增）----CHANGE BY SSY 20231108
        /// </summary>
        /// <param name="userEntitys">用户实体集</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<UserEntity> userEntitys)
        {
            int addNum = 0;
            if (userEntitys.Count > 1000)
            {
                this.BaseRepository().ORM.Utilities.PageEach(userEntitys, 1000, (pageList) => {
                    this.BaseRepository().ORM.StorageableByObject(pageList).ExecuteCommand();
                    addNum += pageList.Count;
                });
            }
            else if (userEntitys.Count > 0)
            {
                await this.BaseRepository().Inserts(userEntitys);
            }
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取用户信息通过CA登录名
        /// </summary>
        /// <param name="causersign">用户账号</param>
        /// <returns></returns>
        public Task<UserEntity> GetEntityByCaUserSign(string causersign)
        {
            return this.BaseRepository().FindEntity<UserEntity>(t => t.F_DomainName == causersign && t.F_DeleteMark == 0);
        }
        #endregion
    }
}
