using jcode.util;
using jcode.autofac;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.18
    /// 描 述：行政区域
    /// </summary>
    public interface AreaIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取区域列表数据
        /// </summary>
        /// <param name="parentId">父节点主键（0表示顶层）</param>
        /// <returns></returns>
        Task<IEnumerable<AreaEntity>> GetList(string parentId);
        /// <summary>
        /// 区域实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        Task<AreaEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 虚拟删除区域
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="areaEntity">区域实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, AreaEntity areaEntity);

        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="areaEntitys">区域实体</param>
        /// <returns></returns>
        Task SaveEntitys(List<AreaEntity> areaEntitys);
        #endregion
    }
}
