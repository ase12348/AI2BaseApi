using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.07
    /// 描 述：移动端功能管理
    /// </summary>
    public class FunctionBLL:BLLBase, FunctionIBLL,BLL
    {
        private readonly FunctionSerivce functionSerivce = new FunctionSerivce();
        private readonly ICache _iCache;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public FunctionBLL(ICache iCache)
        {
            _iCache = iCache;
        }


        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<FunctionEntity>> GetList() {
            return functionSerivce.GetList();
        }

        /// <summary>
        /// 获取实体对象
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<FunctionEntity> GetEntity(string keyValue)
        {
            return functionSerivce.GetEntity(keyValue);
        }

        /// <summary>
        /// 获取移动功能模板
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<FunctionSchemeEntity> GetScheme(string keyValue)
        {
            return functionSerivce.GetScheme(keyValue);
        }

        /// <summary>
        /// 获取移动功能子集项
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<IEnumerable<ModuleItemEntity>> GetModuleItemList(string keyValue) {
            return functionSerivce.GetModuleItemList(keyValue);
        }

        /// <summary>
        /// 获取移动功能所有子集
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<ModuleItemEntity>> GetModuleItemList() {
            return functionSerivce.GetModuleItemList();
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            await functionSerivce.Delete(keyValue);
        }
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="functionEntity">功能信息</param>
        /// <param name="moduleItemEntitys">功能子集信息</param>
        public async Task SaveEntity(string keyValue, FunctionEntity functionEntity, List<ModuleItemEntity> moduleItemEntitys)
        {
            functionSerivce.BeginTrans();
            try
            {
                await functionSerivce.SaveEntity(keyValue, functionEntity, moduleItemEntitys);

                // 处理权限信息
                if (!string.IsNullOrEmpty(keyValue) && moduleItemEntitys != null)
                {
                    var objectList = await functionSerivce.BaseRepository().FindList<AuthorizeEntity>(t => t.F_ParentId == keyValue);

                    List<string> buttonList = new List<string>();
                    List<string> columnList = new List<string>();
                    List<string> formList = new List<string>();

                    foreach (var item in moduleItemEntitys)
                    {
                        if (item.F_Type == 1)
                        {
                            buttonList.Add(item.F_Code);
                        }
                        else if (item.F_Type == 2)
                        {
                            columnList.Add(item.F_Code);
                        }
                        else if (item.F_Type == 3)
                        {
                            formList.Add(item.F_Code);
                        }
                    }

                    if (buttonList.Count == 0)
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 6 && t.F_ParentId == functionEntity.F_Id);
                    }
                    else
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 6 && t.F_ParentId == functionEntity.F_Id && !buttonList.Contains(t.F_ItemId));
                    }

                    if (columnList.Count == 0)
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 7 && t.F_ParentId == functionEntity.F_Id);
                    }
                    else
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 7 && t.F_ParentId == functionEntity.F_Id && !columnList.Contains(t.F_ItemId));
                    }

                    if (formList.Count == 0)
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 8 && t.F_ParentId == functionEntity.F_Id);
                    }
                    else
                    {
                        await functionSerivce.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == 8 && t.F_ParentId == functionEntity.F_Id && !formList.Contains(t.F_ItemId));
                    }

                    Dictionary<string, bool> omap = new Dictionary<string, bool>();

                    foreach (var item in objectList)
                    {
                        if (!omap.ContainsKey(item.F_ObjectId))
                        {
                            omap.Add(item.F_ObjectId, true);
                            _iCache.Remove($"Authorize_{item.F_ObjectId}_2_{keyValue}");
                            _iCache.Remove($"Authorize_{item.F_ObjectId}_3_{keyValue}");
                            _iCache.Remove($"Authorize_{item.F_ObjectId}_4_{keyValue}");
                        }

                    }
                }



                functionSerivce.Commit();
            }
            catch (System.Exception)
            {
                functionSerivce.Rollback();
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
            await functionSerivce.UpdateState(keyValue, state);
        }
        #endregion
    }
}
