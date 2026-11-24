using jcode.iapplication;
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
    /// 日 期：2019.09.25
    /// 描 述：用户关联对象
    /// </summary>
    public class UserRelationService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetObjectIdList(string userId, int category)
        {
            return this.BaseRepository().FindList<UserRelationEntity>(t=>t.F_UserId == userId && t.F_Category == category);
        }
        
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userIds">用户主键集合</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetObjectIdList(List<string> userIds, int category)
        {
            return this.BaseRepository().FindList<UserRelationEntity>(t=>userIds.Contains(t.F_UserId)  && t.F_Category == category);
        }

        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="strSql">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetObjectIdList(string strSql)
        {
            return this.BaseRepository().FindList<UserRelationEntity>(t=>t.F_Category == 1,"",strSql,new {});
        }
        
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="strSql">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetObjectIdList(List<string> list)
        {
            return this.BaseRepository().FindList<UserRelationEntity>(t=>list.Contains(t.F_ObjectId) && t.F_Category ==1 && (t.F_Type == 10 || t.F_Type == 11) );
        }
        
        /// <summary>
        /// 获取离职，禁用用户
        /// </summary>
        /// <returns></returns>
        private async Task<List<UserEntity>> GetRemoveUsers()
        {
            var list =(List<UserEntity>)await this.BaseRepository().FindList<UserEntity>(t=>t.F_DeleteMark == 1 || t.F_EnabledMark == 0 || t.F_EnabledMark == 2 || t.F_Type == 3 || t.F_IsBlack == 1);

            return list;
        }

        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联角色或岗位组件</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId)
        {
            var removeUsers = await GetRemoveUsers();
            var list = (List<UserRelationEntity>) await this.BaseRepository().FindList<UserRelationEntity>(t=>t.F_ObjectId == objectId);
            return list.FindAll(t => removeUsers.FindIndex(t2 => t2.F_UserId == t.F_UserId) == -1);
        }
                
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联角色或岗位或部门</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId,string attrCode)
        {
            var removeUsers = await GetRemoveUsers();
            var list = (List<UserRelationEntity>) await this.BaseRepository().FindList<UserRelationEntity>(t=>t.F_ObjectId == objectId && t.F_AttrCode == attrCode);
            return list.FindAll(t => removeUsers.FindIndex(t2 => t2.F_UserId == t.F_UserId) == -1);
        }
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联角色或岗位组件</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList)
        {
            var removeUsers = await GetRemoveUsers();
            var list = (List<UserRelationEntity>) await this.BaseRepository().FindList<UserRelationEntity>(t => objectIdList.Contains(t.F_ObjectId));
            return list.FindAll(t => removeUsers.FindIndex(t2 => t2.F_UserId == t.F_UserId) == -1);
        }
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联角色或岗位组件</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList,string attrCode)
        {
            var removeUsers = await GetRemoveUsers();
            var list = (List<UserRelationEntity>) await this.BaseRepository().FindList<UserRelationEntity>(t => objectIdList.Contains(t.F_ObjectId) && t.F_AttrCode == attrCode);
            return list.FindAll(t => removeUsers.FindIndex(t2 => t2.F_UserId == t.F_UserId) == -1);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 保存对象对应用户数据
        /// </summary>
        /// <param name="objectId">岗位或者角色主键</param>
        /// <param name="userRelationEntityList">列表</param>
        public async Task SaveEntityList(string objectId, IEnumerable<UserRelationEntity> userRelationEntityList)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<UserRelationEntity>(t => t.F_ObjectId == objectId);
                var userRelationEntityList2 =(List<UserRelationEntity>)userRelationEntityList;
                if (userRelationEntityList2.Count > 0) {
                    await db.Inserts(userRelationEntityList2);
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
        /// 保存用户对应对象数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="category"></param>
        /// <param name="userRelationEntityList"></param>
        /// <returns></returns>
        public async Task SaveEntityList(string userId,string userName,int category, IEnumerable<UserRelationEntity> userRelationEntityList)
        {
            
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<UserRelationEntity>(t => t.F_UserId == userId && t.F_Category == category);
                var addList = new List<UserRelationEntity>();
                foreach (UserRelationEntity userRelationEntity in userRelationEntityList)
                {
                    userRelationEntity.F_CreateDate = DateTime.Now;
                    userRelationEntity.F_UserId = userId;
                    userRelationEntity.F_Type = 1;
                    userRelationEntity.F_Name = userName;
                    userRelationEntity.F_AttrCode = userRelationEntity.F_AttrCode;
                    userRelationEntity.F_Id = Guid.NewGuid().ToString();
                    addList.Add(userRelationEntity);
                }
                if (addList.Count > 0) {
                    await db.Inserts(addList);
                }
                db.Commit();

            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
        }
        #endregion
    }
}
