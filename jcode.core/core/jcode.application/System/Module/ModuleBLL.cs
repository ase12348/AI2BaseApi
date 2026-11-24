using System;
using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：功能模块
    /// </summary>
    public class ModuleBLL : BLLBase, ModuleIBLL, BLL
    {
        private readonly ModuleService moduleService = new ModuleService();
        private readonly ICache _iCache;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public ModuleBLL(ICache iCache) {
            _iCache = iCache;
        }


        #region 功能模块
        /// <summary>
        /// 获取所有模块数据
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<ModuleEntity>> GetModuleList() {
            var list = _iCache.Read<IEnumerable<ModuleEntity>>("jcode_modules");
            if (list == null)
            {
                list = await moduleService.GetList();
                _iCache.Write<IEnumerable<ModuleEntity>>("jcode_modules", list);
                
            }
            return list;
        }
        /// <summary>
        /// 功能列表
        /// </summary>
        /// <returns></returns>
        public Task<ModuleEntity> GetModuleByUrl(string url)
        {
            return moduleService.GetEntityByUrl(url);
        }
        /// <summary>
        ///  功能列表(code)
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<ModuleEntity> GetEntityByCode(string code)
        {
            return moduleService.GetEntityByCode(code);
        }
        /// <summary>
        /// 功能实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<ModuleEntity> GetModuleEntity(string keyValue)
        {
            return moduleService.GetEntity(keyValue);
        }

        /// <summary>
        /// 根据父级主键获取数据
        /// </summary>
        /// <param name="parentId">父级主键</param>
        /// <returns></returns>
        public async Task<IEnumerable<ModuleEntity>> GetModuleListByParentId(string parentId)
        {
            List<ModuleEntity> list = (List<ModuleEntity>)await GetModuleList();
            list = list.FindAll(t => t.F_ParentId == parentId);
            return list;
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除模块功能
        /// </summary>
        /// <param name="keyValue">主键值</param>
        public async Task<bool> Delete(string keyValue)
        {
            List<ModuleEntity> list = (List<ModuleEntity>) await GetModuleListByParentId(keyValue);
            moduleService.BeginTrans();
            try
            {
                if (list.Count > 0)
                {
                    foreach (var item in list)
                    {
                        await Delete(item.F_ModuleId);
                    }
                }
                await moduleService.Delete(keyValue);
                moduleService.Commit();
                _iCache.Remove($"jcode_modules");
            }
            catch (Exception)
            {
                moduleService.Rollback();
                throw;
            }
            return true;
        }
       
        
        /// <summary>
        /// 保存模块功能实体（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="moduleEntity">实体</param>
        public async Task<string> SaveEntity(string keyValue, ModuleEntity moduleEntity)
        {
            await moduleService.SaveEntity(keyValue, moduleEntity);
            _iCache.Remove("jcode_modules");
            return moduleEntity.F_ModuleId;
        }
        
        
        
        /// <summary>
        /// 更新状态
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="state"></param>
        /// <returns></returns>
        public async Task UpdateState(string keyValue, int state) {
            _iCache.Remove("jcode_modules");
            _iCache.Remove($"jcode_button{keyValue}");
            _iCache.Remove($"jcode_column{keyValue}");
            _iCache.Remove($"jcode_form{keyValue}");
            await moduleService.UpdateState(keyValue, state);
        }
        #endregion

        #region 测试模块
        /// <summary>
        /// 测试功能
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        private async Task Test(IEnumerable<ModuleEntity> list) {
            foreach (var item in list) {
               await moduleService
                    .BaseRepository("Server=182.61.27.110;user id=jcode;password=jcode123qweASD@mysql.com;database=core-vue-tool2;charset=utf8;", "MySql")
                    .ORM.Updateable<ModuleEntity>(new { F_Icon = item.F_Icon }).IgnoreColumns(ignoreAllNullColumns: true).Where(t => t.F_EnCode == item.F_EnCode).ExecuteCommandAsync();

                    //.Update<ModuleEntity>(new { F_Icon = item.F_Icon }, t => t.F_EnCode == item.F_EnCode);
            }
        }
        #endregion
    }
}
