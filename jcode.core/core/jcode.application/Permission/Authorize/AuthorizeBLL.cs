using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.10
    /// 描 述：授權功能
    /// </summary>
    public class AuthorizeBLL : BLLBase, AuthorizeIBLL, BLL
    {
        private readonly AuthorizeService authorizeService = new AuthorizeService();
        private readonly ICache _iCache;
        private readonly UserRelationIBLL _userRelationIBLL;
        private readonly ModuleIBLL _moduleIBLL;
        private readonly FunctionIBLL _functionIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="moduleIBLL"></param>
        /// <param name="functionIBLL"></param>
        public AuthorizeBLL(ICache iCache, UserRelationIBLL userRelationIBLL, ModuleIBLL moduleIBLL, FunctionIBLL functionIBLL)
        {
            _iCache = iCache;
            _userRelationIBLL = userRelationIBLL ?? throw new ArgumentNullException(nameof(userRelationIBLL));
            _moduleIBLL = moduleIBLL ?? throw new ArgumentNullException(nameof(moduleIBLL));
            _functionIBLL = functionIBLL ?? throw new ArgumentNullException(nameof(functionIBLL)); 
        }
        #region 获取数据
        /// <summary>
        /// 获取被授权的功能的主键字串
        /// </summary>
        /// <param name="objectId">对象主键（角色,用户）</param>
        /// <param name="itemType">项目类型:1-菜单2-按钮3-视图</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<AuthorizeEntity>> GetList(string objectId, int itemType,string parentId)
        {
            var list = _iCache.Read<IEnumerable<AuthorizeEntity>>($"Authorize_{objectId}_{itemType}_{parentId}");
            if (list == null) {
                list =await authorizeService.GetList(objectId, itemType, parentId);
                _iCache.Write($"Authorize_{objectId}_{itemType}_{parentId}", list);
            }
            return list;
        }
        /// <summary>
        /// 获取被授权的功能的主键字串
        /// </summary>
        /// <param name="objectIds">对象主键串（角色,用户）</param>
        /// <param name="itemType">功能类型1菜单功能2按钮3视图列表4表单5App</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetItemIdListByobjectIds(string[] objectIds, int itemType, string parentId)
        {
            List<string> reslist = new List<string>();
            Dictionary<string,bool>dic = new Dictionary<string, bool>();
            foreach (var objectId in objectIds) {
                var list = await GetList(objectId, itemType, parentId);
                foreach (var item in list)
                {
                    if (!dic.ContainsKey(item.F_ItemId)) {
                        dic.Add(item.F_ItemId, true);

                        if (itemType == 4 || itemType == 8 || itemType == 40 || itemType == 80)
                        {
                            if (item.F_Type == null) {
                                item.F_Type = 2;
                            }
                            reslist.Add($"{item.F_ItemId},{item.F_Type}");
                        }
                        else {
                            reslist.Add(item.F_ItemId);
                        }
                        
                    }
                }
            }
            return reslist;
        }

        /// <summary>
        /// 获取登录者的功能模块
        /// </summary>
        /// <param name="itemType">类型 2按钮 3 列表 4表单</param>
        /// <param name="parentId">父级id</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetMyItemIdList(int itemType, string parentId) {
            var user = await this.CurrentUser();
            var mylist = new List<string>();
            if (user.F_SecurityLevel == 1 || user.F_SecurityLevel == 5)
            {

                /*switch (itemType)
                {
                    case 2:
                        var buttonList = await _moduleIBLL.GetButtonList(parentId);
                        foreach (var item in buttonList)
                        {
                            mylist.Add(item.F_EnCode);
                        }
                        break;
                    case 3:
                        var columnList = await _moduleIBLL.GetColumnList(parentId);
                        foreach (var item in columnList)
                        {
                            mylist.Add(item.F_EnCode);
                        }
                        break;
                    case 4:
                        var formList = await _moduleIBLL.GetFormList(parentId);
                        foreach (var item in formList)
                        {
                            mylist.Add(item.F_EnCode);
                        }
                        break;
                    case 6:
                    case 7:
                    case 8:
                        var moduleItemList = await _functionIBLL.GetModuleItemList(parentId);
                        foreach (var item in moduleItemList)
                        {
                            if (itemType == 6 && item.F_Type == 1) {
                                mylist.Add(item.F_Code);
                            }
                            else if (itemType == 7 && item.F_Type == 2)
                            {
                                mylist.Add(item.F_Code);
                            }
                            else if(itemType == 8 && item.F_Type == 3)
                            {
                                mylist.Add(item.F_Code);
                            }
                        }

                        break;
                }*/

                mylist.Add("jcode_auth_all");
            }
            else {
                var roleIds = await _userRelationIBLL.GetObjectIds(user.F_UserId, 1);
                roleIds.Add(user.F_UserId);
                mylist = (List<string>)await GetItemIdListByobjectIds(roleIds.ToArray(), itemType, parentId);
            }

            return mylist;
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 添加授权
        /// </summary>
        /// <param name="objectType">权限分类-1角色2用户</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="ids">功能Id</param>
        /// <param name="type">功能类型</param>
        /// <param name="parentId">父级Id</param>
        private async Task SaveAuthorize(int objectType, string objectId, IEnumerable<string> ids, int type,string parentId)
        {
            await authorizeService.SaveAuthorize(objectType, objectId, ids, type, parentId);
            _iCache.Remove($"Authorize_{objectId}_{type}_{parentId}");
        }

        /// <summary>
        /// 保存授权信息
        /// </summary>
        /// <param name="id">角色或用户id</param>
        /// <param name="authorizeDto">授权数据</param>
        /// <returns></returns>
        public async Task SaveAuthorize(string id, AuthorizeDto authorizeDto) {
            authorizeService.BeginTrans();
            try
            {
                if (authorizeDto.ItemType == 4 || authorizeDto.ItemType == 8 || authorizeDto.ItemType == 40  || authorizeDto.ItemType == 80)
                {
                    List<AuthorizeEntity> formList = new List<AuthorizeEntity>();
                    foreach (var item in authorizeDto.Ids) {
                        var idList = item.Split(",");
                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.F_CreateUserId = this.GetUserId();
                        authorizeEntity.F_CreateUserName = this.GetUserName();
                        authorizeEntity.F_CreateDate = DateTime.Now;

                        authorizeEntity.F_ParentId = authorizeDto.ParentId;
                        authorizeEntity.F_ObjectType = authorizeDto.ObjectType;
                        authorizeEntity.F_ObjectId = id;
                        authorizeEntity.F_ItemType = authorizeDto.ItemType;
                        authorizeEntity.F_Type =Convert.ToInt32(idList[1]);
                        authorizeEntity.F_ItemId = idList[0];
                        formList.Add(authorizeEntity);
                    }

                    await authorizeService.SaveAuthorize(authorizeDto.ObjectType, id, authorizeDto.ParentId, authorizeDto.ItemType, formList);
                }
                else {
                    // 获取当前的信息
                    var oldList = (List<AuthorizeEntity>)await GetList(id, authorizeDto.ItemType, authorizeDto.ParentId);
                    var addList = authorizeDto.Ids.FindAll(t => oldList.FindIndex(t2 => t2.F_ItemId == t) == -1);
                    var removeIds = new List<string>();

                    foreach (var item in oldList)
                    {
                        if (!authorizeDto.Ids.Contains(item.F_ItemId))
                        {
                            removeIds.Add(item.F_ItemId);
                        }
                    }

                    if (authorizeDto.ItemType == 1)// 功能模块
                    {
                        // 1.获取新增的按钮，列表，表单字段数据
                        foreach (var modlueId in addList)
                        {
                            var list = await GetMyItemIdList(2, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list, 2, modlueId);

                            var list2 = await GetMyItemIdList(3, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list2, 3, modlueId);

                            var list3 = await GetMyItemIdList(4, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list3, 4, modlueId);
                        }
                        if (removeIds.Count > 0)
                        {
                            await authorizeService.Remove(authorizeDto.ObjectType, id, removeIds);
                        }
                    }
                    else if (authorizeDto.ItemType == 10)// 功能模块
                    {
                        if (removeIds.Count > 0)
                        {
                            await authorizeService.Remove(authorizeDto.ObjectType, id, removeIds);
                        }
                    }
                    else if (authorizeDto.ItemType == 5) { // 移动功能
                        // 1.获取新增的按钮，列表，表单字段数据
                        foreach (var modlueId in addList)
                        {
                            var list = await GetMyItemIdList(6, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list, 6, modlueId);

                            var list2 = await GetMyItemIdList(7, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list2, 7, modlueId);

                            var list3 = await GetMyItemIdList(8, modlueId);
                            await SaveAuthorize(authorizeDto.ObjectType, id, list3, 8, modlueId);
                        }
                        if (removeIds.Count > 0) {
                            await authorizeService.Remove(authorizeDto.ObjectType, id, removeIds);
                        }
                    }

                    await authorizeService.AddAuthorize(authorizeDto.ObjectType, id, addList, authorizeDto.ItemType, authorizeDto.ParentId);
                    await authorizeService.Remove(authorizeDto.ObjectType, id, authorizeDto.ParentId,authorizeDto.ItemType, removeIds);
                }



                authorizeService.Commit();
                _iCache.Remove($"Authorize_{id}_{authorizeDto.ItemType}_{authorizeDto.ParentId}");
            }
            catch
            {
                authorizeService.Rollback();
                throw;
            }
        }



        /// <summary>
        /// 删除无效项
        /// </summary>
        /// <param name="itemIds">有效项</param>
        /// <param name="itemType">功能类型1菜单功能2按钮3视图列表4表单5App</param>
        /// <param name="parentId">父级Id</param>
        /// <returns></returns>
        public async Task RemoveInvalid(List<string> itemIds, int itemType, string parentId)
        {
            await authorizeService.RemoveInvalid(itemIds, itemType, parentId);
        }

        #endregion
    }
}
