using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：功能模块
    /// </summary>
    public class ModuleService : ServiceBase
    {
        #region 功能模块
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ModuleEntity>> GetList()
        {
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                var appCode = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
                return this.BaseRepository().FindList<ModuleEntity>(t=>t.F_DeleteMark == 0 &&appCode.Contains(t.F_AppCode), "F_ParentId,F_SortCode");
            }
            return this.BaseRepository().FindList<ModuleEntity>(t=>t.F_DeleteMark == 0, "F_ParentId,F_SortCode");
        }

        /// <summary>
        /// 功能列表(url)
        /// </summary>
        /// <returns></returns>
        public Task<ModuleEntity> GetEntityByUrl(string url)
        {
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                var appCode = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
                return this.BaseRepository().FindEntity<ModuleEntity>(t=>t.F_DeleteMark == 0 && t.F_UrlAddress == url && appCode.Contains(t.F_AppCode));
            }
            return this.BaseRepository().FindEntity<ModuleEntity>(t=>t.F_DeleteMark == 0 && t.F_UrlAddress == url);
        }

        /// <summary>
        ///  功能列表(code)
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<ModuleEntity> GetEntityByCode(string code)
        {
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                var appCode = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
                return this.BaseRepository().FindEntity<ModuleEntity>(t => t.F_DeleteMark == 0 && t.F_EnCode == code && appCode.Contains(t.F_AppCode));
            }
            return this.BaseRepository().FindEntity<ModuleEntity>(t => t.F_DeleteMark == 0 && t.F_EnCode == code);
        }
        /// <summary>
        /// 功能实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<ModuleEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<ModuleEntity>(keyValue);
        }
        #endregion
        
        #region 提交数据
        /// <summary>
        /// 虚拟删除模块功能
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await this.BaseRepository().Delete<ModuleEntity>(keyValue);
        }
        
        /// <summary>
        /// 保存模块功能实体（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moduleEntity">实体</param>
        public async Task SaveEntity(string keyValue, ModuleEntity moduleEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                var oldModuleEntity =await GetEntity(keyValue);
                if (oldModuleEntity == null) {
                    moduleEntity.F_ModuleId = keyValue;
                    keyValue = string.Empty;
                }
            }
            if (string.IsNullOrEmpty(moduleEntity.F_ParentId) || moduleEntity.F_ParentId == "-1")
            {
                moduleEntity.F_ParentId = "0";
            }

            if (string.IsNullOrEmpty(keyValue))
            {
                // 新增
                if (string.IsNullOrEmpty(moduleEntity.F_ModuleId)) {
                    moduleEntity.F_ModuleId = Guid.NewGuid().ToString();
                }
                    
                moduleEntity.F_CreateDate = DateTime.Now;
                moduleEntity.F_CreateUserId = this.GetUserId();
                moduleEntity.F_CreateUserName = this.GetUserName();
                moduleEntity.F_DeleteMark = 0;
                await this.BaseRepository().Insert(moduleEntity);
            }
            else
            {
                // 编辑
                moduleEntity.F_ModuleId = keyValue;
                moduleEntity.F_ModifyDate = DateTime.Now;
                moduleEntity.F_ModifyUserId = this.GetUserId();
                moduleEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(moduleEntity,true);
            }
        }


        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task UpdateState(string keyValue, int state)
        {
            await this.BaseRepository().Update(new ModuleEntity { F_ModuleId = keyValue, F_EnabledMark = state }, true);
        }
        #endregion
    }
}
