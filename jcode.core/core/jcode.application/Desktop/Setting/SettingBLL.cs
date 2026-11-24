using jcode.autofac;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using jcode.oss;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-08-19 15:29:43
    /// 描 述： 桌面配置
    /// </summary>
    public class SettingBLL : BLLBase, ISettingBLL,BLL
    {
        private readonly SettingService desktopService = new SettingService();
        private readonly ModuleIBLL _moduleIBLL;
        private readonly FunctionIBLL _functionIBLL;

        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly CompanyIBLL _companyIBLL;
        private readonly DepartmentIBLL _departmentIBLL;
        private readonly AnnexesFileIBLL _annexesFileIBLL;
        
        private readonly IAnnexes _iAnnexes;
        

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="moduleIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="companyIBLL"></param>
        /// <param name="departmentIBLL"></param>
        /// <param name="functionIBLL"></param>
        /// <param name="annexesFileIBLL"></param>
        /// <param name="iAnnexes"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public SettingBLL(
            ModuleIBLL moduleIBLL, 
            UserRelationIBLL userRelationIBLL,
            CompanyIBLL companyIBLL, 
            DepartmentIBLL departmentIBLL, 
            FunctionIBLL functionIBLL,
            AnnexesFileIBLL annexesFileIBLL,
            IAnnexes iAnnexes
            )
        {
            _moduleIBLL = moduleIBLL ?? throw new ArgumentNullException(nameof(moduleIBLL));
            _userRelationIBLL = userRelationIBLL;
            _companyIBLL = companyIBLL;
            _departmentIBLL = departmentIBLL;
            _functionIBLL = functionIBLL;
            
            _annexesFileIBLL = annexesFileIBLL;
            
            _iAnnexes = iAnnexes;
        }
        #region 获取数据
        /// <summary>
        /// 获取桌面配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public async Task<IEnumerable<Lr_desktop_infoEntity>> GetList(Lr_desktop_infoEntity queryParams)
        {
            var list = (List<Lr_desktop_infoEntity>)await desktopService.GetList(queryParams);
            
            return  list.FindAll(t=>!t.F_ReleaseId.IsEmpty() && !t.F_ModuleId.IsEmpty());
        }


        /// <summary>
        /// 获取桌面配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_infoEntity>> GetPageList(Pagination pagination, Lr_desktop_infoEntity queryParams, string tags = null)
        {
            return desktopService.GetPageList(pagination, queryParams, tags);
        }



        
        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntity(String f_Id)
        {
            return desktopService.GetEntity(f_Id);
        }

        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntityByCode(String code) {
            return desktopService.GetEntityByCode(code);
        }

        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="moduleId">菜单id</param>
        /// <returns></returns>
        public Task<Lr_desktop_infoEntity> GetEntityByModuleId(String moduleId)
        {
            return desktopService.GetEntityByModuleId(moduleId);
        }
        
        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        public Task<Lr_desktop_schemeEntity> GetLr_desktop_scheme(String f_Id)
        {
            return desktopService.GetLr_desktop_scheme(f_Id);
        }
        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemeList(String f_InfoId)
        {

            return desktopService.GetLr_desktop_schemeList(f_InfoId);
        }


        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<Lr_desktop_schemeEntity> GetSchemeEntityByInfoId(string keyValue)
        {
            return desktopService.GetSchemeEntityByInfoId(keyValue);
        }

        /// <summary>
        /// 获取表lr_desktop_scheme的分页列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        public Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemePage(String f_InfoId, Pagination pagination)
        {
            return desktopService.GetLr_desktop_schemePage(f_InfoId, pagination);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">主键</param>
        public async Task Delete(string f_Id)
        {
            var entity = await GetEntity(f_Id);
            if (entity != null)
            {
                await Delete(entity);
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="moduleId">菜单id</param>
        public async Task DeleteByModuleId(string moduleId)
        {
            var entity = await GetEntityByModuleId(moduleId);
            if (entity != null)
            {
                await Delete(entity);
            }
            else {
                await _moduleIBLL.Delete(moduleId);
                await _functionIBLL.Delete(moduleId);
            }
        }

        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task Delete(Lr_desktop_infoEntity entity)
        {
            desktopService.BeginTrans();
            try
            {
                await desktopService.Delete(entity.F_Id);
                await _annexesFileIBLL.DeleteEntity(entity.F_SchemeId);
                desktopService.Commit();
                
            }
            catch (Exception)
            {
                desktopService.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 删除lr_desktop_scheme表数据
        /// </summary>
        /// <param name="f_Id">主键</param>
        public async Task DeleteLr_desktop_scheme(String f_Id)
        {
            await desktopService.DeleteLr_desktop_scheme(f_Id);
        }


        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <param name="lr_desktop_infoEntity">lr_desktop_info实体数据</param>
        /// <param name="lr_desktop_schemeEntity">lr_desktop_scheme实体数据</param>
        public async Task SaveEntity(String f_Id,Lr_desktop_infoEntity lr_desktop_infoEntity, Lr_desktop_schemeEntity lr_desktop_schemeEntity)
        {
            if (!string.IsNullOrEmpty(lr_desktop_infoEntity.F_SchemeId))
            {
                var schemeOldEntity = await desktopService.GetLr_desktop_scheme(lr_desktop_infoEntity.F_SchemeId);
                if (schemeOldEntity == null || schemeOldEntity.F_Type != lr_desktop_schemeEntity.F_Type || schemeOldEntity.F_Ver != lr_desktop_schemeEntity.F_Ver || schemeOldEntity.F_Content != lr_desktop_schemeEntity.F_Content)
                {
                    await desktopService.SaveEntity(f_Id, lr_desktop_infoEntity, lr_desktop_schemeEntity);
                }
                else
                {
                    await desktopService.SaveEntity(f_Id, lr_desktop_infoEntity, null);
                }
            }
            else {
                //新增
                await desktopService.SaveEntity(f_Id, lr_desktop_infoEntity, lr_desktop_schemeEntity);
            }
            
            
            // 保存图片
            if (!string.IsNullOrEmpty(lr_desktop_infoEntity.Img))
            {
                var imgContent = lr_desktop_infoEntity.Img.Replace("data:image/png;base64,", "");
                lr_desktop_infoEntity.Img = "";
                byte[] arr = Convert.FromBase64String(imgContent);
                await _annexesFileIBLL.SaveAnnexes(
                    lr_desktop_infoEntity.F_SchemeId,
                    lr_desktop_infoEntity.F_SchemeId, 
                    $"BI_{lr_desktop_infoEntity.F_Name}.png", 
                    arr, 
                    "", 
                    "lr_desktop_info", 
                    "F_SchemeId");
            }


           
        }
        


        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        public async Task UpdateEntity(string id, string schemeId)
        {
            await desktopService.UpdateEntity(new Lr_desktop_infoEntity() {F_Id= id,F_SchemeId = schemeId });
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="enabledMark">状态码</param>
        /// <returns></returns>
        public async Task UpdateState(string id, int enabledMark)
        {
            
            await desktopService.UpdateEntity(new Lr_desktop_infoEntity() {F_Id= id,F_EnabledMark = enabledMark });
        }
        
        /// <summary>
        /// 更新菜单id 和 是否首屏
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="moduleId">菜单ID</param>
        /// <param name="isFirst">是否首屏</param>
        /// <returns></returns>
        public async Task UpdateEntity(string id, string moduleId,int isFirst)
        {
            
            await desktopService.UpdateEntity(new Lr_desktop_infoEntity() {F_Id= id,F_ModuleId = moduleId,F_IsFirst = isFirst});
        }



        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <param name="enabledMark">状态码</param>
        /// <returns></returns>
        public async Task UpdateStateByModuleId(string moduleId, int enabledMark)
        {
            var entity =await GetEntityByModuleId(moduleId);
            if (entity != null) {
                entity.F_EnabledMark = enabledMark;
                await UpdateState(entity);
            }
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        private async Task UpdateState(Lr_desktop_infoEntity entity)
        {
            desktopService.BeginTrans();
            try
            {
                await desktopService.UpdateEntity(entity);
                if (entity.F_Type == 1)
                {
                    await _moduleIBLL.UpdateState(entity.F_ModuleId, (int)entity.F_EnabledMark);
                }
                else {
                    await _functionIBLL.UpdateState(entity.F_ModuleId, (int)entity.F_EnabledMark);
                }

                desktopService.Commit();
            }
            catch (Exception)
            {
                desktopService.Rollback();
                throw;
            }
        }




        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取跳转条件的sql语句
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> GetSql(string param) {
            var paramList = param.Split("|");
            var code = paramList[0];
            var id = paramList[1];

            var entity = await desktopService.GetEntityByCode(code);
            var lr_desktop_schemeEntity =await desktopService.GetLr_desktop_scheme(entity.F_SchemeId);
            var layouts = lr_desktop_schemeEntity.F_Content.ToObject<List<LayoutModel>>();
            var layout = layouts.Find(t => t.I == id);

            string strSql = "";
            //条件信息
            if (layout.Config.ConditionInfo!=null)
            {
                if (!string.IsNullOrEmpty(layout.Config.ConditionInfo.Formula))
                {
                    strSql = layout.Config.ConditionInfo.Formula;
                    for (int i = 1; i < layout.Config.ConditionInfo.Conditions.Count + 1; i++)
                    {
                        strSql = strSql.Replace("" + i, "{@jcode" + i + "jcode@}");
                    }
                }
                else
                {
                    for (int i = 1; i < layout.Config.ConditionInfo.Conditions.Count + 1; i++)
                    {
                        if (strSql != "")
                        {
                            strSql += " AND ";
                        }
                        strSql += " {@jcode" + i + "jcode@} ";
                    }
                }
                int num = 1;
                foreach (var conditionItem in layout.Config.ConditionInfo.Conditions)
                {
                    var strone = new StringBuilder();
                    if (conditionItem.Type == "glbd")
                    {
                        strone.Append($" {conditionItem.RelationField} in ( SELECT {conditionItem.Field} FROM {conditionItem.Table} WHERE {conditionItem.Cfield}  ");
                    }
                    else
                    {
                        strone.Append($" {conditionItem.Prop}");
                    }


                    string value = await GetValue(conditionItem.ValueType, conditionItem.FiledValue);

                    switch (conditionItem.Symbol)
                    {
                        case 1:// 等于
                            strone.Append($" = '{value}'");
                            break;
                        case 2:// 大于
                            strone.Append($" > {value}");
                            break;
                        case 3:// 大于等于
                            strone.Append($" >= {value}");
                            break;
                        case 4:// 小于
                            strone.Append($" < {value}");
                            break;
                        case 5:// 小于等于
                            strone.Append($" <= {value}");
                            break;
                        case 6:// 包含
                            strone.Append($" like '%{value}%'");
                            break;
                        case 7:// 包含于
                            strone.Append($" in ({value})");
                            break;
                        case 8:// 不等于
                            strone.Append($" != '{value}'");
                            break;
                        case 9:// 不包含
                            strone.Append($" not like '%{value}%'");
                            break;
                        case 10:// 不包含于
                            strone.Append($" not in ({value})");
                            break;
                        default:
                            break;
                    }
                    if (conditionItem.Type == "glbd")
                    {
                        strone.Append(" ) ");
                    }
                    else
                    {
                        strone.Append(" ");
                    }

                    strSql = strSql.Replace("{@jcode" + num + "jcode@}", strone.ToString());
                    num++;
                }
            }
            else
            {
                strSql = " 1=1 ";
            }
            return strSql;

        }


        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="type">数据类型</param>
        /// <param name="value">数据值</param>
        /// <returns></returns>
        private async Task<string> GetValue(int? type, string value)
        {
            var userInfo = await this.CurrentUser();
            
            //1.文本2.登录者ID3.登录者账号4.登录者公司5.登录者部门6.登录者岗位7.登录者角色
            string text;
            switch (type)
            {
                case 1:// 文本
                    text = value;
                    break;
                case 2:// 登录者ID
                    text = userInfo.F_UserId;
                    break;
                case 3:// 登录者账号
                    text = userInfo.F_Account;
                    break;
                case 4:// 登录者公司
                    text = userInfo.F_CompanyId;
                    break;
                case 41:// 登录者公司及下属公司
                    text = "'" + string.Join("','", await _companyIBLL.GetSubNodes(userInfo.F_CompanyId)) + "'";
                    break;
                case 5:// 登录者部门
                    text = userInfo.F_DepartmentId;
                    break;
                case 51:// 登录者部门及下属部门
                    var departmentList = new List<string>();
                    departmentList.AddRange(await _userRelationIBLL.GetObjectIds(userInfo.F_UserId, 3));
                    departmentList.AddRange(await _departmentIBLL.GetSubNodes(userInfo.F_DepartmentId));
                    text = "'" + string.Join("','", departmentList) + "'";
                    break;
                case 6:// 登录者岗位
                    text = "'" + string.Join("','", await this.CurrentUserPostIds() + "'");
                    break;
                case 7:// 登录者角色
                    text = "'" + string.Join("','", await this.CurrentUserRoleIds() + "'");
                    break;
                default:
                    text = value;
                    break;
            }
            return text;
        }
        #endregion

        #region 导入导出方法
        /// <summary>
        /// 导出流程模版
        /// </summary>
        /// <param name="keyValue">流程主键</param>
        /// <param name="path">目录</param>
        /// <returns></returns>
        public async Task<string> Export(string keyValue,string path)
        {
            var data = new SettingDto();
            data.Lr_desktop_infoEntity = await GetEntity(keyValue);
            data.Lr_desktop_schemeEntity = await GetLr_desktop_scheme(data.Lr_desktop_infoEntity.F_SchemeId);


            var fileName = Guid.NewGuid().ToString();
            // 写入一个临时文件
            _iAnnexes.UploadFile(fileName,$"{path}/{fileName}.json", ".json", Encoding.UTF8.GetBytes(data.ToJson()),0);
            return fileName;
        }
        
        
        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="jsonData"></param>
        public async Task Import(string jsonData)
        {
            SettingDto dto = jsonData.ToObject<SettingDto>();
            dto.Lr_desktop_infoEntity.F_Id = "";
            var schemeInfo = await GetEntityByCode(dto.Lr_desktop_infoEntity.F_Code);
            string keyValue = schemeInfo?.F_Id;
            await SaveEntity(keyValue, dto.Lr_desktop_infoEntity, dto.Lr_desktop_schemeEntity);
        }

        

        #endregion
        

    }
}
