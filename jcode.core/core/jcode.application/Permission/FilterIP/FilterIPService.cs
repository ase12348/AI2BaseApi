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
    /// 日 期：2019.10.25
    /// 描 述：IP过滤
    /// </summary>
    public class FilterIPService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 过滤IP列表
        /// </summary>
        /// <param name="objectId">对象Id</param>
        /// <param name="visitType">访问:0-拒绝，1-允许</param>
        /// <returns></returns>
        public Task<IEnumerable<FilterIPEntity>> GetList(string objectId, int? visitType)
        {
            var exp = Expressionable.Create<FilterIPEntity>()
               .And(t => t.F_ObjectId == objectId)
               .AndIF(visitType != null, t => t.F_VisitType == visitType)
               .ToExpression();
            return this.BaseRepository().FindList(exp, "F_CreateDate");
        }
        /// <summary>
        /// 过滤IP实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<FilterIPEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<FilterIPEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除过滤IP
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteEntiy(string keyValue)
        {
            await this.BaseRepository().Delete<FilterIPEntity>(keyValue);
        }
        /// <summary>
        /// 保存过滤IP表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="filterIPEntity">过滤IP实体</param>
        /// <returns></returns>
        public async Task SaveForm(string keyValue, FilterIPEntity filterIPEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                filterIPEntity.F_ModifyUserId = this.GetUserId();
                filterIPEntity.F_ModifyUserName = this.GetUserName();

                filterIPEntity.F_Id = keyValue;
                filterIPEntity.F_ModifyDate = DateTime.Now;
                await this.BaseRepository().Update(filterIPEntity);
            }
            else
            {
                filterIPEntity.F_CreateUserId = this.GetUserId();
                filterIPEntity.F_CreateUserName = this.GetUserName();

                filterIPEntity.F_Id = Guid.NewGuid().ToString();
                filterIPEntity.F_CreateDate = DateTime.Now;
                filterIPEntity.F_DeleteMark = 0;
                filterIPEntity.F_EnabledMark = 1;
                await this.BaseRepository().Insert(filterIPEntity);
            }
        }
        #endregion
    }
}
