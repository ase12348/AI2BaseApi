using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-图片模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2022-07-07 11:19:40
    /// 描 述： 图片管理
    /// </summary>
    public interface ILrBaseImgBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取图片管理的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrBaseImgEntity>> GetList(LrBaseImgEntity queryParams);

        /// <summary>
        /// 获取图片管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        Task<IEnumerable<LrBaseImgEntity>> GetPageList(Pagination pagination, LrBaseImgEntity queryParams);

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<LrBaseImgEntity> GetEntity(string keyValue);



        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task Delete(string keyValue);



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, LrBaseImgEntity entity);



        #endregion


        #region 扩展方法

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="dto">文件信息</param>
        /// <param name="fileType">图片分类</param>
        /// <returns></returns>
       Task<string> UploadImg(AnnexesFileInputDto dto, string fileType);

        #endregion
    }
}
