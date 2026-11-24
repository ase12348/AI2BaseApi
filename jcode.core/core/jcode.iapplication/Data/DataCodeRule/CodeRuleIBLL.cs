using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：编号规则
    /// </summary>
    public interface CodeRuleIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <param name="pagination">分页</param>
        /// <param name="keyword">查询参数</param>
        /// <param name="appId">所属应用主键（为空表示不过滤）</param>
        /// <returns></returns>
        Task<IEnumerable<CodeRuleEntity>> GetPageList(Pagination pagination, string keyword, string appId = null);
        /// <summary>
        /// 规则列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CodeRuleEntity>> GetList();
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task<CodeRuleEntity> GetEntity(string keyValue);
        /// <summary>
        /// 规则实体
        /// </summary>
        /// <param name="enCode">规则编码</param>
        /// <returns></returns>
        Task<CodeRuleEntity> GetEntityByCode(string enCode);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除规则
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存规则表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="codeRuleEntity">规则实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, CodeRuleEntity codeRuleEntity);
        #endregion

        #region 单据编码处理
        /// <summary>
        /// 获得指定模块或者编号的单据号
        /// </summary>
        /// <param name="enCode">编码</param>
        /// <param name="formData">表单值</param>
        /// <returns>单据号</returns>
        Task<string> GetBillCode(string enCode,string formData);
        #endregion
    }
}
