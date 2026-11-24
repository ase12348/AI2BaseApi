using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.oss;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using TencentCloud.Cme.V20191029.Models;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：用户
    /// </summary>
    public class UserBLL : UserIBLL, BLL
    {
        private readonly UserService userService = new UserService();
        private readonly ICache _iCache;
        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly IAnnexes _iAnnexes;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        /// <param name="userRelationIBLL"></param>
        public UserBLL(ICache iCache, UserRelationIBLL userRelationIBLL, IAnnexes iAnnexes)
        {
            _iCache = iCache;
            _userRelationIBLL = userRelationIBLL;
            _iAnnexes = iAnnexes;
        }

        #region 获取数据
        /// <summary>
        /// 获取用户信息通过账号
        /// </summary>
        /// <param name="account">用户账号</param>
        /// <returns></returns>
        public async Task<UserEntity> GetEntityByAccount(string account)
        {
            string key = $"user_{account.ToLower()}";
            var entity = _iCache.Read<UserEntity>(key);
            if (entity == null || entity.F_EnabledMark == 0) // 如果账号被锁定就再去数据库获取
            {
                entity = await userService.GetEntityByAccount(account);
                _iCache.Write(key, entity);
            }

            

            return entity;
        }
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<UserEntity> GetEntity(string keyValue)
        {
            string key = $"user_{keyValue}";
            var entity = _iCache.Read<UserEntity>(key);
            if (entity == null)
            {
                entity = await userService.GetEntity(keyValue);
                _iCache.Write(key, entity);
            }
            return entity;
        }
        
        /// <summary>
        /// 获取登录者用户信息
        /// </summary>
        /// <returns></returns>
        public async Task<UserEntity> GetEntity()
        {
            string account = ContextHelper.GetItem("account") as string;
            if (string.IsNullOrEmpty(account))
            {
                return new UserEntity();
            }
            var userEntity = await GetEntityByAccount(account);
            
            return userEntity;
        }
        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserEntity>> GetListByKeyValues(string keyValues)
        {
            var userList =  await userService.GetListByKeyValues(keyValues);
            foreach (var user in userList)
            {
                var post =await  _userRelationIBLL.GetObjectIds(user.F_UserId,2);
                if (post.Count > 0)
                {
                    user.F_Post = post[0];
                }
            }
            
            return userList;
        }

        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetListByKeyValues(string[] keyValues)
        {
            return userService.GetListByKeyValues(keyValues);
        }

        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetListByKeyValues2(string[] keyValues)
        {
            return userService.GetListByKeyValues2(keyValues);
        }
        /// <summary>
        /// 用户列表(根据公司主键,部门主键)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="keyword">查询关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetList(string companyId, string departmentId, string keyword)
        {
            return userService.GetList(companyId, departmentId, keyword);
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="departmentDeepNum">部门层级编码</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="userType">人员类别</param>
        /// <param name="salaryLevel">待遇级别</param>
        /// <param name="type">账号状态类型</param>
        /// <param name="isBlack">是否是黑名单</param>
        /// <param name="enabledMark">是否获取有效用户</param>
        /// <param name="gender">性别</param>
        /// <param name="ids">用户ids</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetPageList(string companyId, string departmentId,  string departmentDeepNum,  Pagination pagination, string keyword,string userType,string salaryLevel,int? type,int? isBlack,  int? enabledMark, int? gender,string ids = "")
        {
            return userService.GetPageList(companyId, departmentId, departmentDeepNum, pagination, keyword,userType,salaryLevel,type,isBlack, enabledMark,gender, ids);
        }
        /// <summary>
        /// 用户列表,全部
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetAllList(string keyword)
        {
            return userService.GetAllList(keyword);
        }
        /// <summary>
        /// 用户列表（导出Excel）
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetExportList()
        {
            var id = Guid.NewGuid().ToString();
            //取出数据源
            DataTable exportTable = await userService.GetExportList();
            //设置导出格式
            ExcelConfig excelconfig = new ExcelConfig();
            excelconfig.Title = "测试用户导出";
            excelconfig.TitleFont = "微软雅黑";
            excelconfig.TitlePoint = 25;
            excelconfig.FileName = "用户信息.xlsx"; //$"{ConfigHelper.GetConfig().FilePath}/Jcode_EXCEL_EXPORT/{id}";
            excelconfig.IsAllSizeColumn = true;
            //每一列的设置,没有设置的列信息，系统将按datatable中的列名导出
            excelconfig.ColumnEntity = new List<ColumnModel>();
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_account", ExcelColumn = "账户" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_realname", ExcelColumn = "姓名" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_gender", ExcelColumn = "性别" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_birthday", ExcelColumn = "生日" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_mobile", ExcelColumn = "手机", Background = Color.Red });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_telephone", ExcelColumn = "电话", Background = Color.Red });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_wechat", ExcelColumn = "微信" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_company", ExcelColumn = "公司" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_department", ExcelColumn = "部门" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_description", ExcelColumn = "说明" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_createdate", ExcelColumn = "创建日期" });
            excelconfig.ColumnEntity.Add(new ColumnModel() { Column = "f_createusername", ExcelColumn = "创建人" });
            // 保存为一个临时文件
            //ExcelHelper.ExcelExport(exportTable, excelconfig);
            _iAnnexes.UploadFile(id,$"Excel/{id}.xlsx",".xlsx", ExcelHelper.ExportMemoryStream(exportTable, excelconfig).ToArray(),0);

            return id;
        }
        /// <summary>
        /// 获取超级管理员用户列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<UserEntity>> GetAdminList()
        {
            return userService.GetAdminList();
        }



        /// <summary>
        /// 获取用户信息通过openid
        /// </summary>
        /// <param name="openId">小程序的openid</param>
        /// <param name="type">类型，默认weixin;dingtalk,ali</param>
        /// <returns></returns>
        public Task<UserEntity> GetEntityByOpenId(string openId, string type = "weixin") {
            return userService.GetEntityByOpenId(openId, type);
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
        public Task<IEnumerable<UserEntity>> GetList(
            List<string> users,
            List<string> departments,
            List<string> companys,
            List<string> secretLevels,
            List<string> levels,
            List<string> jobs,
            List<string> roles,
            List<string> titles,
            List<string> posts,
            List<string> limitDepartments,
            List<string> outDepartments)
        {
            return userService.GetList(users,departments,companys,secretLevels,levels,jobs,roles,titles,posts,limitDepartments,outDepartments);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var entity = await GetEntity(keyValue);
            if (entity!=null)
            {
                if (entity.F_SecurityLevel == 1)//系统用户无法删除
                {
                    throw new Exception("系统内置用户，无法删除！");
                }
                _iCache.Remove($"user_{entity.F_Account.ToLower()}");
                _iCache.Remove($"user_{keyValue}");

                entity.F_DeleteMark = 1;
                await userService.SaveEntity(keyValue,entity);
            }
            //await userService.Delete(keyValue);
        }
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, UserEntity userEntity)
        {
            await userService.SaveEntity(keyValue, userEntity);
            if (!string.IsNullOrEmpty(keyValue))
            {
                _iCache.Remove($"user_{userEntity.F_Account.ToLower()}");
                _iCache.Remove($"user_{keyValue}");
            }
        }
        /// <summary>
        /// 保存用户表单-批量（新增）----ADD BY SSY 20221008
        /// </summary>
        /// <param name="userEntitys">用户实体集</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<UserEntity> userEntitys)
        {
            await userService.SaveEntitys(userEntitys);
        }
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        public UserEntity GetEntity(UserEntity userEntity) {
            return userService.GetEntity(userEntity);
        }


        /// <summary>
        /// 保存用户表单
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dto">用户数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, UserDto dto)
        {
            userService.BeginTrans();
            try
            {
                await userService.SaveEntity(keyValue, dto.User);
                if (dto.Roles != null) {
                    await _userRelationIBLL.SaveEntityList(dto.User.F_UserId,dto.User.F_RealName, 1, dto.Roles);
                }
                if (dto.Posts != null)
                {
                    await _userRelationIBLL.SaveEntityList(dto.User.F_UserId,dto.User.F_RealName, 2, dto.Posts);
                }
                if (dto.Departments != null)
                {
                    await _userRelationIBLL.SaveEntityList(dto.User.F_UserId,dto.User.F_RealName, 3, dto.Departments);
                }
                if (dto.Companys != null)
                {
                    await _userRelationIBLL.SaveEntityList(dto.User.F_UserId,dto.User.F_RealName, 4, dto.Companys);
                }
                userService.Commit();
            }
            catch (System.Exception)
            {
                userService.Rollback();
                throw;
            }

            if (!string.IsNullOrEmpty(keyValue))
            {
                _iCache.Remove($"user_{dto.User.F_Account.ToLower()}");
                _iCache.Remove($"user_{keyValue}");
            }
        }

        
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        public async Task UpdateEntity(UserEntity userEntity)
        {
            _iCache.Remove($"user_{userEntity.F_Account.ToLower()}");
            _iCache.Remove($"user_{userEntity.F_UserId}");
            await userService.UpdateEntity(userEntity);
        }
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="newPassword">新密码（MD5 小写）</param>
        /// <param name="oldPassword">旧密码（MD5 小写）</param>
        public async Task<bool> RevisePassword(string newPassword, string oldPassword)
        {
            var userInfo = await GetEntity();
            _iCache.Remove($"user_{userInfo.F_Account.ToLower()}");
            _iCache.Remove($"user_{userInfo.F_UserId}");
            string oldPasswordByEncrypt = Md5Helper.Encrypt(DESEncrypt.Encrypt(oldPassword, userInfo.F_Secretkey).ToLower(), 32).ToLower();
            if (oldPasswordByEncrypt == userInfo.F_Password)
            {
                await userService.RevisePassword(userInfo, newPassword);
            }
            else
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 重置密码(123456)
        /// </summary>
        /// <param name="keyValue">账号主键</param>
        public async Task ResetPassword(string keyValue)
        {
            var userInfo = await GetEntity(keyValue);
            userInfo.F_LastChangePDate = null;//重置密码后要求用户立即修改密码
            string password = Md5Helper.Encrypt("123456", 32).ToLower();// 密码重置，后面改成可以配置

            _iCache.Remove($"user_{userInfo.F_Account.ToLower()}");
            _iCache.Remove($"user_{keyValue}");

            await userService.RevisePassword(userInfo, password);
        }
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="state">状态：1-启动；0-禁用</param>
        public async Task UpdateState(string keyValue, int state)
        {
            var userInfo = await GetEntity(keyValue);
            _iCache.Remove($"user_{userInfo.F_Account.ToLower()}");
            _iCache.Remove($"user_{keyValue}");
            await userService.UpdateState(userInfo, state);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断密码是否正确
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassWord">新密码</param>
        /// <param name="secretkey">密钥</param>
        /// <returns></returns>
        public bool IsPasswordOk(string oldPassword, string newPassWord, string secretkey)
        {

            string dbPassword = Md5Helper.Encrypt(DESEncrypt.Encrypt(newPassWord, secretkey).ToLower(), 32).ToLower();
            if (dbPassword == oldPassword)
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 获取用户信息通过CA登录名
        /// </summary>
        /// <param name="causersign">CA登录名</param>
        /// <returns></returns>
        public async Task<UserEntity> GetEntityByCaUserSign(string causersign)
        {
            return await userService.GetEntityByCaUserSign(causersign);
        }
        #endregion
    }
}
