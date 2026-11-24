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
    /// 日 期：2019.09.19
    /// 描 述：数据字典
    /// </summary>
    public interface DataItemIBLL : IBLL
    {
        #region 数据字典分类
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<DataItemEntity>> GetClassifyList();
        /// <summary>
        /// 分类列表
        /// </summary>
        /// <param name="keyword">关键词（名称/编码）</param>
        /// <param name="enabledMark">是否只取有效</param>
        /// <param name="appId">所属应用主键：应用工作台里只列本应用的分类，为空时不过滤</param>
        /// <returns></returns>
        Task<IEnumerable<DataItemEntity>> GetClassifyList(string keyword, bool enabledMark = true, string appId = null);
        /// <summary>
        /// 获取分类树形数据
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<TreeModel>> GetClassifyTree();
        /// <summary>
        /// 保存分类数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        Task SaveClassifyEntity(string keyValue, DataItemEntity entity);
        /// <summary>
        /// 删除分类数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteClassify(string keyValue);
        /// <summary>
        /// 通过编号获取字典分类实体
        /// </summary>
        /// <param name="itemCode">编码</param>
        /// <returns></returns>
        Task<DataItemEntity> GetClassifyEntityByCode(string itemCode);
        #endregion

        #region 字典明细
        /// <summary>
        /// 获取数据字典明显
        /// </summary>
        /// <param name="itemCode">分类编码</param>
        /// <returns></returns>
        Task<IEnumerable<DataItemDetailEntity>> GetDetailList(string itemCode);
        /// <summary>
        /// 获取数据字典明显
        /// </summary>
        /// <param name="itemCode">分类编码</param>
        /// <param name="keyword">关键词（名称/值）</param>
        /// <param name="appId">所属应用主键：传值时只取本应用的明细加全局明细，为空时不过滤</param>
        /// <returns></returns>
        Task<IEnumerable<DataItemDetailEntity>> GetDetailList(string itemCode, string keyword, string appId = null);
        /// <summary>
        /// 获取数据字典明显
        /// </summary>
        /// <param name="itemCode">分类编号</param>
        /// <param name="parentId">父级主键</param>
        /// <returns></returns>
        Task<IEnumerable<DataItemDetailEntity>> GetDetailListByParentId(string itemCode, string parentId);
        /// <summary>
        /// 获取字典明细树形数据
        /// </summary>
        /// <param name="itemCode">分类编号</param>
        /// <returns></returns>
        Task<IEnumerable<TreeModel>> GetDetailTree(string itemCode);
        /// <summary>
        /// 项目值不能重复
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="itemValue">项目值</param>
        /// <param name="itemCode">分类编码</param>
        /// <returns></returns>
        Task<bool> ExistDetailItemValue(string keyValue, string itemValue, string itemCode);
        /// <summary>
        /// 项目名不能重复
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="itemName">项目名</param>
        /// <param name="itemCode">分类编码</param>
        /// <returns></returns>
        Task<bool> ExistDetailItemName(string keyValue, string itemName, string itemCode);
        /// <summary>
        /// 保存明细数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        Task SaveDetailEntity(string keyValue, DataItemDetailEntity entity);
        /// <summary>
        /// 虚拟删除明细数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteDetail(string keyValue);



        /// <summary>
        /// 保存明细数据实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <param name="list">实体</param>
        Task SaveDetailList(string code, List<DataItemDetailEntity> list);

        #endregion

        #region 扩展方法
        /// <summary>
        /// 根据字典编码以及项目值获取项目名称
        /// </summary>
        /// <param name="ItemCode">字典编码</param>
        /// <param name="ItemValue">项目值</param>
        /// <returns></returns>
        Task<List<DataItemDetailEntity>> GetDetails(string ItemCode,string ItemValue);
        #endregion
    }
}
