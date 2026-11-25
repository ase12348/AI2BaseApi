using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
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
        private readonly ModuleIBLL _moduleIBLL;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="moduleIBLL">功能模块（统一菜单表）</param>
        public ModuleController(ModuleIBLL moduleIBLL)
        {
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

        /// <summary>
        /// 将移动端功能方案（F_Scheme）中的按钮/列表字段/表单字段转换为子项集合
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <param name="scheme">方案JSON：{"btns":[],"columns":[],"formFields":[]}</param>
        private static List<ModuleItemEntity> BuildSchemeItems(string moduleId, string scheme)
        {
            var res = new List<ModuleItemEntity>();
            if (string.IsNullOrWhiteSpace(scheme))
            {
                return res;
            }
            JObject obj;
            try
            {
                obj = scheme.ToJObject();
            }
            catch
            {
                return res;
            }
            if (obj == null)
            {
                return res;
            }
            void Fill(string key, int type)
            {
                if (!(obj[key] is JArray arr))
                {
                    return;
                }
                int sort = 1;
                foreach (var item in arr)
                {
                    string code = item["prop"]?.ToString();
                    string name = item["label"]?.ToString();
                    if (string.IsNullOrWhiteSpace(code) && string.IsNullOrWhiteSpace(name))
                    {
                        continue;
                    }
                    res.Add(new ModuleItemEntity
                    {
                        F_Id = code,
                        F_ModuleId = moduleId,
                        F_Type = type,
                        F_Code = code,
                        F_Name = string.IsNullOrWhiteSpace(name) ? code : name,
                        F_SortCode = sort++,
                    });
                }
            }
            Fill("btns", 1);
            Fill("columns", 2);
            Fill("formFields", 3);
            return res;
        }


        #region 获取数据
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
                F_Type = t.F_Type,
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
                Scheme = t.F_Scheme,
            }).ToList();
            return Success(data);
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <returns></returns>
        [HttpGet("mapp/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto<FunctionInputDto>), 200)]
        public async Task<IActionResult> GetForm(string id)
        {
            // 移动端菜单统一取功能模块表，按钮/列表字段/表单字段取功能方案
            var module = await _moduleIBLL.GetModuleEntity(id);
            if (module == null)
            {
                return Fail("获取失败");
            }
            var dto = new FunctionInputDto
            {
                FunctionEntity = new FunctionEntity
                {
                    F_Id = module.F_ModuleId,
                    F_Type = module.F_Type,
                    F_Icon = module.F_Icon,
                    F_Name = module.F_FullName,
                    F_Url = module.F_UrlAddress,
                    F_EnabledMark = module.F_EnabledMark,
                    F_SortCode = module.F_SortCode,
                    F_IsSystem = 0,
                    F_CreateDate = module.F_CreateDate,
                    F_CreateUserId = module.F_CreateUserId,
                    F_CreateUserName = module.F_CreateUserName,
                    Scheme = module.F_Scheme,
                },
                ModuleItemEntitys = BuildSchemeItems(id, module.F_Scheme),
            };
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
            // 移动端菜单（lr_base_module）的按钮/列表字段/表单字段保存在 F_Scheme 中
            var module = await _moduleIBLL.GetModuleEntity(id);
            var list = module == null
                ? new List<ModuleItemEntity>()
                : BuildSchemeItems(id, module.F_Scheme);
            if (type == 0)
            {
                return Success(list);
            }
            return Success(list.FindAll(t => t.F_Type == type));
        }

        #endregion
    }
}
