using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.controllers.MApp
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.10.16
    /// 描 述：移动功能管理
    /// </summary>
    public class ModuleController : BaseApiController
    {
        private readonly FunctionIBLL _functionIBLL;
        private readonly MyFunctionIBLL _myFunctionIBLL;
        private readonly ModuleIBLL _moduleIBLL;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="myFunctionIBLL">我的功能</param>
        /// <param name="functionIBLL">移动端功能</param>
        /// <param name="moduleIBLL">功能模块（统一菜单表）</param>
        public ModuleController(MyFunctionIBLL myFunctionIBLL, FunctionIBLL functionIBLL, ModuleIBLL moduleIBLL)
        {
            _myFunctionIBLL = myFunctionIBLL;
            _functionIBLL = functionIBLL;
            _moduleIBLL = moduleIBLL;
        }

        /// <summary>
        /// 取移动端菜单（lr_base_module 中 F_TerminalType=1 的记录）
        /// </summary>
        private async Task<List<ModuleEntity>> GetMobileModules()
        {
            var list = await _moduleIBLL.GetModuleList();
            return list
                .Where(t => (t.F_TerminalType ?? 0) == 1 && (t.F_DeleteMark ?? 0) == 0)
                .OrderBy(t => t.F_SortCode)
                .ToList();
        }


        #region 获取数据
        /// <summary>
        /// 获取我的常用应用数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("mapp/module/mylist")]
        public async Task<IActionResult> MyList()
        {

            var list = await _myFunctionIBLL.GetList(this.GetUserId());
            var mobileModules = await GetMobileModules();
            var mobileIds = mobileModules.Select(t => t.F_ModuleId).ToList();
            // 只保留仍存在的移动端菜单；用户从未配置过常用时默认展示全部移动端菜单
            List<string> res = list
                .Select(t => t.F_FunctionId)
                .Where(id => mobileIds.Contains(id))
                .ToList();
            if (res.Count == 0)
            {
                res = mobileIds;
            }
            return Success(res);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("mapp/modules")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<FunctionEntity>>), 200)]
        public async Task<IActionResult> GetList()
        {
            // 移动端菜单统一取功能模块表中终端类型为移动端的记录
            var modules = await GetMobileModules();
            var data = modules.Select(t => new FunctionEntity
            {
                F_Id = t.F_ModuleId,
                F_Type = t.F_Target,
                F_Icon = t.F_Icon,
                F_Name = t.F_FullName,
                F_Url = t.F_UrlAddress,
                F_SortCode = t.F_SortCode,
                F_EnabledMark = t.F_EnabledMark,
                F_IsSystem = 0,
                F_AppCode = t.F_AppCode,
                F_TenantId = t.F_TenantId,
                F_CreateDate = t.F_CreateDate,
                F_CreateUserId = t.F_CreateUserId,
                F_CreateUserName = t.F_CreateUserName,
            }).ToList();
            return Success(data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("mapp/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<FunctionInputDto>>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            FunctionInputDto dto = new FunctionInputDto();
            var data = await _functionIBLL.GetEntity(id);
            if (data == null) {
                return Fail("获取失败");
            }
            if (data.F_IsSystem == 2 && !string.IsNullOrEmpty(data.F_SchemeId))
            {
                var scheme = await _functionIBLL.GetScheme(data.F_SchemeId);
                data.Scheme = scheme.F_Scheme;
            }
            else if (data.F_IsSystem == 1)
            {
                
                dto.ModuleItemEntitys = (List<ModuleItemEntity>)await _functionIBLL.GetModuleItemList(id);
            }
            dto.FunctionEntity = data;

            return Success(dto);
        }


        /// <summary>
        /// 获取子集数据
        /// </summary>
        /// <param name="id">功能主键</param>
        /// <param name="type">类型 0 全部 1按钮 2列表 3 表单</param>
        /// <returns></returns>
        [HttpGet("mapp/module/children/{type}/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<FunctionInputDto>>), 200)]
        public async Task<IActionResult> GetModuleItemList(string id,int type)
        {
            var list = (List<ModuleItemEntity>)await _functionIBLL.GetModuleItemList(id);
            if (type == 0)
            {
                return Success(list);
            }
            else {
                return Success(list.FindAll(t=>t.F_Type == type));
            }
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpDelete("mapp/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteForm(string id)
        {
            await _functionIBLL.Delete(id);
            return SuccessInfo("删除成功！");
        }
        /// <summary>
        /// 新增移动功能
        /// </summary>
        /// <param name="dto">提交对象信息</param>
        /// <returns></returns>
        [HttpPost("mapp/module")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> SaveForm(FunctionInputDto dto)
        {
            await _functionIBLL.SaveEntity(string.Empty, dto.FunctionEntity,dto.ModuleItemEntitys);
            return Success("保存成功！",dto.FunctionEntity.F_Id);
        }

        /// <summary>
        /// 更新移动功能
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="dto">提交对象信息</param>
        /// <returns></returns>
        [HttpPut("mapp/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateForm(string id, FunctionInputDto dto)
        {
            await _functionIBLL.SaveEntity(id, dto.FunctionEntity, dto.ModuleItemEntitys);
            return Success("更新成功！",dto.FunctionEntity.F_Id);
        }
        /// <summary>
        /// 启用/停用表单
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        [HttpPut("mapp/module/{id}/{state}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpDateSate(string id, int state)
        {
            await _functionIBLL.UpdateState(id, state);
            return SuccessInfo((state == 1 ? "启用" : "禁用") + "成功！");
        }

        /// <summary>
        /// 更新我的常用应用
        /// </summary>
        /// <param name="dto">功能id集合字串</param>
        /// <returns></returns>
        [HttpPut("mapp/module/mylist")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateMyList(IDSDto dto)
        {
            await _myFunctionIBLL.SaveEntity(this.GetUserId(), dto.Ids);
            return SuccessInfo("保存成功");
        }
        #endregion
    }
}
