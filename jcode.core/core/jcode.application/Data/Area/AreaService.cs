using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.18
    /// 描 述：行政区域
    /// </summary>
    public class AreaService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 区域列表
        /// 调整为linq写法，目前行政区域关闭租户单独维护;2022/05/31
        /// </summary>
        /// <param name="parentId">父节点Id</param>
        /// <param name="keyword">关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<AreaEntity>> GetList(string parentId,string keyword)
        {
            var expression = LinqExtensions.True<AreaEntity>();
            if (string.IsNullOrEmpty(parentId))
            {
                parentId = "0";
            }
            expression = expression.And(t => t.F_ParentId == parentId);
            if (!string.IsNullOrEmpty(keyword))
            {
                expression = expression.And(t => (t.F_AreaName.Contains(keyword) || t.F_AreaCode.Contains(keyword)));
            }
            return this.BaseRepository(false,true).FindList(expression, "F_AreaCode");
        }
        /// <summary>
        /// 区域实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<AreaEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository(false, true).FindEntityByKey<AreaEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除区域
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await this.BaseRepository(false, true).Delete<AreaEntity>(keyValue);
        }
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="areaEntity">区域实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, AreaEntity areaEntity)
        {
            areaEntity.F_QuickQuery = Str.ConvertPinYin(areaEntity.F_AreaName).ToUpper();
            areaEntity.F_SimpleSpelling = Str.PinYin(areaEntity.F_AreaName);
            
            var sortCode = areaEntity.F_AreaCode;
            if (sortCode.Length >= 12)
            {
                sortCode = sortCode.Substring(sortCode.Length - 5, 5);
            }

            try
            {
                areaEntity.F_SortCode = Convert.ToInt32(sortCode);
            }
            catch
            {
                areaEntity.F_SortCode = 1;
            }



            if (!string.IsNullOrEmpty(keyValue))
            {
                areaEntity.F_AreaId = keyValue;
                areaEntity.F_ModifyDate = DateTime.Now;
                areaEntity.F_ModifyUserId = this.GetUserId();
                areaEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository(false, true).Update(areaEntity);
            }
            else
            {
                areaEntity.F_AreaId = areaEntity.F_AreaCode;
                areaEntity.F_CreateDate = DateTime.Now;
                areaEntity.F_DeleteMark = 0;
                areaEntity.F_EnabledMark = 1;

                areaEntity.F_CreateUserId = this.GetUserId();
                areaEntity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository(false, true).Insert(areaEntity);
            }

        }
        
        
        /// <summary>
        /// 保存区域表单（新增、修改）
        /// </summary>
        /// <param name="areaEntitys">区域实体</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<AreaEntity> areaEntitys)
        {
      
            foreach (var areaEntity in areaEntitys)
            {
                areaEntity.F_QuickQuery = Str.ConvertPinYin(areaEntity.F_AreaName).ToUpper();
                areaEntity.F_SimpleSpelling = Str.PinYin(areaEntity.F_AreaName);
                areaEntity.F_SortCode = Convert.ToInt32(areaEntity.F_AreaCode);
                areaEntity.F_AreaId = areaEntity.F_AreaCode;
                areaEntity.F_CreateDate = DateTime.Now;
                areaEntity.F_DeleteMark = 0;
                areaEntity.F_EnabledMark = 1;
                areaEntity.F_CreateUserId = this.GetUserId();
                areaEntity.F_CreateUserName = this.GetUserName();
                
            }
            await this.BaseRepository(false, true).Inserts(areaEntitys);
        }
        #endregion
    }
}
