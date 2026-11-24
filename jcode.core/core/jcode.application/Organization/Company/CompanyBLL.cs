using System;
using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：公司管理
    /// </summary>
    public class CompanyBLL : CompanyIBLL, BLL
    {
        #region 属性
        private readonly CompanyService companyService = new CompanyService();
        private readonly UserIBLL _userIBLL;
        private readonly DepartmentIBLL _departmentIBLL;
        private readonly UserRelationIBLL _userRelationIBLL;
        
        private readonly ICache _iCache;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userIBLL"></param>
        /// <param name="departmentIBLL"></param>
        /// <param name="userRelationIBLL"></param>
        /// <param name="iCache"></param>
        public CompanyBLL(UserIBLL userIBLL,DepartmentIBLL departmentIBLL,UserRelationIBLL userRelationIBLL,ICache iCache ) {
            _userIBLL = userIBLL;
            _departmentIBLL= departmentIBLL;
            _userRelationIBLL = userRelationIBLL;
            _iCache = iCache;
        }

        #endregion

        #region 获取数据
        /// <summary>
        /// 获取公司列表数据
        /// </summary>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public async Task<IEnumerable<CompanyEntity>> GetList(string keyWord = "")
        {
            var list = await companyService.GetList(keyWord);
            foreach (var item in list)
            {
                item.F_DingAppKey = "";
                item.F_DingAppSecret = "";
                item.F_DingAgentId = "";
            }
            return list;
        }

        /// <summary>
        /// 获取子公司列表信息
        /// </summary>
        /// <param name="pId">父级Id</param>
        /// <returns></returns>
        public Task<IEnumerable<CompanyEntity>> GetListByPId(string pId) {
            return companyService.GetListByPId(pId);
        }

        /// <summary>
        /// 获取公司信息实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<CompanyEntity> GetEntity(string keyValue)
        {
            string key = $"company_{keyValue}";
            var entity = _iCache.Read<CompanyEntity>(key);
            if (entity == null)
            {
                entity =await companyService.GetEntity(keyValue);
                var list =await _userRelationIBLL.GetUserIdList(keyValue);
                entity.DeptRoleList = list.ToJson();
                _iCache.Write(key,entity);
            }
            return entity;
        }
        /// <summary>
        /// 获取树形数据
        /// </summary>
        /// <param name="parentId">父级id</param>
        /// <returns></returns>
        public async Task<IEnumerable<TreeModel>> GetTree(string parentId) {
            List<CompanyEntity> list = (List<CompanyEntity>)await GetList();
            List<TreeModel> treeList = new List<TreeModel>();
            foreach (var item in list)
            {
                TreeModel node = new TreeModel
                {
                    id = item.F_CompanyId,
                    text = item.F_FullName,
                    value = item.F_CompanyId,
                    showcheck = false,
                    checkstate = 0,
                    isexpand = true,
                    parentId = item.F_ParentId
                };
                treeList.Add(node);
            }
            return treeList.ToTree(parentId);
        }
        /// <summary>
        /// 获取公司本身和子公司的id
        /// </summary>
        /// <param name="parentId">父级ID</param>
        /// <returns></returns>
        public async  Task<IEnumerable<string>> GetSubNodes(string parentId)
        {
            if (string.IsNullOrEmpty(parentId))
            {
                return new List<string>();
            }
            List<string> res = new List<string>();
            res.Add(parentId);
            List<TreeModel> list = (List<TreeModel>)await GetTree(parentId);
            GetSubNodes(list, res);
            return res;
        }
        /// <summary>
        /// 遍历树形数据获取全部子节点ID
        /// </summary>
        /// <param name="list">树形数据列表</param>
        /// <param name="outList">输出数据列表</param>
        private void GetSubNodes(List<TreeModel> list, List<string> outList) {
            foreach (var item in list) {
                outList.Add(item.id);
                if (item.hasChildren) {
                    GetSubNodes(item.ChildNodes, outList);
                }
            }
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公司信息
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            //删除时，需要判断所属公司下是否存在部门与人员，存在则无法删除
            List<DepartmentEntity> depts = (List<DepartmentEntity>)await _departmentIBLL.GetList(keyValue,string.Empty,string.Empty);
            List<UserEntity> users = (List<UserEntity>)await _userIBLL.GetList(keyValue,null,null);
            if (depts.Count>0)
            {
                throw new System.Exception("#NotLog#该公司存在关联部门信息，无法删除！");
            }
            if (users.Count>0)
            {
                throw new System.Exception("#NotLog#该公司存在关联用户信息，无法删除！");
            }
            await companyService.Delete(keyValue);
            string key = $"company_{keyValue}";
            _iCache.Remove(key);
        }
        /// <summary>
        /// 保存公司信息（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="companyEntity">公司实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, CompanyEntity companyEntity)
        {
            string parentId = companyEntity.F_ParentId;
            var deepNum = "C";
            
            
            await _iCache.Lock($"company_deep", async _ =>
            {
                
                if (parentId != "0")
                {
                    var parentEntity = await GetEntity(parentId);
                    if (parentEntity == null)
                    {
                        throw new Exception("父级公司已被删除");
                    }

                    deepNum = parentEntity.F_DeepNum + ".";
                }

                companyService.BeginTrans();
                try
                {
                    if (string.IsNullOrEmpty(keyValue))
                    {
                        var list =(List<CompanyEntity>)await GetListByPId(parentId);
                        companyEntity.F_DeepNum = deepNum + (list.Count + 1);
                    }
                    else if(string.IsNullOrEmpty(companyEntity.F_DeepNum) || companyEntity.F_DeepNum.IndexOf(deepNum, StringComparison.Ordinal) == -1)
                    {
                        var allList = (List<CompanyEntity>)await GetList();
                        var list =allList.FindAll(t=>t.F_ParentId == parentId);
                        companyEntity.F_DeepNum = deepNum + (list.Count + 1);

                        await UpdateDeepNums(keyValue,companyEntity.F_DeepNum + ".",allList);
                    }
                    await companyService.SaveEntity(keyValue, companyEntity);

                    

                    companyService.Commit();
                }
                catch (Exception)
                {
                    companyService.Rollback();
                    throw;
                }
            });
            
            if (!string.IsNullOrEmpty(companyEntity.DeptRoleList))
            {
                var list = companyEntity.DeptRoleList.ToObject<List<UserRelationEntity>>();
                await _userRelationIBLL.SaveEntityList(companyEntity.F_CompanyId, 4, list);
            }
            
            if (!string.IsNullOrEmpty(keyValue))
            {
                string key = $"company_{keyValue}";
                _iCache.Remove(key);
            }
            
        }
        /// <summary>
        /// 保存公司对应的企业微信部门ID
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="wechatDepId">企业微信部门ID</param>
        /// <returns></returns>
        public async Task SaveWechatDepId(string companyId, string wechatDepId)
        {
            var entity = await companyService.GetEntity(companyId);
            if (entity == null || entity.F_WechatDepId == wechatDepId)
            {
                return;
            }
            entity.F_WechatDepId = wechatDepId;
            await companyService.SaveEntity(companyId, entity);
            _iCache.Remove($"company_{companyId}");
        }
        #endregion

        #region 层级编码

        /// <summary>
        /// 给老数据添加层级编码
        /// </summary>
        public async Task InitDeepNum()
        {
            var allList = (List<CompanyEntity>)await GetList();
            await UpdateDeepNums("0","C",allList);
        }

        /// <summary>
        /// 更新层级编码
        /// </summary>
        private async Task UpdateDeepNums(string parnetId,string deepNum,List<CompanyEntity> allList)
        {
            var list = allList.FindAll(t => t.F_ParentId == parnetId);
            int num = 1;
            foreach (var item in list)
            {
                item.F_DeepNum = $"{deepNum}{num}";
                await companyService.SaveEntity(item.F_CompanyId,item);
                await UpdateDeepNums(item.F_CompanyId,item.F_DeepNum + ".",allList);
                num++;
            }
        }
        #endregion
    }
}
