using jcode.iapplication;
using SqlSugar;
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
    /// 日 期：2019.09.24
    /// 描 述：部门管理
    /// </summary>
    public class DepartmentService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="parentId">上级部门</param>
        /// <param name="type">部门类型</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetList(string companyId,string parentId ,string type,string keyWord)
        {
            var exp = Expressionable.Create<DepartmentEntity>()
                .And(t => t.F_EnabledMark == 1)
                .And(t => t.F_DeleteMark == 0)
                .AndIF(!string.IsNullOrEmpty(companyId) && companyId != "all",t => t.F_CompanyId == companyId)
               
                .AndIF(!string.IsNullOrEmpty(type),t=>t.F_DeptTag == type)
                .AndIF(!string.IsNullOrEmpty(parentId),t=>t.F_ParentId == parentId)
                .AndIF(!string.IsNullOrEmpty(keyWord),t=>t.F_FullName.Contains(keyWord)
                               || t.F_EnCode.Contains(keyWord)
                               || t.F_ShortName.Contains(keyWord)
                              )
                .ToExpression();
            return this.BaseRepository().FindList(exp, "F_CompanyId,F_SortCode,F_EnCode,F_ParentId,F_FullName");
        }
        
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="deepNums">层级编码</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetList(List<string> deepNums)
        {
            return this.BaseRepository().FindList<DepartmentEntity>(t=>t.F_EnabledMark == 1 && t.F_DeleteMark == 0 && deepNums.Contains(t.F_DeepNum));
        }
        
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="deepNum">层级编码</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetParentListByDeepNum(string deepNum)
        {
            return this.BaseRepository().FindList<DepartmentEntity>(t=>t.F_EnabledMark == 1 && t.F_DeleteMark == 0 && deepNum.Contains(t.F_DeepNum));
        }

        /// <summary>
        /// 获取部门列表信息(根据公司Id 和 父级 id)
        /// </summary>
        /// <param name="companyId">公司主键</param>
        /// <param name="pid">父级 id</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetListByPid(string companyId,string pid)
        {
            var exp = Expressionable.Create<DepartmentEntity>()
                .And(t => t.F_EnabledMark == 1)
                .And(t => t.F_DeleteMark == 0)
                .And(t => t.F_ParentId == pid)
                .AndIF(!string.IsNullOrEmpty(companyId),t => t.F_CompanyId == companyId)
                .ToExpression();
            return this.BaseRepository().FindList(exp, "F_SortCode,F_EnCode,F_FullName");
        }

        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <param name="keyWord">公司主键</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetAllList(string keyWord)
        {

            var exp = Expressionable.Create<DepartmentEntity>()
                .And(t => t.F_EnabledMark == 1)
                .And(t => t.F_DeleteMark == 0)
                .AndIF(!string.IsNullOrEmpty(keyWord),t=>t.F_FullName.Contains(keyWord)
                || t.F_EnCode.Contains(keyWord)
                || t.F_ShortName.Contains(keyWord)
                )
                .ToExpression();
            return this.BaseRepository().FindList(exp, "F_CompanyId,F_ParentId,F_SortCode,F_FullName");
        }
        /// <summary>
        /// 获取部门列表信息(根据部门ID集合)
        /// </summary>
        /// <param name="keyValues">部门ID集合</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetListByKeys(string[] keyValues)
        {
            return this.BaseRepository().FindList<DepartmentEntity>(t=> keyValues.Contains(t.F_DepartmentId));
        }
        /// <summary>
        /// 获取部门列表信息(根据公司Id)
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetAllList()
        {
            return this.BaseRepository().FindList<DepartmentEntity>(t=>t.F_EnabledMark == 1 && t.F_DeleteMark == 0);
        }
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<DepartmentEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<DepartmentEntity>(keyValue);
        }
        
        /// <summary>
        /// 获取部门数据实体
        /// </summary>
        /// <param name="deepNum">主键</param>
        /// <returns></returns>
        public Task<IEnumerable<DepartmentEntity>> GetSubs(string deepNum)
        {
            return this.BaseRepository().FindList<DepartmentEntity>(t=>t.F_EnabledMark == 1 && t.F_DeleteMark == 0 && t.F_DeepNum.Contains(deepNum));
        }
        

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除部门
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<DepartmentEntity>(keyValue);
                await db.Delete<UserRelationEntity>(t => t.F_ObjectId == keyValue);
                db.Commit();
            }
            catch (Exception)
            {
                db.Rollback();
                throw;
            }
          
        }
        /// <summary>
        /// 保存部门表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="departmentEntity">部门实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, DepartmentEntity departmentEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                departmentEntity.F_DepartmentId = keyValue;
                departmentEntity.F_ModifyDate = DateTime.Now;
                departmentEntity.F_ModifyUserId = this.GetUserId();
                departmentEntity.F_ModifyUserName = this.GetUserName();
                await this.BaseRepository().Update(departmentEntity);
            }
            else
            {
                departmentEntity.F_DepartmentId = Guid.NewGuid().ToString();
                departmentEntity.F_CreateDate = DateTime.Now;
                departmentEntity.F_DeleteMark = 0;
                departmentEntity.F_EnabledMark = 1;
                departmentEntity.F_CreateUserId = this.GetUserId();
                departmentEntity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(departmentEntity);
            }
        }

        /// <summary>
        /// 保存部门信息-批量（新增、修改）----ADD BY SSY 20220929
        /// </summary>
        /// <param name="departmentEntitys">部门实体</param>
        /// <returns></returns>
        public async Task SaveEntitys(List<DepartmentEntity> departmentEntitys)
        {
            await this.BaseRepository().Inserts(departmentEntitys);
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
            var list =(List<DepartmentEntity>)await this.BaseRepository().FindList<DepartmentEntity>(it=>it.F_WechatDepId== WechatDeptId);
            if (list.Count>0)
            {
                return list[0];
            }
            return new DepartmentEntity { };
        }
        #endregion
    }
}
