using jcode.iapplication;
using jcode.util;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.25
    /// 描 述：数据权限
    /// </summary>
    public class DataAuthorizeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取对应编码中的权限设置
        /// </summary>
        /// <param name="code">数据权限编码</param>
        /// <param name="objectIds">用户或角色主键</param>
        /// <returns></returns>
        public Task<IEnumerable<DataAuthorizeEntity>> GetList(string code, string[] objectIds)
        {
            return this.BaseRepository().FindList<DataAuthorizeEntity>(t=> t.F_Code == code &&  objectIds.Contains(t.F_ObjectId));
        }
        
        /// <summary>
        /// 获取对应编码中的权限设置
        /// </summary>
        /// <param name="code">数据权限编码</param>
        /// <param name="objectIds">用户或角色主键</param>
        /// <param name="isPc">是否是pc端请求</param>
        /// <returns></returns>
        public Task<IEnumerable<DataAuthorizeEntity>> GetList(string code, string[] objectIds,bool isPc)
        {
            if (isPc)
            {
                return this.BaseRepository().FindList<DataAuthorizeEntity>(t=> t.F_Code == code &&  objectIds.Contains(t.F_ObjectId) && (t.F_DeviceType == null || t.F_DeviceType == 0 || t.F_DeviceType == 1));
            }
            return this.BaseRepository().FindList<DataAuthorizeEntity>(t=> t.F_Code == code &&  objectIds.Contains(t.F_ObjectId) && (t.F_DeviceType == null || t.F_DeviceType == 0 || t.F_DeviceType == 2));
        }
        /// <summary>
        /// 获取数据权限列表（分页）
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询关键词</param>
        /// <param name="objectId">用户或角色主键</param>
        /// <param name="type">1.普通权限 2.自定义表单权限</param>
        /// <returns></returns>
        public Task<IEnumerable<DataAuthorizeEntity>> GetPageList(Pagination pagination, string keyword, string objectId, int? type)
        {
            var exp = Expressionable.Create<DataAuthorizeEntity>()
               .AndIF(type != null, t => t.F_Type == type)
               .AndIF(!string.IsNullOrEmpty(objectId), t => t.F_ObjectId == objectId)
               .AndIF(!string.IsNullOrEmpty(keyword), t => t.F_Name.Contains(keyword) || t.F_Code.Contains(keyword))
               .ToExpression();
            
            return this.BaseRepository().FindList<DataAuthorizeEntity>(exp, pagination);
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DataAuthorizeEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<DataAuthorizeEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await this.BaseRepository().Delete<DataAuthorizeEntity>(keyValue);
        }
        /// <summary>
        ///  保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        public async Task SaveEntity(string keyValue, DataAuthorizeEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(entity);
            }
            else
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();

                await this.BaseRepository().Insert(entity);

            }
        }

        #endregion
    }
}
