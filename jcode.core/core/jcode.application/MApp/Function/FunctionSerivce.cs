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
    /// 日 期：2020.04.07
    /// 描 述：移动端功能管理
    /// </summary>
    public class FunctionSerivce: ServiceBase
    {

        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<FunctionEntity>> GetList()
        {
            if (ConfigHelper.GetConfig().OpenAppCodeFilter && !string.IsNullOrEmpty(ConfigHelper.GetConfig().AppCode))
            {
                var appCode = ConfigHelper.GetConfig().AppCode.Split(",").ToList();
                return this.BaseRepository().FindList<FunctionEntity>(t=>appCode.Contains(t.F_AppCode), "F_SortCode");
            }
            return this.BaseRepository().FindAll<FunctionEntity>("F_SortCode");
        }
        /// <summary>
        /// 获取移动功能模板
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<FunctionSchemeEntity> GetScheme(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<FunctionSchemeEntity>(keyValue);
        }
        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<FunctionEntity> GetEntity(string keyValue) {
            return this.BaseRepository().FindEntityByKey<FunctionEntity>(keyValue);
        }

        /// <summary>
        /// 获取移动功能子集项
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ModuleItemEntity>> GetModuleItemList(string keyValue)
        {
            return this.BaseRepository().FindList<ModuleItemEntity>(t=>t.F_ModuleId == keyValue);
        }

        /// <summary>
        /// 获取移动功能所有子集
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ModuleItemEntity>> GetModuleItemList()
        {
            return this.BaseRepository().FindAll<ModuleItemEntity>();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                FunctionEntity entity =await db.FindEntityByKey<FunctionEntity>(keyValue);
                if (entity != null) {
                    string schemeId = entity.F_SchemeId;
                    if (!string.IsNullOrEmpty(schemeId))
                    {
                        await db.Delete<FunctionSchemeEntity>(schemeId);
                    }
                }
                await db.Delete<FunctionEntity>(keyValue);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="functionEntity">功能信息</param>
        /// <param name="moduleItemEntitys">功能子集信息</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, FunctionEntity functionEntity, List<ModuleItemEntity> moduleItemEntitys)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                var oldEntity = await GetEntity(keyValue);
                if (oldEntity == null)
                {
                    keyValue = "";
                }

                if (!string.IsNullOrEmpty(functionEntity.Scheme) && functionEntity.F_IsSystem == 2) {
                    var functionSchemeEntity = new FunctionSchemeEntity()
                    {
                        F_Scheme = functionEntity.Scheme
                    };

                    if (string.IsNullOrEmpty(functionEntity.F_SchemeId))
                    {
                        functionSchemeEntity.F_Id = Guid.NewGuid().ToString();
                        await db.Insert(functionSchemeEntity);
                    }
                    else {
                        functionSchemeEntity.F_Id = functionEntity.F_SchemeId;
                        await db.Update(functionSchemeEntity);
                    }

                    functionEntity.F_SchemeId = functionSchemeEntity.F_Id;
                }

                if (string.IsNullOrEmpty(keyValue))
                {
                    functionEntity.F_Id = Guid.NewGuid().ToString();
                    functionEntity.F_CreateDate = DateTime.Now;
                    functionEntity.F_CreateUserId = this.GetUserId();
                    functionEntity.F_CreateUserName = this.GetUserName();
                   
                    await db.Insert(functionEntity);
                }
                else
                {
                    functionEntity.F_Id = keyValue;
                    functionEntity.F_ModifyDate = DateTime.Now;
                    functionEntity.F_ModifyUserId = this.GetUserId();
                    functionEntity.F_ModifyUserName = this.GetUserName();
                    await db.Update(functionEntity,true);
                    await db.Delete<ModuleItemEntity>(t=>t.F_ModuleId == keyValue);
                }
                int num = 0;
                if (moduleItemEntitys != null) {
                    foreach (var item in moduleItemEntitys)
                    {
                        item.F_SortCode = num;
                        item.F_ModuleId = functionEntity.F_Id;
                        if (string.IsNullOrEmpty(item.F_Id))
                        {
                            item.F_Id = Guid.NewGuid().ToString();
                        }

                        if (item.F_ParentId == null || moduleItemEntitys.Find(t => t.F_Id == item.F_ParentId) == null)
                        {
                            item.F_ParentId = "0";
                        }
                        
                        num++;
                    }
                    if (moduleItemEntitys.Count > 0) {
                        await db.Inserts(moduleItemEntitys);
                    }
                }


                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }

            
        }
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">模板信息主键</param>
        /// <param name="state">状态1启用0禁用</param>
        public async Task UpdateState(string keyValue, int state)
        {
            FunctionEntity entity = new FunctionEntity
            {
                F_Id = keyValue,
                F_EnabledMark = state
            };
            await this.BaseRepository().Update(entity,true);
        }
        #endregion
    }
}
