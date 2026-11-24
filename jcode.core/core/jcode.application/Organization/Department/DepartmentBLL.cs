using System;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentBLL : DepartmentIBLL, BLL
    {
        #region 属性
        private readonly DepartmentService departmentService = new DepartmentService();
        private readonly UserIBLL _userIBLL;
        private readonly UserRelationIBLL _userRelationIBLL;
        
        private readonly ICache _iCache;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="iCache"></param>
        public DepartmentBLL(UserIBLL userIBLL,UserRelationIBLL userRelationIBLL, ICache iCache)
        {
            _userIBLL = userIBLL; 
            _userRelationIBLL = userRelationIBLL;
            _iCache = iCache;
        }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <param name="companyId">公司Id</param>
        /// <param name="parentId">上级部门</param>
        /// <param name="type">部门类型</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetList(string companyId,string parentId,string type,string keyWord = "")
        {
            return departmentService.GetList(companyId,parentId,type,keyWord);
        }

        /// <summary>
        /// 获取部门列表信息(根据公司Id 和 父级 id)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="pid">父级 id</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetListByPid(string companyId, string pid)
        {
            return departmentService.GetListByPid(companyId, pid);
        }

        /// <summary>
        /// 获取部门列表信息
        /// </summary>
        /// <param name="keyWord">关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetAllList(string keyWord) {
            return departmentService.GetAllList(keyWord);
        }

        /// <summary>
        /// 获取部门列表信息(根据部门ID集合)
        /// </summary>
        /// <param name="keyValues">部门ID集合</param>
        /// <returns></returns>
        public async Task<IEnumerable<DepartmentEntity>> GetListByKeys(string[] keyValues)
        {
            var ids = new List<string>();
            var res = new List<DepartmentEntity>();
            foreach (var id in keyValues)
            {
                string key = $"department_{id}";
                var entity = _iCache.Read<DepartmentEntity>(key);
                if (entity == null)
                {
                    ids.Add(id);
                }
                else
                {
                    res.Add(entity); 
                }
            }

            if (ids.Count > 0)
            {
                var list = await departmentService.GetListByKeys(ids.ToArray());
                foreach (var item in list)
                {
                    string key = $"department_{item.F_DepartmentId}";
                    _iCache.Write(key,item);
                }
                res.AddRange(list);
            }
            
            return res;
        }
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<DepartmentEntity> GetEntity(string keyValue)
        {
            string key = $"department_{keyValue}";
            var entity = _iCache.Read<DepartmentEntity>(key);
            if (entity == null)
            {
                entity = await departmentService.GetEntity(keyValue);
                if (entity != null)
                {
                    var list =await _userRelationIBLL.GetUserIdList(keyValue);
                    entity.DeptRoleList = list.ToJson();
                    _iCache.Write(key,entity);
                }
            }
            return entity;
        }
        /// <summary>
        /// 获取部门本身和子部门的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetSubNodes(string parentId)
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(parentId))
            {
                return res;
            }
            var entity = await GetEntity(parentId);
            if (entity == null)
            {
                return res;
            }
            res.Add(parentId);
            var list =await departmentService.GetSubs(entity.F_DeepNum + ".");
            foreach (var item in list)
            {
                  res.Add(item.F_DepartmentId);
            }

           
            return res;
        }
        
        /// <summary>
        /// 获取部门本身和父级部门的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public async Task<IEnumerable<string>> GetParentNodes(string parentId)
        {
            List<string> res = new List<string>();
            if (string.IsNullOrEmpty(parentId))
            {
                return res;
            }
            var entity = await GetEntity(parentId);
            if (entity == null)
            {
                return res;
            }
            res.Add(parentId);
            
         
            var deepNums =new List<string>();
            deepNums.Add(entity.F_DeepNum);
            var myDeepNums = entity.F_DeepNum.Split(".");
            var deepNum = "";
            foreach (var item in myDeepNums)
            {
                deepNum += item;
                deepNums.Add(deepNum);
                deepNum += ".";
            }
            
            
            var list =await departmentService.GetList(deepNums);
            foreach (var item in list)
            {
                res.Add(item.F_DepartmentId);
            }
            
            return res;
        }
        
        /// <summary>
        /// 获取完整部门链名称
        /// </summary>
        /// <param name="id">部门id</param>
        /// <returns></returns>
        public async Task<string> GetFullName(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return "";
            }
            var entity = await GetEntity(id);
            if (entity == null)
            {
                return "";
            }
            var list =(List<DepartmentEntity>)await departmentService.GetParentListByDeepNum(entity.F_DeepNum);
            return GetFullName(entity,list);
        }

        private string GetFullName(DepartmentEntity entity, List<DepartmentEntity> list)
        {
            var res = entity.F_FullName;
            var pEntity = list.Find(t => t.F_DepartmentId == entity.F_ParentId);
            if (pEntity != null)
            {
                var pRes = GetFullName(pEntity,list);
                res = pRes + "-" + res;
            }
            return res;
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            //删除时，需要判断该部门下是否存在人员，存在则无法删除
            List<UserEntity> users = (List<UserEntity>)await _userIBLL.GetList(null, keyValue, null);
            if (users.Count>0)
            {
                throw new System.Exception("#NotLog#该部门存在关联用户信息，无法删除！");
            }
            await departmentService.Delete(keyValue);

            string key = $"department_{keyValue}";
            _iCache.Remove(key);
        }
        /// <summary>
        /// 保存部门信息（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">部门实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DepartmentEntity departmentEntity)
        {
            string parentId = departmentEntity.F_ParentId;
            var deepNum = "D";
            
            
            await _iCache.Lock($"department_deep", async _ =>
            {
                
                if (parentId != "0")
                {
                    var parentEntity = await GetEntity(parentId);
                    if (parentEntity == null)
                    {
                        throw new Exception("父级部门已被删除");
                    }

                    deepNum = parentEntity.F_DeepNum + ".";
                }

                departmentService.BeginTrans();
                try
                {
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var list =(List<DepartmentEntity>)await GetListByPid(string.Empty, parentId);
                        departmentEntity.F_DeepNum = deepNum + (list.Count + 1);
                    }
                    else if(string.IsNullOrEmpty(departmentEntity.F_DeepNum)  || departmentEntity.F_DeepNum.IndexOf(deepNum, StringComparison.Ordinal) == -1)
                    {
                        var allList = (List<DepartmentEntity>)await GetAllList(string.Empty);
                        var list =allList.FindAll(t=>t.F_ParentId == parentId);
                        departmentEntity.F_DeepNum = deepNum + (list.Count + 1);
                    
                        
                        await UpdateDeepNums(keyValue,departmentEntity.F_DeepNum + ".",allList);
                    }
                    await departmentService.SaveEntity(keyValue, departmentEntity);

                    

                    departmentService.Commit();
                }
                catch (Exception)
                {
                    departmentService.Rollback();
                    throw;
                }
            });
            
            
            
          
            if (!string.IsNullOrEmpty(departmentEntity.DeptRoleList))
            {
                var list = departmentEntity.DeptRoleList.ToObject<List<UserRelationEntity>>();
                await _userRelationIBLL.SaveEntityList(departmentEntity.F_DepartmentId, 3, list);
            }

            
            
            if (!string.IsNullOrEmpty(keyValue))
            {
                string key = $"department_{keyValue}";
                _iCache.Remove(key);
            }
        }
        /// <summary>
        /// 保存部门信息-批量（新增、修改）----ADD BY SSY 20220929
        /// </summary>
        /// <param name="departmentEntitys">部门实体</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<DepartmentEntity> departmentEntitys)
        {
            await departmentService.SaveEntitys(departmentEntitys);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取部门本身和子部门的id
        /// </summary>
        /// <param name="WechatDeptId">同步后的企微部门ID</param>
        /// <returns></returns>
        public async Task<DepartmentEntity> GetEntityByWechatDeptId(string WechatDeptId)
        {
            return await departmentService.GetEntityByWechatDeptId(WechatDeptId);
        }
        #endregion
        
        
        #region 层级编码

        /// <summary>
        /// 给老数据添加层级编码
        /// </summary>
        public async Task InitDeepNum()
        {
            var allList = (List<DepartmentEntity>)await GetAllList(string.Empty);
            await UpdateDeepNums("0","D",allList);
        }

        /// <summary>
        /// 更新层级编码
        /// </summary>
        private async Task UpdateDeepNums(string parnetId,string deepNum,List<DepartmentEntity> allList)
        {
            var list = allList.FindAll(t => t.F_ParentId == parnetId);
            int num = 1;
            foreach (var item in list)
            {
                item.F_DeepNum = $"{deepNum}{num}";
                await departmentService.SaveEntity(item.F_DepartmentId,item);
                await UpdateDeepNums(item.F_DepartmentId,item.F_DeepNum + ".",allList);
                num++;
            }
        }
        #endregion
    }
}
