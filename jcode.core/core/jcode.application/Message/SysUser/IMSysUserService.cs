using jcode.iapplication;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：即时通讯用户注册
    /// </summary>
    public class IMSysUserService : ServiceBase
    {
        #region 获取数据

        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<IMSysUserEntity>> GetList(string keyWord)
        {
            var expression = LinqExtensions.True<IMSysUserEntity>();
            if (!string.IsNullOrEmpty(keyWord)) {
                expression = expression.And(t => t.F_Name.Contains(keyWord));
            }
            return this.BaseRepository().FindList<IMSysUserEntity>(expression);
        }

        /// <summary>
        /// 获取列表分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyWord">查询关键字</param>
        /// <returns></returns>
        public Task<IEnumerable<IMSysUserEntity>> GetPageList(Pagination pagination, string keyWord)
        {
            var expression = LinqExtensions.True<IMSysUserEntity>();
            if (!string.IsNullOrEmpty(keyWord))
            {
                expression = expression.And(t => t.F_Name.Contains(keyWord));
            }

            return this.BaseRepository().FindList(expression, pagination);
        }

        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<IMSysUserEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<IMSysUserEntity>(keyValue);
        }
        /// <summary>
        /// 获取实体数据byCode
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Task<IMSysUserEntity> GetEntityByCode(string code)
        {
            return this.BaseRepository().FindEntity<IMSysUserEntity>(t=>t.F_Code == code);
        }

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task DeleteEntity(string keyValue)
        {
            await this.BaseRepository().Delete<IMSysUserEntity>(keyValue);
        }

        /// <summary>
        /// 保存实体数据（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体数据</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, IMSysUserEntity entity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                entity.F_Id = keyValue;
                await this.BaseRepository().Update(entity);
            }
            else
            {
                entity.F_Id = Guid.NewGuid().ToString();
                entity.F_CreateDate = DateTime.Now;
                entity.F_CreateUserId = this.GetUserId();
                entity.F_CreateUserName = this.GetUserName();
                await this.BaseRepository().Insert(entity);
            }
        }
        #endregion
    }
}
