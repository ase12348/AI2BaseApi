using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座项目-图片模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2022-07-07 11:19:40
    /// 描 述： 图片管理
    /// </summary>
    public class LrBaseImgBLL : BLLBase, ILrBaseImgBLL, BLL
    {
        private readonly LrBaseImgService lrBaseImgService = new LrBaseImgService();

        private readonly AnnexesFileIBLL _annexesFileIBLL;


        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="annexesFileIBLL"></param>
        public LrBaseImgBLL(AnnexesFileIBLL annexesFileIBLL)
        {
            _annexesFileIBLL = annexesFileIBLL;
        }

        #region 获取数据
        /// <summary>
        /// 获取图片管理的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseImgEntity>> GetList(LrBaseImgEntity queryParams)
        {
            return lrBaseImgService.GetList(queryParams);
        }

        /// <summary>
        /// 获取图片管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseImgEntity>> GetPageList(Pagination pagination, LrBaseImgEntity queryParams)
        {
            return lrBaseImgService.GetPageList(pagination, queryParams);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseImgEntity> GetEntity(string keyValue)
        {
            return lrBaseImgService.GetEntity(keyValue);
        }



        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            lrBaseImgService.BeginTrans();
            try
            {
                await lrBaseImgService.Delete(keyValue);
                await _annexesFileIBLL.DeleteEntity(keyValue);
                lrBaseImgService.Commit();
            }
            catch (Exception)
            {
                lrBaseImgService.Rollback();
                throw;
            }
        }



        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseImgEntity entity)
        {
            await lrBaseImgService.SaveEntity(keyValue, entity);
        }



        #endregion


        #region 扩展方法

        /// <summary>
        /// 上传图片
        /// </summary>
        /// <param name="dto">文件信息</param>
        /// <param name="fileType">图片分类</param>
        /// <returns></returns>
        public async Task<string> UploadImg(AnnexesFileInputDto dto,string fileType)
        {
            var annexeEntity = await _annexesFileIBLL.SaveAnnexes(dto.FolderId, dto.FileId, dto.FileName, dto.Chunks,"", "lr_base_img", "F_Id",ConfigHelper.GetConfig().IsImgSaveDb);
            if (annexeEntity != null)
            {
                string FileEextension = Path.GetExtension(dto.FileName);
                var entity = new LrBaseImgEntity();
                entity.F_Id = dto.FileId;
                entity.F_IsPublic = dto.IsPublic;
                entity.F_FileType = fileType;
                entity.F_FileName = dto.FileName;
                entity.F_FileExtensions = FileEextension;

                await SaveEntity(string.Empty, entity);

                return entity.F_Id;
            }
            else
            {
                throw (new Exception("上传图片失败！"));
            }
        }

        #endregion
    }
}
