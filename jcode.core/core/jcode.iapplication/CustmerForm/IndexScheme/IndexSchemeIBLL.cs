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
    /// 日 期：2024.04.18
    /// 描 述：页面模板
    /// </summary>
    public interface IndexSchemeIBLL:IBLL
    {
        #region 获取数据
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
        Task<IEnumerable<IndexSchemeInfoEntity>> GetSchemeInfoPageList(Pagination pagination, string keyword, string category, bool isEnabled, string appId = null, string tags = null);
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <returns></returns>
        Task<IEnumerable<IndexSchemeEntity>> GetSchemePageList(Pagination pagination, string schemeInfoId);
        /// <summary>
        /// 获取模板基础信息的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<IndexSchemeInfoEntity> GetSchemeInfoEntity(string keyValue);
        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<IndexSchemeEntity> GetSchemeEntity(string keyValue);

        /// <summary>
        /// 获取模板的实体
        /// </summary>
        /// <param name="infoId">信息主键</param>
        /// <param name="ver">版本号</param>
        /// <returns></returns>
        Task<IndexSchemeEntity> GetSchemeEntityByInfoIdAndVer(string infoId, string ver);
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
        Task SaveEntity(string keyValue, IndexSchemeInfoEntity schemeInfoEntity, IndexSchemeEntity schemeEntity);
        /// <summary>
        /// 保存模板基础信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="schemeInfoEntity">模板基础信息</param>
        Task SaveSchemeInfoEntity(string keyValue, IndexSchemeInfoEntity schemeInfoEntity);
        /// <summary>
        /// 更新自定义表单模板状态
        /// </summary>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <param name="state">状态1启用0禁用</param>
        Task UpdateState(string schemeInfoId, int state);
        #endregion
        
        #region 自定义表单导入导出

        /// <summary>
        /// 导出自定义表单模版
        /// </summary>
        /// <param name="keyValue">自定义表单主键</param>
        /// <param name="path">目录</param>
        /// <returns></returns>
        Task<string> Export(string keyValue, string path);

        /// <summary>
        /// 导入自定义表单数据
        /// </summary>
        /// <param name="jsonData"></param>
        Task<bool> Import(string jsonData);
        #endregion
    }
}
