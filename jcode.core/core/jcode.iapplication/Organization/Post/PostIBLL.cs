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
    /// 描 述：岗位管理
    /// </summary>
    public interface PostIBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <returns></returns>
        Task<IEnumerable<PostEntity>> GetList(string companyId);
        /// <summary>
        /// 获取岗位数据列表（根据公司列表）
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="departmentId">部门Id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        Task<IEnumerable<PostEntity>> GetList(string companyId,  string departmentId,string keyword);
        /// <summary>
        /// 获取岗位数据列表(根据主键串)
        /// </summary>
        /// <param name="postIds">根据主键串</param>
        /// <returns></returns>
        Task<IEnumerable<PostEntity>> GetListByPostIds(string postIds);
        /// <summary>
        /// 获取岗位实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<PostEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存岗位（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="postEntity">岗位实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, PostEntity postEntity);
        #endregion

        #region 扩展方法
        /// <summary>
        /// 判断是否是上级
        /// </summary>
        /// <param name="myId">自己的岗位</param>
        /// <param name="otherIds">对方的岗位</param>
        /// <returns></returns>
        Task<bool> IsUp(string myId, List<string> otherIds);
        /// <summary>
        /// 判断是否是下级
        /// </summary>
        /// <param name="myId">自己的岗位</param>
        /// <param name="otherIds">对方的岗位</param>
        /// <returns></returns>
        Task<bool> IsDown(string myId, List<string> otherIds);
        /// <summary>
        /// 获取上级岗位人员ID
        /// </summary>
        /// <param name="postIds">岗位</param>
        /// <param name="level">级数</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetUpIdList(string postIds, int level);
        /// <summary>
        /// 获取下级岗位人员ID
        /// </summary>
        /// <param name="postIds">岗位</param>
        /// <param name="level">级数</param>
        /// <returns></returns>
        Task<IEnumerable<string>> GetDownIdList(string postIds, int level);
        #endregion
    }
}
