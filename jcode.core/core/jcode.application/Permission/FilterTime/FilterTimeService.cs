using jcode.iapplication;
using System;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.10.25
    /// 描 述：时段过滤
    /// </summary>
    public class FilterTimeService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 过滤时段实体
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <returns></returns>
        public Task<FilterTimeEntity> GetEntity(string keyValue)
        {
            return this.BaseRepository().FindEntityByKey<FilterTimeEntity>(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除过滤时段
        /// </summary>
        /// <param name="keyValue">主键</param>
        public async Task DeleteEntiy(string keyValue)
        {
            await this.BaseRepository().Delete<FilterTimeEntity>(keyValue);
        }
        /// <summary>
        /// 保存过滤时段表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="filterTimeEntity">过滤时段实体</param>
        /// <returns></returns>
        public async Task SaveForm(string keyValue, FilterTimeEntity filterTimeEntity)
        {
            if (!string.IsNullOrEmpty(keyValue))
            {
                filterTimeEntity.F_ModifyUserId = this.GetUserId();
                filterTimeEntity.F_ModifyUserName = this.GetUserName();
                filterTimeEntity.F_ModifyDate = DateTime.Now;
                await this.BaseRepository().Update(filterTimeEntity, false);
            }
            else
            {
                filterTimeEntity.F_CreateUserId = this.GetUserId();
                filterTimeEntity.F_CreateUserName = this.GetUserName();

                filterTimeEntity.F_CreateDate = DateTime.Now;
                filterTimeEntity.F_DeleteMark = 0;
                filterTimeEntity.F_EnabledMark = 1;

                await this.BaseRepository().Insert(filterTimeEntity);
            }
        }
        #endregion
    }
}
