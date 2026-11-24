using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace jcode.controllers.Permission
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.13
    /// 描 述：功能权限
    /// </summary>
    public class ModuleController : BaseApiController
    {
        private readonly AuthorizeIBLL _authorizeIBLL;
        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly LogIBLL _logIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="authorizeIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="logIBLL"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public ModuleController(AuthorizeIBLL authorizeIBLL, UserRelationIBLL userRelationIBLL,LogIBLL logIBLL)
        {
            _authorizeIBLL = authorizeIBLL?? throw new ArgumentNullException(nameof(authorizeIBLL));
            _userRelationIBLL = userRelationIBLL ?? throw new ArgumentNullException(nameof(userRelationIBLL));
            _logIBLL = logIBLL ?? throw new ArgumentNullException(nameof(logIBLL));
        }
        
        /// <summary>
        /// 获取设置信息
        /// </summary>
        /// <param name="id">设置对象:角色或人员</param>
        /// <param name="type">类型 1 模块 2 按钮 3 列 4 表单 5 移动端功能</param>
        /// <param name="parentId">父级id</param>
        /// <returns></returns>
        [HttpGet("permission/module/{type}/{parentId}/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<string>>), 200)]
        public async Task<IActionResult> GetData(string id, int type,string parentId)
        {
            AuthorizeSettingDto res = new AuthorizeSettingDto();
            res.SettingList = await _authorizeIBLL.GetList(id, type, parentId);
            if (type != 1) {
                var user = await this.CurrentUser();
                if (user.F_SecurityLevel != 1 && user.F_SecurityLevel != 5)// 超级管理员
                {
                    var roleIds = await _userRelationIBLL.GetObjectIds(user.F_UserId, 1);
                    roleIds.Add(user.F_UserId);
                    res.MyList = await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), type, parentId);
                }
            }
            return Success(res);
        }

        /// <summary>
        /// 获取功能页面权限
        /// </summary>
        /// <param name="id">对象Id</param>
        /// <param name="name">功能名称</param>
        /// <returns></returns>
        [HttpGet("permission/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto<AuthorizeModuleDto>), 200)]
        public async Task<IActionResult> GetModule(string id,string name)
        {
            // 记录访问权限
            Stopwatch stopwatch = CommonHelper.TimerStart();
            
            #region 写入日志
            LogEntity logEntity = new LogEntity();
            logEntity.F_CategoryId = 2;
            logEntity.F_ExecuteResult = 1;
            logEntity.F_ExecuteResultJson = "访问:" + name;
           
            #endregion
            
            AuthorizeModuleDto res = new AuthorizeModuleDto();
            var user = await this.CurrentUser();
            if (user.F_SecurityLevel == 1 || user.F_SecurityLevel == 5)
            {
                _logIBLL.Write(logEntity,stopwatch);
                return Success(res);
            }

            var roleIds = await _userRelationIBLL.GetRoleIds();
            roleIds.Add(user.F_UserId);
            
            var buttons =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 2, id);
            var notButtons =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 20, id);
            res.Buttons = buttons.FindAll(t => notButtons.FindIndex(t2 => t2 == t) == -1);
            
            var columns =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 3, id);
            var notColumns =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 30, id);
            res.Columns = columns.FindAll(t => notColumns.FindIndex(t2 => t2 == t) == -1);
            
            var forms =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 4, id);
            var notForms =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 40, id);
            
            res.Forms = forms.FindAll(t => notForms.FindIndex(t2 =>
            {
                var idList1 = t.Split(',');
                var id1 = idList1[0];
                var type1 = idList1[1];
                var idList2 = t2.Split(',');
                var id2 = idList2[0];
                var type2 = idList2[1];
                if (type2 == "1")
                {
                    return id == id2;
                }
                else
                {
                    return id1 == id2 && type1 == "2";
                }
            }) == -1);
            

            _logIBLL.Write(logEntity,stopwatch);
            return Success(res);
        }


        /// <summary>
        /// 获取移动端功能页面权限
        /// </summary>
        /// <param name="id">对象Id</param>
        /// <returns></returns>
        [HttpGet("permission/module/app/{id}")]
        [ProducesResponseType(typeof(ResponseDto<AuthorizeModuleDto>), 200)]
        public async Task<IActionResult> GetAppModule(string id)
        {
            var user = await this.CurrentUser();
            var roleIds = await _userRelationIBLL.GetObjectIds(user.F_UserId, 1);
            roleIds.Add(user.F_UserId);
            AuthorizeModuleDto res = new AuthorizeModuleDto();
            
             
            var buttons =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 6, id);
            var notButtons =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 60, id);
            res.Buttons = buttons.FindAll(t => notButtons.FindIndex(t2 => t2 == t) == -1);
            
            var columns =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 7, id);
            var notColumns =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 70, id);
            res.Columns = columns.FindAll(t => notColumns.FindIndex(t2 => t2 == t) == -1);
            
            var forms =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(roleIds.ToArray(), 8, id);
            var notForms =(List<string>) await _authorizeIBLL.GetItemIdListByobjectIds(user.F_UserId.Split(','), 80, id);
           
            res.Forms = forms.FindAll(t => notForms.FindIndex(t2 =>
            {
                var idList1 = t.Split(',');
                var id1 = idList1[0];
                var type1 = idList1[1];
                var idList2 = t2.Split(',');
                var id2 = idList2[0];
                var type2 = idList2[1];
                if (type2 == "1")
                {
                    return id == id2;
                }
                else
                {
                    return id1 == id2 && type1 == "2";
                }
            }) == -1);

            return Success(res);
        }

        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="id">对象Id</param>
        /// <param name="authorizeDto">提交信息</param>
        /// <returns></returns>
        [HttpPost("permission/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveForm(string id, AuthorizeDto authorizeDto)
        {
            await _authorizeIBLL.SaveAuthorize(id, authorizeDto);
            return Success("保存成功！");
        }


        
    }
}
