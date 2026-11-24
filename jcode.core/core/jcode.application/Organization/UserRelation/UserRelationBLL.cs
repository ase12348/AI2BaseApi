using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.util;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.25
    /// 描 述：用户关联对象
    /// </summary>
    public class UserRelationBLL: UserRelationIBLL,BLL
    {
        private readonly UserRelationService  userRelationService = new UserRelationService();

        #region 获取数据
                
        
        
        
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetObjectIdList(string userId, int category)
        {
            return userRelationService.GetObjectIdList(userId, category);
        }
        /// <summary>
        /// 获取对象主键列表信息
        /// </summary>
        /// <param name="userId">用户主键</param>
        /// <param name="category">分类:1-角色2-岗位</param>
        /// <returns></returns>
        public async Task<List<string>> GetObjectIds(string userId, int category)
        {

                var res = new List<string>();
                if (category == 1)
                {
                    var userBLL = IocManager.Instance.GetService<UserIBLL>();
                    var user = await userBLL.GetEntity(userId);
                    // 角色 1.用户 2.部门 3.密级 4.行政级别 5.行政职务 6.技术职务 7.职称等级 8.管理职务 9.集成角色  10.限定部门 11.排除部门 12公司 12 部门包含子部门
                    // 1.用户
                    string sql = $"(f_UserId = '{user.F_UserId}' AND f_Type = 1 ) ";
                        
                    
                    // 2.部门
                    // var departmentId = this.GetP
                    if (!string.IsNullOrEmpty(user.F_DepartmentId))
                    {
                        sql += $" OR (f_UserId = '{user.F_DepartmentId}' AND f_Type = 2) ";


                        var departmentIBLL = IocManager.Instance.GetService<DepartmentIBLL>();
                        var departmentIds = await departmentIBLL.GetParentNodes(user.F_DepartmentId);
                        foreach (var departmentId in departmentIds)
                        {
                            sql += $" OR (f_UserId = '{departmentId}' AND f_Type = 13) ";
                        }

                    }
                    
                    // 12.公司
                    if (!string.IsNullOrEmpty(user.F_CompanyId))
                    {
                        sql += $" OR (f_UserId = '{user.F_CompanyId}' AND f_Type = 12) ";
                    }
                    
                    // 3.密级
                    if (user.F_SecretLevel != null)
                    {
                        sql += $" OR (f_UserId = '{user.F_SecretLevel}' AND f_Type = 3) ";
                    }
                    
                    // 4.行政级别
                    if (!string.IsNullOrEmpty(user.F_Level))
                    {
                        sql += $" OR (f_UserId = '{user.F_Level}' AND f_Type = 4) ";
                    }
                    
                    // 5.行政职务
                    if (!string.IsNullOrEmpty(user.F_Job))
                    {
                        sql += $" OR (f_UserId = '{user.F_Job}' AND f_Type = 5) ";
                    }
                    
                    // 6.技术职务
                    if (!string.IsNullOrEmpty(user.F_Role))
                    {
                        sql += $" OR (f_UserId = '{user.F_Role}' AND f_Type = 6) ";
                    }
                    
                    // 7.职称等级
                    if (!string.IsNullOrEmpty(user.F_Title))
                    {
                        sql += $" OR (f_UserId = '{user.F_Title}' AND f_Type = 7) ";
                    }
                    
                    // 8.管理职务
                    if (!string.IsNullOrEmpty(user.F_Post))
                    {
                        sql += $" OR (f_UserId = '{user.F_Post}' AND f_Type = 8) ";
                    }
                    


                    var list = await userRelationService.GetObjectIdList(sql);
                    if (list != null)
                    {
                        foreach (UserRelationEntity item in list)
                        {
                            res.Add(item.F_ObjectId);
                        }
                        // 获取限定部门 和 排除部门
                        var list2 = await userRelationService.GetObjectIdList(res);
                        Dictionary<string, List<string>> limitDic = new Dictionary<string, List<string>>();
                        Dictionary<string, List<string>> outDic = new Dictionary<string, List<string>>();
                        foreach (UserRelationEntity item in list2)
                        {
                            if (item.F_Type == 10)
                            {
                                if (!limitDic.ContainsKey(item.F_ObjectId))
                                {
                                    limitDic.Add(item.F_ObjectId,new List<string>());
                                }
                            
                                limitDic[item.F_ObjectId].Add(item.F_UserId);
                            }
                            else
                            {
                                if (!outDic.ContainsKey(item.F_ObjectId))
                                {
                                    outDic.Add(item.F_ObjectId,new List<string>());
                                }
                                limitDic[item.F_ObjectId].Add(item.F_UserId);
                            }
                        
                        
                        }


                        var _res = new List<string>();
                        foreach (var id in res)
                        {
                            if (!outDic.ContainsKey(id) || outDic[id].FindIndex(t => t == user.F_DepartmentId) == -1)
                            {
                                if (!limitDic.ContainsKey(id) || outDic[id].FindIndex(t => t == user.F_DepartmentId) != -1)
                                {
                                    _res.Add(id);
                                }
                            }
                        }


                        res = _res;
                    }
                    


                }
                else
                {
                    var list = (List<UserRelationEntity>)await GetObjectIdList(userId, category);
                    if (category == 2)
                    {
                        // 如果是岗位，主岗放第一个
                        var mainPost = list.Find(t => t.F_AttrCode == "1");
                        if (mainPost != null)
                        {
                            res.Add(mainPost.F_ObjectId);
                        }
                        foreach (UserRelationEntity item in list)
                        {
                            if (mainPost == null || item.F_ObjectId != mainPost.F_ObjectId)
                            {
                                res.Add(item.F_ObjectId);
                            }
                        }
                    }
                    else
                    {
                        foreach (UserRelationEntity item in list)
                        {
                            res.Add(item.F_ObjectId);
                        }
                    }

                  
                }




            return res;
        }


        
        /// <summary>
        /// 获取登录者当前岗位
        /// </summary>
        /// <returns></returns>
        private async Task<string> GetPostId(string userId)
        {

            var postList = await GetObjectIds(userId, 2);
            if (postList.Count > 1) // 大于1个岗位才需要设置主主岗所在部门
            {
                var postId = ContextHelper.GetItem("postId") as string;
                if (!string.IsNullOrEmpty(postId) && postList.IndexOf(postId) > -1 )
                {
                    return  postId;
                }
                else
                {
                    postId = postList[0];
                    ContextHelper.SetItem("postId", postId);
                    return postId;
                }
            }
            ContextHelper.SetItem("postId", "");
            return "";
        }
        /// <summary>
        /// 获取登录者角色
        /// </summary>
        /// <returns></returns>
        public async Task<List<string>> GetRoleIds()
        {
            var userId = ContextHelper.GetItem("userId") as string;
            
            
            

            var postId = await GetPostId(userId);


                var userBLL = IocManager.Instance.GetService<UserIBLL>();
                var user = await userBLL.GetEntity(userId);
                var departmentId = user.F_DepartmentId;
                var companyId = user.F_CompanyId;

                if (!string.IsNullOrEmpty(postId))
                {
                    var postIBLL = IocManager.Instance.GetService<PostIBLL>();
                    var postEntity = await postIBLL.GetEntity(postId);
                    departmentId = postEntity.F_DepartmentId;
                    companyId = postEntity.F_CompanyId;
                }

                
                
                var res = new UserRelationRoleDto();
                res.Roles = new List<string>();

                res.PostId = postId;

                // 角色 1.用户 2.部门 3.密级 4.行政级别 5.行政职务 6.技术职务 7.职称等级 8.管理职务 9.集成角色  10.限定部门 11.排除部门 12公司 12 部门包含子部门
                // 1.用户
                string sql = $"(f_UserId = '{user.F_UserId}' AND f_Type = 1 ) ";
                        
                    
                // 2.部门
                if (!string.IsNullOrEmpty(departmentId))
                {
                    sql += $" OR (f_UserId = '{departmentId}' AND f_Type = 2) ";


                    var departmentIBLL = IocManager.Instance.GetService<DepartmentIBLL>();
                    var departmentIds = await departmentIBLL.GetParentNodes(departmentId);
                    foreach (var depId in departmentIds)
                    {
                        sql += $" OR (f_UserId = '{depId}' AND f_Type = 13) ";
                    }

                }
                    
                // 12.公司
                if (!string.IsNullOrEmpty(companyId))
                {
                    sql += $" OR (f_UserId = '{companyId}' AND f_Type = 12) ";
                }
                    
                // 3.密级
                if (user.F_SecretLevel != null)
                {
                    sql += $" OR (f_UserId = '{user.F_SecretLevel.ToString()}' AND f_Type = 3) ";
                }
                    
                // 4.行政级别
                if (!string.IsNullOrEmpty(user.F_Level))
                {
                    sql += $" OR (f_UserId = '{user.F_Level}' AND f_Type = 4) ";
                }
                    
                // 5.行政职务
                if (!string.IsNullOrEmpty(user.F_Job))
                {
                    sql += $" OR (f_UserId = '{user.F_Job}' AND f_Type = 5) ";
                }
                    
                // 6.技术职务
                if (!string.IsNullOrEmpty(user.F_Role))
                {
                    sql += $" OR (f_UserId = '{user.F_Role}' AND f_Type = 6) ";
                }
                    
                // 7.职称等级
                if (!string.IsNullOrEmpty(user.F_Title))
                {
                    sql += $" OR (f_UserId = '{user.F_Title}' AND f_Type = 7) ";
                }
                
                // 8.管理职务
                if (!string.IsNullOrEmpty(user.F_Post))
                {
                    sql += $" OR (f_UserId = '{user.F_Post}' AND f_Type = 8) ";
                }
                    


                var list = await userRelationService.GetObjectIdList(sql);
                if (list != null)
                {
                    foreach (UserRelationEntity item in list)
                    {
                        res.Roles.Add(item.F_ObjectId);
                    }
                    // 获取限定部门 和 排除部门
                    var list2 = await userRelationService.GetObjectIdList(res.Roles);
                    Dictionary<string, List<string>> limitDic = new Dictionary<string, List<string>>();
                    Dictionary<string, List<string>> outDic = new Dictionary<string, List<string>>();
                    foreach (UserRelationEntity item in list2)
                    {
                        if (item.F_Type == 10)
                        {
                            if (!limitDic.ContainsKey(item.F_ObjectId))
                            {
                                limitDic.Add(item.F_ObjectId,new List<string>());
                            }
                        
                            limitDic[item.F_ObjectId].Add(item.F_UserId);
                        }
                        else
                        {
                            if (!outDic.ContainsKey(item.F_ObjectId))
                            {
                                outDic.Add(item.F_ObjectId,new List<string>());
                            }
                            limitDic[item.F_ObjectId].Add(item.F_UserId);
                        }
                    
                    
                    }


                    var _res = new List<string>();
                    foreach (var id in res.Roles)
                    {
                        if (!outDic.ContainsKey(id) || outDic[id].FindIndex(t => t == departmentId) == -1)
                        {
                            if (!limitDic.ContainsKey(id) || outDic[id].FindIndex(t => t == departmentId) != -1)
                            {
                                _res.Add(id);
                            }
                        }
                    }


                    res.Roles = _res;
                }


              
            
            return res.Roles;
        }




        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联或角色主键集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId)
        {
            if (string.IsNullOrEmpty(objectId))
            {
                return new List<UserRelationEntity>();
            }

            return await userRelationService.GetUserIdList(objectId);
        }
        
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectId">关联角色或岗位或部门</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetUserIdList(string objectId,string attrCode)
        {
            return userRelationService.GetUserIdList(objectId,attrCode);
        }
        
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList)
        {
            return userRelationService.GetUserIdList(objectIdList);
        }
        /// <summary>
        /// 获取用户主键列表信息
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <param name="attrCode">属性编码</param>
        /// <returns></returns>
        public Task<IEnumerable<UserRelationEntity>> GetUserIdList(IEnumerable<string> objectIdList,string attrCode)
        {
            return userRelationService.GetUserIdList(objectIdList,attrCode);
        }
        
        
        /// <summary>
        /// 获取用户信息通过角色
        /// </summary>
        /// <param name="objectId">关联或角色主键集合</param>
        /// <returns></returns>
        public async Task<List<UserEntity>> GetUserIdByRoleList(string objectId)
        {
            var list = await userRelationService.GetUserIdList(objectId);
            return await GetUserList(list);
        }
       
        /// <summary>
        /// 获取用户信息通过角色a
        /// </summary>
        /// <param name="objectIdList">关联或角色主键集合</param>
        /// <returns></returns>
        public async Task<List<UserEntity>> GetUserIdByRoleList(IEnumerable<string> objectIdList)
        {
            var list =(List<UserRelationEntity>) await userRelationService.GetUserIdList(objectIdList);
            List<UserEntity> List = new List<UserEntity>();
            Dictionary<string,bool > dic = new Dictionary<string,bool>();
            foreach (var objectId in objectIdList)
            {
               var userList = await GetUserList(list);
               foreach (var user in userList)
               {
                   if (!dic.ContainsKey(user.F_UserId))
                   {
                       dic.Add(user.F_UserId,true);
                       userList.Add(user);
                   }
               }
            }
            return List;
        }


        private async Task<List<UserEntity>> GetUserList(IEnumerable<UserRelationEntity> list)
        {
            // 1.用户
            List<string> users = new List<string>();
            // 2.部门
            List<string> departments = new List<string>();
            // 12.公司
            List<string> companys = new List<string>();
            // 3.密级
            List<string> secretLevels = new List<string>();
            // 4.行政级别
            List<string> levels = new List<string>();
            // 5.行政职务
            List<string> jobs = new List<string>();
            // 6.技术职务
            List<string> roles = new List<string>();
            // 7.职称等级
            List<string> titles = new List<string>();
            // 8.管理职务
            List<string> posts = new List<string>();
            
            // 10.限定部门
            List<string> limitDepartments = new List<string>();
            // 11.排除角色
            List<string> outDepartments = new List<string>();
            
            
            foreach (var item  in list)
            {
                switch (item.F_Type)
                {
                    case 1:// 人员
                        users.Add(item.F_UserId);
                        break;
                    case 2:// 部门
                        departments.Add(item.F_UserId);
                        break;
                    case 3:// 密级
                        secretLevels.Add(item.F_UserId);
                        break;
                    case 4:// 行政级别
                        levels.Add(item.F_UserId);
                        break;
                    case 5:// 行政职务
                        jobs.Add(item.F_UserId);
                        break;
                    case 6:// 技术职务
                        roles.Add(item.F_UserId);
                        break;
                    case 7:// 职称等级
                        titles.Add(item.F_UserId);
                        break;
                    case 8:// 管理职务
                        posts.Add(item.F_UserId);
                        break;
                    case 10:// 限定部门
                        limitDepartments.Add(item.F_UserId);
                        break;
                    case 11:// 排除角色
                        outDepartments.Add(item.F_UserId);
                        break;
                    case 12:// 公司
                        companys.Add(item.F_UserId);
                        break;
                    case 13:// 子部门
                        var departmentIBLL = IocManager.Instance.GetService<DepartmentIBLL>();
                        var departmentIds = await departmentIBLL.GetSubNodes(item.F_UserId);
                        departments.AddRange(departmentIds);
                        break;
                        
                }
            }
            
            var userBLL = IocManager.Instance.GetService<UserIBLL>();
            return (List<UserEntity>)await userBLL.GetList(users,departments,companys,secretLevels,levels,jobs,roles,titles,posts,limitDepartments,outDepartments);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 保存对象对应用户数据
        /// </summary>
        /// <param name="objectId">对应对象主键</param>
        /// <param name="category">分类:1-角色2-岗位-3部门角色-4公司角色</param>
        /// <param name="userIds">对用户主键列表</param>
        public async Task SaveEntityList(string objectId, int category, string userIds)
        {
            List<UserRelationEntity> list = new List<UserRelationEntity>();
            if (!string.IsNullOrEmpty(userIds)) {
                string[] userIdList = userIds.Split(',');
                foreach (string userId in userIdList)
                {
                    UserRelationEntity userRelationEntity = new UserRelationEntity();

                    userRelationEntity.F_Id = Guid.NewGuid().ToString();
                    userRelationEntity.F_CreateDate = DateTime.Now;

                    userRelationEntity.F_UserId = userId;
                    userRelationEntity.F_Category = category;
                    userRelationEntity.F_ObjectId = objectId;
                    list.Add(userRelationEntity);
                }
            }
            await userRelationService.SaveEntityList(objectId, list);
            
        }

        /// <summary>
        /// 保存对象对应用户数据
        /// </summary>
        /// <param name="objectId">对应对象主键</param>
        /// <param name="category">分类:1-角色2-岗位-3部门角色-4公司角色</param>
        /// <param name="userRelationEntityList"></param>
        public async Task SaveEntityList(string objectId, int category, IEnumerable<UserRelationEntity> userRelationEntityList)
        {
            foreach (var userRelationEntity in userRelationEntityList)
            {
                userRelationEntity.F_Id = Guid.NewGuid().ToString();
                userRelationEntity.F_CreateDate = DateTime.Now;
                userRelationEntity.F_Category = category;
                userRelationEntity.F_ObjectId = objectId;
            }
            await userRelationService.SaveEntityList(objectId, userRelationEntityList);
        }

        /// <summary>
        /// 保存用户对应对象数据
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="category">分类:1-角色2-岗位-3部门角色</param>
        /// <param name="userRelationEntityList"></param>
        /// <returns></returns>
        public async Task SaveEntityList(string userId,string userName, int category, IEnumerable<UserRelationEntity> userRelationEntityList)
        {
            await userRelationService.SaveEntityList(userId,userName, category, userRelationEntityList);

        }
        #endregion
    }
}
