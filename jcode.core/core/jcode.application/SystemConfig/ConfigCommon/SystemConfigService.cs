using jcode.iapplication;
using System;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-18 09:37:38
    /// 描 述： 系统配置-登录策略数据库执行类
    /// </summary>
    public class SystemConfigService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<SystemConfigEntity> GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<SystemConfigEntity>(keyValue);
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
            await this.BaseRepository().Delete<SystemConfigEntity>(keyValue);
        }
        
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, SystemConfigEntity entity)
        {
            if ( string.IsNullOrEmpty(keyValue) )
            {
                if(string.IsNullOrEmpty(entity.F_Id))
                {
                    entity.F_Id = Guid.NewGuid().ToString();
                }
                
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();

                await this.BaseRepository().Insert(entity);
            }
            else
            {
                entity.F_ModifyDate = DateTime.Now;
                entity.F_ModifyUserId = this.GetUserId();
                entity.F_ModifyUserName = this.GetUserName();
                entity.F_Id = keyValue;
                
                await this.BaseRepository().Update(entity);
            }
        }
        #endregion
    }
}
