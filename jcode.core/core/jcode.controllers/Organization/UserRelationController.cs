using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers.Organization
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.13
    /// 描 述：用户组（角色，岗位）
    /// </summary>
    public class UserRelationController : BaseApiController
    {
        private readonly UserRelationIBLL _userRelationIBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="userRelationIBLL"></param>
        public UserRelationController(UserRelationIBLL userRelationIBLL)
        {
            _userRelationIBLL = userRelationIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取角色或岗位成员
        /// </summary>
        /// <param name="id">角色或岗位主键</param>
        /// <returns></returns>
        [HttpGet("organization/userrelation/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<UserRelationEntity>>), 200)]
        public async Task<IActionResult> GetUserIdList(string id)
        {
            var data = await _userRelationIBLL.GetUserIdList(id);
            return Success(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="id">对象主键</param>
        /// <param name="type">分类:1-角色2-岗位</param>
        /// <param name="userRelationInputDto">对用户主键列表</param>
        /// <returns></returns>
        [HttpPost("organization/userrelation/{type}/{id}")]
        public async Task<IActionResult> SaveForm(string id, int type, UserRelationInputDto userRelationInputDto)
        {
            await _userRelationIBLL.SaveEntityList(id, type, userRelationInputDto.UserIds);
            return SuccessInfo("保存成功！");
        }
        
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="id">对象主键</param>
        /// <param name="list">对用户主键列表</param>
        /// <returns></returns>
        [HttpPost("organization/userrelation/role/{id}")]
        public async Task<IActionResult> SaveRoleForm(string id, List<UserRelationEntity> list)
        {
            await _userRelationIBLL.SaveEntityList(id, 1, list);
            return SuccessInfo("保存成功！");
        }
        #endregion
    }
}
