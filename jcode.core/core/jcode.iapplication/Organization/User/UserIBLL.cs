using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.12
    /// 描 述：用户
    /// </summary>
    public interface UserIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取用户信息通过账号
        /// </summary>
        /// <param name="account">用户账号</param>
        /// <returns></returns>
        Task<UserEntity> GetEntityByAccount(string account);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<UserEntity> GetEntity(string keyValue);
        /// <summary>
        /// 获取登录者用户信息
        /// </summary>
        /// <returns></returns>
        Task<UserEntity> GetEntity();
        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetListByKeyValues(string keyValues);
        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetListByKeyValues(string[] keyValues);
        /// <summary>
        /// 用户列表(根据用户主键集合)
        /// </summary>
        /// <param name="keyValues">用户主键集合主键</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetListByKeyValues2(string[] keyValues);
        /// <summary>
        /// 用户列表(根据公司主键,部门主键)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门主键</param>
        /// <param name="keyword">查询关键词</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetList(string companyId, string departmentId, string keyword);
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
        Task<IEnumerable<UserEntity>> GetPageList(string companyId, string departmentId,string departmentDeepNum ,Pagination pagination, string keyword,string userType,string salaryLevel,int? type,int? isBlack,  int? enabledMark,int? gender, string ids = "");
        /// <summary>
        /// 用户列表,全部
        /// </summary>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetAllList(string keyword);
        /// <summary>
        /// 用户列表（导出Excel）
        /// </summary>
        /// <returns></returns>
        Task<string> GetExportList();

        /// <summary>
        /// 获取超级管理员用户列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<UserEntity>> GetAdminList();

        /// <summary>
        /// 获取用户信息通过openid
        /// </summary>
        /// <param name="openId">小程序的openid</param>
        /// <param name="type">类型，默认weixin;dingtalk,ali</param>
        /// <returns></returns>
        Task<UserEntity> GetEntityByOpenId(string openId, string type = "weixin");

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
        Task<IEnumerable<UserEntity>> GetList(
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
            List<string> outDepartments);
        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存用户表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, UserEntity userEntity);
        /// <summary>
        /// 保存用户表单-批量（新增）----ADD BY SSY 20221008
        /// </summary>
        /// <param name="userEntitys">用户实体集</param>
        /// <returns></returns>
        Task SaveEntitys(List<UserEntity> userEntitys);
        /// <summary>
        /// 获取用户
        /// </summary>
        /// <param name="userEntity">用户实体</param>
        /// <returns></returns>
        UserEntity GetEntity(UserEntity userEntity);

        /// <summary>
        /// 保存用户表单
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="dto">用户数据</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, UserDto dto);
        /// <summary>
        /// 更新用户信息
        /// </summary>
        /// <param name="userEntity"></param>
        /// <returns></returns>
        Task UpdateEntity(UserEntity userEntity);
        /// <summary>
        /// 修改用户登录密码
        /// </summary>
        /// <param name="newPassword">新密码（MD5 小写）</param>
        /// <param name="oldPassword">旧密码（MD5 小写）</param>
        Task<bool> RevisePassword(string newPassword, string oldPassword);
        /// <summary>
        /// 重置密码(123456)
        /// </summary>
        /// <param name="keyValue">账号主键</param>
        Task ResetPassword(string keyValue);
        /// <summary>
        /// 修改用户状态
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="state">状态：1-启动；0-禁用</param>
        Task UpdateState(string keyValue, int state);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断密码是否正确
        /// </summary>
        /// <param name="oldPassword">旧密码</param>
        /// <param name="newPassWord">新密码</param>
        /// <param name="secretkey">密钥</param>
        /// <returns></returns>
        bool IsPasswordOk(string oldPassword, string newPassWord, string secretkey);
        /// <summary>
        /// 获取用户信息通过CA登录名
        /// </summary>
        /// <param name="causersign">CA登录名</param>
        /// <returns></returns>
        Task<UserEntity> GetEntityByCaUserSign(string causersign);
        #endregion
    }
}
