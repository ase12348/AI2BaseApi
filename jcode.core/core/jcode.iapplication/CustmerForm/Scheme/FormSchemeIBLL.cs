using jcode.autofac;
using jcode.util;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.21
    /// 描 述：表单模板
    /// </summary>
    public interface FormSchemeIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取自定义表单列表(启用)
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<FormSchemeInfoEntity>> GetCustmerSchemeInfoList();
        /// <summary>
        /// 获取表单分页列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">关键字</param>
        /// <param name="category">分类（保留参数兼容既有调用，界面已不再使用）</param>
        /// <param name="isEnabled">是否获取正式表单</param>
        /// <param name="appId">所属应用主键（为空表示不过滤）</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        Task<IEnumerable<FormSchemeInfoEntity>> GetSchemeInfoPageList(Pagination pagination, string keyword, string category, bool isEnabled, string appId = null, string tags = null);
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <returns></returns>
        Task<IEnumerable<FormSchemeEntity>> GetSchemePageList(Pagination pagination, string schemeInfoId);
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <returns></returns>
        Task<IEnumerable<FormSchemeEntity>> GetSchemeList(string schemeInfoId);
        /// <summary>
        /// 获取模板基础信息的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<FormSchemeInfoEntity> GetSchemeInfoEntity(string keyValue);
        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<FormSchemeEntity> GetSchemeEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除模板信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存模板信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="schemeInfoEntity">模板基础信息</param>
        /// <param name="schemeEntity">模板信息</param>
        Task SaveEntity(string keyValue, FormSchemeInfoEntity schemeInfoEntity, FormSchemeEntity schemeEntity);
        /// <summary>
        /// 保存模板基础信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="schemeInfoEntity">模板基础信息</param>
        Task SaveSchemeInfoEntity(string keyValue, FormSchemeInfoEntity schemeInfoEntity);
        /// <summary>
        /// 更新模板
        /// </summary>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <param name="schemeId">模板主键</param>
        Task UpdateScheme(string schemeInfoId, string schemeId);
        /// <summary>
        /// 更新自定义表单模板状态
        /// </summary>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <param name="state">状态1启用0禁用</param>
        Task UpdateState(string schemeInfoId, int state);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 保存表单数据
        /// </summary>
        /// <param name="dto">参数</param>
        /// <returns></returns>
        Task<JObject> SaveFormData(FormDataDto dto);

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="desktopParam">桌面参数</param>
        /// <param name="schemeId">模板信息主键</param>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryJson">查询条件</param>
        /// <returns></returns>
        Task<DataTable> GetFormDataPage(string desktopParam, string schemeId, Pagination pagination, string queryJson);

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="desktopParam">桌面参数</param>
        /// <param name="schemeId">模板信息主键</param>
        /// <param name="queryJson">查询条件</param>
        /// <param name="sidx">排序字段</param>
        /// <returns></returns>
        Task<DataTable> GetFormDataList(string desktopParam, string schemeId, string queryJson,string sidx);

        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="schemeId">模板信息主键</param>
        /// <param name="key">主键名</param>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task<Dictionary<string, DataTable>> GetFormData(string schemeId, string key, string keyValue);

        /// <summary>
        /// 获取视图表单子视图数据
        /// </summary>
        /// <param name="schemeId">表单模板主键</param>
        /// <param name="mainData">主视图数据</param>
        /// <returns></returns>
        Task<Dictionary<string, DataTable>> GetViewFormData(string schemeId, string mainData);

        /// <summary>
        /// 删除表单数据
        /// </summary>
        /// <param name="schemeId">模板信息主键</param>
        /// <param name="key">主键名</param>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task DeleteFormData(string schemeId, string key, string keyValue);

        /// <summary>
        /// 获取sql列
        /// </summary>
        /// <param name="dbCode"></param>
        /// <param name="sql"></param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDataColName(string dbCode, string sql);

        /// <summary>
        /// 批量删除表单数据
        /// </summary>
        /// <param name="schemeId">模板信息主键</param>
        /// <param name="key">主键名</param>
        /// <param name="keyValues">主键值集</param>
        /// <returns></returns>
        Task DeleteFormDatas(string schemeId, string key, string keyValues);

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
        Task<bool> Import(string jsonData);
        #endregion
        
    }
}
