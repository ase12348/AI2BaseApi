using jcode.iapplication;
using jcode.oss;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.controllers
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-08-19 15:29:43
    /// 描 述： 桌面配置
    /// </summary>
    public class DesktopController : BaseApiController
    {
        private readonly ISettingBLL _iDesktopBLL;
        private readonly IAnnexes _iAnnexes;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iDesktopBLL">接口</param>
        /// <param name="iAnnexes">接口</param>
        public DesktopController(ISettingBLL iDesktopBLL, IAnnexes iAnnexes)
        {
            _iDesktopBLL = iDesktopBLL ?? throw new ArgumentNullException(nameof(iDesktopBLL));
            _iAnnexes = iAnnexes;
        }

        #region 获取数据
        /// <summary>
        /// 获取桌面配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        [HttpGet("desktop/settings")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<Lr_desktop_infoEntity>>), 200)]
        public async Task<IActionResult> GetList([FromQuery] Lr_desktop_infoEntity queryParams)
        {
            var list = await _iDesktopBLL.GetList(queryParams);
            return Success(list);
        }

        /// <summary>
        /// 获取桌面配置的列表数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/page")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<Lr_desktop_infoEntity>>>), 200)]
        public async Task<IActionResult> GetPageList([FromQuery] PaginationInputDto paginationInputDto, [FromQuery] Lr_desktop_infoEntity queryParams, string tags)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var list = await _iDesktopBLL.GetPageList(pagination, queryParams, tags);
            var jsonData = new
            {
                rows = list,
                pagination.total,
                pagination.page,
                pagination.records
            };
            return Success(jsonData);
        }
        /// <summary>
        /// 获取主表桌面配置的表单数据
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto<SettingDto>), 200)]
        public async Task<IActionResult> GetForm(String f_Id)
        {
            var data = new SettingDto();
            data.Lr_desktop_infoEntity = await _iDesktopBLL.GetEntity(f_Id);
            data.Lr_desktop_schemeEntity = await _iDesktopBLL.GetLr_desktop_scheme(data.Lr_desktop_infoEntity.F_SchemeId);
            return Success(data);
        }

        /// <summary>
        /// 获取主表桌面配置的表单数据
        /// </summary>
        /// <param name="id">功能主键</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/release/{id}")]
        [ProducesResponseType(typeof(ResponseDto<SettingDto>), 200)]
        public async Task<IActionResult> GetRelease(string id)
        {
            var data = new SettingDto();
            data.Lr_desktop_infoEntity = await _iDesktopBLL.GetEntityByModuleId(id);
            data.Lr_desktop_schemeEntity = await _iDesktopBLL.GetSchemeEntityByInfoId(data.Lr_desktop_infoEntity.F_Id);
            return Success(data);
        }

        /// <summary>
        /// 获取主表桌面配置的表单数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/bycode/{code}")]
        [ProducesResponseType(typeof(ResponseDto<SettingDto>), 200)]
        public async Task<IActionResult> GetFormByCode(String code)
        {
            var data = new SettingDto();
            data.Lr_desktop_infoEntity = await _iDesktopBLL.GetEntityByCode(code);
            data.Lr_desktop_schemeEntity = await _iDesktopBLL.GetLr_desktop_scheme(data.Lr_desktop_infoEntity.F_SchemeId);
            return Success(data);
        }

        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="id">表lr_desktop_info关联字段F_Id</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/historys/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IEnumerable<Lr_desktop_schemeEntity>>), 200)]
        public async Task<IActionResult> GetLr_desktop_schemeList(String id)
        {
            var data = await _iDesktopBLL.GetLr_desktop_schemeList(id);
            return Success(data);
        }

        /// <summary>
        /// 获取自定义表单模板历史数据
        /// </summary>
        /// <param name="paginationInputDto">分页参数</param>
        /// <param name="id">自定义表单信息主键</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/history/page/{id}")]
        [ProducesResponseType(typeof(ResponseDto<PaginationOutputDto<IEnumerable<Lr_desktop_schemeEntity>>>), 200)]
        public async Task<IActionResult> GetSchemePageList([FromQuery] PaginationInputDto paginationInputDto, string id)
        {
            Pagination pagination = paginationInputDto.ToObject<Pagination>();
            var data = await _iDesktopBLL.GetLr_desktop_schemePage(id, pagination);
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
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="id">表lr_desktop_info关联字段F_Id</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/history/{id}")]
        [ProducesResponseType(typeof(ResponseDto<Lr_desktop_schemeEntity>), 200)]
        public async Task<IActionResult> GetLr_desktop_scheme(String id)
        {
            var data = await _iDesktopBLL.GetLr_desktop_scheme(id);
            return Success(data);
        }
        
        
        /// <summary>
        /// 获取自定义表单设计数据
        /// </summary>
        /// <param name="id">id</param>
        /// <returns></returns>
        [HttpGet("desktop/setting/historyform/{id}")]
        [ProducesResponseType(typeof(ResponseDto<IndexSchemeOutputDto>), 200)]
        public async Task<IActionResult> GetFormByHistory(string id)
        {
            var data = new SettingDto();
            data.Lr_desktop_schemeEntity = await _iDesktopBLL.GetLr_desktop_scheme(id);
            data.Lr_desktop_infoEntity = await _iDesktopBLL.GetEntity(data.Lr_desktop_schemeEntity.F_InfoId);

            return Success(data);
        }
        
        
        /// <summary>
        /// 导出桌面模板数据
        /// </summary>
        /// <param name="path">目录</param>
        /// <param name="id">流程主键</param>
        /// <returns></returns>
        [HttpPost("desktop/setting/download/{path}/{id}")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult> ExportScheme(string path, string id)
        {
            var fileName  =await _iDesktopBLL.Export(id,path);
            return Success(fileName);
        }
        #endregion

        #region 提交数据

        /// <summary>
        /// 新增
        /// </summary>
        /// <param name="desktopDto">实体数据</param>
        [HttpPost("desktop/setting")]
        [ProducesResponseType(typeof(ResponseDto<Lr_desktop_infoEntity>), 200)]
        public async Task<IActionResult> AddEntity(SettingDto desktopDto)
        {
            await _iDesktopBLL.SaveEntity(null, desktopDto.Lr_desktop_infoEntity, desktopDto.Lr_desktop_schemeEntity);
            return Success("新增成功！", desktopDto.Lr_desktop_infoEntity);
        }


        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <param name="desktopDto">实体数据</param>
        [HttpPut("desktop/setting/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateEntity(string f_Id, SettingDto desktopDto)
        {
            await _iDesktopBLL.SaveEntity(f_Id, desktopDto.Lr_desktop_infoEntity, desktopDto.Lr_desktop_schemeEntity);
            return Success("更新成功！", desktopDto.Lr_desktop_infoEntity);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">主键</param>
        [HttpDelete("desktop/setting/{f_Id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> Delete(string f_Id)
        {
            await _iDesktopBLL.Delete(f_Id);
            return SuccessInfo("删除成功！");
        }



        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id">功能主键</param>
        [HttpDelete("desktop/setting/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteByModuleId(string id)
        {
            await _iDesktopBLL.DeleteByModuleId(id);
            return SuccessInfo("删除成功！");
        }

        /// <summary>
        /// 删除lr_desktop_scheme表数据
        /// </summary>
        /// <param name="id">主键</param>
        [HttpDelete("desktop/setting/history/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> DeleteLr_desktop_scheme(string id)
        {
            await _iDesktopBLL.DeleteLr_desktop_scheme(id);
            return SuccessInfo("删除成功！");
        }


        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="schemeId">模版id</param>
        /// <returns></returns>
        [HttpPut("desktop/setting/history/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateEntity(string id,string schemeId)
        {
            await _iDesktopBLL.UpdateEntity(id, schemeId);
            return SuccessInfo("更新成功！");
        }
        
        
        


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabledMark">状态</param>
        /// <returns></returns>
        [HttpPut("desktop/setting/state/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateState(string id, int enabledMark)
        {
            await _iDesktopBLL.UpdateState(id, enabledMark);
            return SuccessInfo("更新成功！");
        }
        
        
        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="moduleId">菜单ID</param>
        /// <param name="isFirst">是否首屏</param>
        /// <returns></returns>
        [HttpPut("desktop/setting/moduleIdAndFirst/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateModuleId(string id, string moduleId, int isFirst)
        {
             await _iDesktopBLL.UpdateEntity(id, moduleId,isFirst);
            return SuccessInfo("更新成功！");
        }




        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="enabledMark"></param>
        /// <returns></returns>
        [HttpPut("desktop/setting/state/module/{id}")]
        [ProducesResponseType(typeof(ResponseDto), 200)]
        public async Task<IActionResult> UpdateStateByModuleId(string id, int enabledMark)
        {
            await _iDesktopBLL.UpdateStateByModuleId(id, enabledMark);
            return SuccessInfo("更新成功！");
        }

        #endregion    
        

    }
}
