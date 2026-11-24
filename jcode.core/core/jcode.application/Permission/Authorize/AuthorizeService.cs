using jcode.orm;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Text;
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
    public class AuthorizeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取授权功能主键数据列表
        /// </summary>
        /// <param name="objectId">对象主键（角色,用户）</param>
        /// <param name="itemType">项目类型:1-菜单2-按钮3-视图4-表单5-app</param>
        /// <returns></returns>
        public Task<IEnumerable<AuthorizeEntity>> GetList(string objectId, int itemType)
        {
            return this.BaseRepository().FindList<AuthorizeEntity>(t=>t.F_ObjectId == objectId && t.F_ItemType == itemType);
        }

        /// <summary>
        /// 获取授权功能主键数据列表
        /// </summary>
        /// <param name="objectId">对象主键（角色,用户）</param>
        /// <param name="itemType">项目类型:1-菜单2-按钮3-视图4-表单5-app</param>
        /// <param name="parentId">父级主键</param>
        /// <returns></returns>
        public Task<IEnumerable<AuthorizeEntity>> GetList(string objectId, int itemType,string parentId)
        {
            return this.BaseRepository().FindList<AuthorizeEntity>(t => t.F_ObjectId == objectId && t.F_ItemType == itemType && t.F_ParentId == parentId);
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
        /// <param name="parentId">父级主键</param>
        public async Task SaveAuthorize(int objectType, string objectId, IEnumerable<string> ids, int type, string parentId)
        {
            IRepository db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<AuthorizeEntity>(t=>t.F_ObjectId == objectId && t.F_ItemType == type && t.F_ParentId == parentId);

                #region 功能
                if (ids != null) {
                    List<AuthorizeEntity> adds = new List<AuthorizeEntity>();
                    foreach (string item in ids)
                    {
                        AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                        authorizeEntity.F_CreateUserId = this.GetUserId();
                        authorizeEntity.F_CreateUserName = this.GetUserName();
                        authorizeEntity.F_CreateDate = DateTime.Now;

                        authorizeEntity.F_ParentId = parentId;
                        authorizeEntity.F_ObjectType = objectType;
                        authorizeEntity.F_ObjectId = objectId;
                        authorizeEntity.F_ItemType = type;
                        authorizeEntity.F_ItemId = item;
                        if (type == 4 || type == 8) {
                            authorizeEntity.F_Type = 2;
                        }
                        adds.Add(authorizeEntity);
                    }

                    await db.Inserts(adds);
                }
                #endregion

                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="objectType">权限分类-1角色2用户</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="parentId">父级对象</param>
        /// <param name="itemType">类型</param>
        /// <param name="ids">功能Id</param>
        /// <returns></returns>
        public async Task Remove(int objectType, string objectId,string parentId,int itemType, List<string> ids) {
            await this.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ObjectType == objectType
                && t.F_ObjectId == objectId
                && t.F_ParentId == parentId
                && t.F_ItemType == itemType
                && ids.Contains(t.F_ItemId) );
        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="objectType">权限分类-1角色2用户</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="parentIds">父级id集合</param>
        /// <returns></returns>
        public async Task Remove(int objectType, string objectId, List<string> parentIds)
        {
            await this.BaseRepository().Delete<AuthorizeEntity>(t =>
                t.F_ObjectType == objectType
                && t.F_ObjectId == objectId
                && parentIds.Contains(t.F_ParentId));
        }

        /// <summary>
        /// 添加授权
        /// </summary>
        /// <param name="objectType">权限分类-1角色2用户</param>
        /// <param name="objectId">对象Id</param>
        /// <param name="ids">功能Id</param>
        /// <param name="type">功能类型</param>
        /// <param name="parentId">父级主键</param>
        public async Task AddAuthorize(int objectType, string objectId, IEnumerable<string> ids, int type, string parentId)
        {
            if (ids != null)
            {
                List<AuthorizeEntity> list = new List<AuthorizeEntity>();
                foreach (string item in ids)
                {
                    AuthorizeEntity authorizeEntity = new AuthorizeEntity();
                    authorizeEntity.F_CreateUserId = this.GetUserId();
                    authorizeEntity.F_CreateUserName = this.GetUserName();
                    authorizeEntity.F_CreateDate = DateTime.Now;

                    authorizeEntity.F_ParentId = parentId;
                    authorizeEntity.F_ObjectType = objectType;
                    authorizeEntity.F_ObjectId = objectId;
                    authorizeEntity.F_ItemType = type;
                    authorizeEntity.F_ItemId = item;
                    list.Add(authorizeEntity);
                }
                if (list.Count > 0) {
                    await this.BaseRepository().Inserts(list);
                }
            }
        }


        /// <summary>
        /// 保存表单权限数据
        /// </summary>
        /// <param name="objectType"></param>
        /// <param name="objectId"></param>
        /// <param name="parentId"></param>
        /// <param name="type"></param>
        /// <param name="list"></param>
        /// <returns></returns>
        public async Task SaveAuthorize(int objectType, string objectId, string parentId, int type, List<AuthorizeEntity> list) {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<AuthorizeEntity>(t => t.F_ObjectType == objectType
                    && t.F_ObjectId == objectId
                    && t.F_ParentId == parentId && t.F_ItemType == type);
                await db.Inserts(list);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
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
            await this.BaseRepository().Delete<AuthorizeEntity>(t => t.F_ItemType == itemType && t.F_ParentId == parentId && itemIds.IndexOf(t.F_ItemId) == -1);
        }

        #endregion
    }
}
