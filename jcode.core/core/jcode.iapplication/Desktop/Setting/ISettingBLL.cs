using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-OA模块
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-08-19 15:29:43
    /// 描 述： 桌面配置
    /// </summary>
    public interface ISettingBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取桌面配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<Lr_desktop_infoEntity>> GetList(Lr_desktop_infoEntity queryParams);

        /// <summary>
        /// 获取桌面配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        Task<IEnumerable<Lr_desktop_infoEntity>> GetPageList(Pagination pagination, Lr_desktop_infoEntity queryParams, string tags = null);

        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        Task<Lr_desktop_infoEntity> GetEntity(String f_Id);

        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<Lr_desktop_infoEntity> GetEntityByCode(String code);
        /// <summary>
        /// 获取主表lr_desktop_info的实体
        /// </summary>
        /// <param name="moduleId">菜单id</param>
        /// <returns></returns>
        Task<Lr_desktop_infoEntity> GetEntityByModuleId(String moduleId);
        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <returns></returns>
        Task<Lr_desktop_schemeEntity> GetLr_desktop_scheme(String f_Id);

        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<Lr_desktop_schemeEntity> GetSchemeEntityByInfoId(string keyValue);
        /// <summary>
        /// 获取表lr_desktop_scheme的列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <returns></returns>
        Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemeList(String f_InfoId);


        /// <summary>
        /// 获取表lr_desktop_scheme的分页列表
        /// </summary>
        /// <param name="f_InfoId">关联键</param>
        /// <param name="pagination">分页参数</param>
        /// <returns></returns>
        Task<IEnumerable<Lr_desktop_schemeEntity>> GetLr_desktop_schemePage(String f_InfoId, Pagination pagination);

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="f_Id">主键</param>
        Task Delete(String f_Id);
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="moduleId">菜单id</param>
        Task DeleteByModuleId(string moduleId);

        /// <summary>
        /// 删除lr_desktop_scheme表数据
        /// </summary>
        /// <param name="f_Id">主键</param>
        Task DeleteLr_desktop_scheme(String f_Id);


        /// <summary>
        /// 保存(新增,更新)
        /// </summary>
        /// <param name="f_Id">主键</param>
        /// <param name="lr_desktop_infoEntity">lr_desktop_info实体数据</param>
        /// <param name="lr_desktop_schemeEntity">lr_desktop_scheme实体数据</param>
        Task SaveEntity(String f_Id, Lr_desktop_infoEntity lr_desktop_infoEntity, Lr_desktop_schemeEntity lr_desktop_schemeEntity);


        /// <summary>
        /// 更新版本
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="schemeId"></param>
        /// <returns></returns>
        Task UpdateEntity(string id, string schemeId);

        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="enabledMark">状态码</param>
        /// <returns></returns>
        Task UpdateState(string id, int enabledMark);

        /// <summary>
        /// 更新菜单id 和 是否首屏
        /// </summary>
        /// <param name="id">信息主键</param>
        /// <param name="moduleId">菜单ID</param>
        /// <param name="isFirst">是否首屏</param>
        /// <returns></returns>
        Task UpdateEntity(string id, string moduleId, int isFirst);
        
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="moduleId">功能主键</param>
        /// <param name="enabledMark">状态码</param>
        /// <returns></returns>
        Task UpdateStateByModuleId(string moduleId, int enabledMark);


        #endregion
        
        #region 导入导出方法

        /// <summary>
        /// 导出流程模版
        /// </summary>
        /// <param name="keyValue">流程主键</param>
        /// <param name="path">目录</param>
        /// <returns></returns>
        Task<string> Export(string keyValue, string path);


        /// <summary>
        /// 导入数据
        /// </summary>
        /// <param name="jsonData"></param>
        Task Import(string jsonData);
        
        #endregion

        /// <summary>
        /// 获取跳转条件的sql语句
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        Task<string> GetSql(string param);
        
        
    }
}
