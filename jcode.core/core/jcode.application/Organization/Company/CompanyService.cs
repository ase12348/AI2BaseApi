using jcode.iapplication;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.24
    /// 描 述：公司管理
    /// </summary>
    public class CompanyService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取公司列表信息
        /// </summary>
        /// <param name="keyWord">查询关键词</param>
        /// <returns></returns>
        public Task<IEnumerable<CompanyEntity>> GetList(string keyWord)
        {
            var exp = Expressionable.Create<CompanyEntity>()
                .And(t=>t.F_EnabledMark == 1)
                .And(t=>t.F_DeleteMark == 0)
                .AndIF(!string.IsNullOrEmpty(keyWord),t=>t.F_FullName.Contains(keyWord) 
                || t.F_EnCode.Contains(keyWord)
                || t.F_ShortName.Contains(keyWord))
                .ToExpression();
            return this.BaseRepository().FindList(exp, "F_SortCode,F_ParentId,F_FullName");
        }
        /// <summary>
        /// 获取子公司列表信息
        /// </summary>
        /// <param name="pId">父级Id</param>
        /// <returns></returns>
        public Task<IEnumerable<CompanyEntity>> GetListByPId(string pId)
        {
            var exp = Expressionable.Create<CompanyEntity>()
                 .And(t => t.F_EnabledMark == 1)
                 .And(t => t.F_DeleteMark == 0)
                 .And(t => t.F_ParentId == pId)
                 .ToExpression();
            return this.BaseRepository().FindList(exp, "F_SortCode,F_FullName");
        }
        /// <summary>
        /// 获取实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<CompanyEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<CompanyEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除公司
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task Delete(string keyValue)
        {
            var db = this.BaseRepository().BeginTrans();
            try
            {
                await db.Delete<CompanyEntity>(keyValue);
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
        /// 保存公司表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="companyEntity">公司实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, CompanyEntity companyEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                companyEntity.F_ModifyUserId = this.GetUserId();
                companyEntity.F_ModifyUserName = this.GetUserName();
                companyEntity.F_CompanyId = keyValue;
                companyEntity.F_ModifyDate = DateTime.Now;

                await this.BaseRepository().Update(companyEntity);
            }
            else
            {
                companyEntity.F_CreateUserId = this.GetUserId();
                companyEntity.F_CreateUserName = this.GetUserName();
                companyEntity.F_CompanyId = Guid.NewGuid().ToString();
                companyEntity.F_CreateDate = DateTime.Now;
                companyEntity.F_DeleteMark = 0;
                companyEntity.F_EnabledMark = 1;
                await this.BaseRepository().Insert(companyEntity);
            }

        }
        #endregion
    }
}
