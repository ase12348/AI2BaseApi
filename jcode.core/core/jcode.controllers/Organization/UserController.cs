using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.08.24
    /// 描 述：用户模块
    /// </summary>
    public class UserController : BaseApiController
    {
        private readonly UserIBLL _userIBLL;
        private readonly UserRelationIBLL _userRelationIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="userIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public UserController(UserIBLL userIBLL, UserRelationIBLL userRelationIBLL) {
            _userIBLL = userIBLL??throw new ArgumentNullException(nameof(userIBLL));
            _userRelationIBLL = userRelationIBLL ?? throw new ArgumentNullException(nameof(userRelationIBLL));
        }

        /// <summary>
        /// 获取用户分页数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="companyId">公司id</param>
        /// <param name="departmentId">部门id</param>
        /// <param name="departmentDeepNum">部门层级编码</param>
        /// <param name="userType">人员类别</param>
        /// <param name="salaryLevel">待遇级别</param>
        /// <param name="type">账号状态类型</param>
        /// <param name="isBlack">是否是黑名单</param>
        /// <param name="enabledMark">是否获取有效用户</param>
        /// <param name="gender">性别</param>
        /// <param name="ids">用户ids</param>
        /// <param name="isSyn">是否查询企业微信同步状态(前端传true才查询)</param>
        /// <returns></returns>
        [HttpGet("organization/user/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<UserEntity>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, string keyword,string companyId,string departmentId,string departmentDeepNum,string userType,string salaryLevel,int? type,int? isBlack,int? enabledMark,int? gender, string ids, bool isSyn = false)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _userIBLL.GetPageList(companyId, departmentId,departmentDeepNum, pagination, keyword,userType,salaryLevel, type,isBlack,enabledMark,gender, ids);
            var jsonData = new
            {
                rows = data,
                pagination.total,
                pagination.page,
                pagination.records,
            };
            
            return Success(jsonData);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="dto">用户id</param>
        /// <returns></returns>
        [HttpPost("organization/users")]
        [ProducesResponseType(typeof(ResponseDto<UserEntity>), 200)]
        public async Task<IActionResult> GetEntity(IDSDto dto)
        {
            var userEntity = await _userIBLL.GetListByKeyValues(dto.Ids);
            
            return Success(userEntity);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="departmentId">部门id</param>
        /// <returns></returns>
        [HttpGet("organization/users/{departmentId}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<UserEntity>>), 200)]
        public async Task<IActionResult> GetList(string departmentId)
        {
            var data = await _userIBLL.GetList(null, departmentId, null);
            return Success(data);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="keyword">查询关键字</param>
        /// <returns></returns>
        [HttpGet("organization/users")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<UserEntity>>), 200)]
        public async Task<IActionResult> GetList2(string keyword)
        {
            var data = await _userIBLL.GetList(null, null, keyword);
            return Success(data);
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="id">用户id</param>
        /// <returns></returns>
        [HttpGet("organization/user/{id}")]
        [ProducesResponseType(typeof(ResponseDto<UserEntity>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            UserDto dto = new UserDto();
            dto.User = await _userIBLL.GetEntity(id);
            dto.Roles = await _userRelationIBLL.GetObjectIdList(id,1);
            dto.Posts = await _userRelationIBLL.GetObjectIdList(id, 2);
            dto.Departments = await _userRelationIBLL.GetObjectIdList(id, 3);
            dto.Companys = await _userRelationIBLL.GetObjectIdList(id, 4);

            return Success(dto);
        }

        /// <summary>
        /// 新增用户
        /// </summary>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        [HttpPost("organization/user")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> AddForm(UserDto dto)
        {
            await _userIBLL.SaveEntity(string.Empty, dto);
            return Success("保存成功！", dto.User.F_UserId);

        }

        /// <summary>
        /// 更新用户
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">实体</param>
        /// <returns></returns>
        [HttpPut("organization/user/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveForm(string id, UserDto dto)
        {
            await _userIBLL.SaveEntity(id, dto);
            return SuccessInfo("保存成功！");
        }
        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("organization/user/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _userIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }


        /// <summary>
        /// 更新用户状态
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">0 禁用 1  启用</param>
        /// <returns></returns>
        [HttpPut("organization/user/state/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateState(string id,int state)
        {
            await _userIBLL.UpdateState(id, state);
            return SuccessInfo("更新成功！");
        }

        /// <summary>
        /// 重置用户账号密码
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpPut("organization/user/resetpsw/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> ResetPassword(string id)
        {
            await _userIBLL.ResetPassword(id);
            return SuccessInfo("操作成功！");
        }

        /// <summary>
        /// 导出用户列表
        /// </summary>
        /// <returns></returns>
        [HttpPost("organization/users/export")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> ExportUserList()
        {
            var id = await _userIBLL.GetExportList();
            return Success(id);
        }
        
        /// <summary>
        /// 验证密码
        /// </summary>
        /// <param name="psw"></param>
        /// <returns></returns>
        [HttpGet("organization/user/validator/{psw}")]
        [ProducesResponseType(typeof(ResponseDtoBool), 200)]
        public async Task<IActionResult> ValidatorPassword(string psw)
        {
            var userInfo = await CurrentUser();
            psw = Md5Helper.Encrypt(DESEncrypt.Encrypt(psw, userInfo.F_Secretkey).ToLower(), 32).ToLower();
            if (psw != userInfo.F_Password)
            {
                return Success("密码错误", false);
            }
            else
            {
                return Success("密码正确", true);
            }
        }

        /// <summary>
        /// 提交修改密码
        /// </summary>
        /// <param name="inputDto">提交参数</param>
        /// <returns></returns>
        [HttpPut("organization/user/password")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdatePassword(UserUpdatePswInputDto inputDto)
        {
            bool res = await _userIBLL.RevisePassword(inputDto.password, inputDto.oldPassword);
            if (!res)
            {
                return Fail("原密码错误");
            }

            return SuccessInfo("修改成功。");
        }
        
        
                
        /// <summary>
        /// 心跳包，检测用户在线状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("organization/user/heart")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public IActionResult GetHeart()
        {
            return SuccessInfo("");
        }
        
        /// <summary>
        /// 获取在线用户列表
        /// </summary>
        /// <param name="paginationInputDto"></param>
        /// <param name="keyword"></param>
        /// <returns></returns>
        [HttpGet("organization/user/onlines")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<OnlineUser>>), 200)]
        public IActionResult GetOnlines([FromQuery] PaginationInputDto paginationInputDto, string keyword)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = OnlineUserHelper.GetList(pagination,keyword);
            var jsonData = new
            {
                rows = list,
                pagination.total,
                pagination.page,
                pagination.records,
            };
            
            return Success(jsonData);
        }
    }
}
