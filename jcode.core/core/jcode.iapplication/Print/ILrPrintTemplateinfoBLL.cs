using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印设计
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-09 14:13:34
    /// 描 述： 打印设计-打印模板
    /// </summary>
    public interface ILrPrintTemplateinfoBLL : IBLL
    {
        #region 获取数据

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <param name="isEnabled">是否获取正式表单</param>
        /// <param name="tags">标签（F_Category 存逗号分隔的标签串，按包含匹配）</param>
        /// <returns></returns>
        Task<IEnumerable<LrPrintTemplateinfoEntity>> GetPageList(Pagination pagination,
            LrPrintTemplateinfoEntity queryParams, bool isEnabled = false, string tags = null);
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrPrintTemplateinfoEntity> GetEntity(string keyValue);
        /// <summary>
        /// 获取实体数据-根据模板编码
        /// </summary>
        /// <param name="code">模板编码</param>
        /// <returns></returns>
        Task<LrPrintTemplateinfoEntity> GetEntityByCode(string code);
        #endregion

        #region 提交数据

        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, LrPrintTemplateinfoEntity entity);
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task Delete(string keyValue);
        #endregion

        #region 扩展
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task DeleteAll(string keyValue);
        /// <summary>
        /// 保存实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="dto">数据集合</param>
        /// <returns></returns>
        Task SaveAll(string keyValue, PrintTemplateDto dto);
        /// <summary>
        /// 启用/停用模板
        /// </summary>
        /// <param name="id">主键</param>
        /// <param name="state">状态1启用0禁用</param>
        /// <returns></returns>
        Task UpdateState(string id, int state);
        /// <summary>
        /// 导出打印模板数据
        /// </summary>
        /// <param name="keyValue">自定义表单主键</param>
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
