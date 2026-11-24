using jcode.autofac;
using jcode.cache;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TencentCloud.Cme.V20191029.Models;

namespace jcode.application
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-10-10 09:59:11
    /// 描 述： 个性化主题配置
    /// </summary>
    public class LrConfigThemeBLL : BLLBase, ILrConfigThemeBLL, BLL
    {
        private readonly LrConfigThemeService lrConfigThemeService = new LrConfigThemeService();
        private readonly ICache _iCache;

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public LrConfigThemeBLL(ICache iCache)
        {
            _iCache = iCache;
        }

        #region 获取数据
        /// <summary>
        /// 获取个性化主题配置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigThemeEntity>> GetList(LrConfigThemeEntity queryParams)
        {
            return lrConfigThemeService.GetList(queryParams);
        }
        /// <summary>
        /// 获取个性化主题配置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrConfigThemeEntity>> GetPageList(Pagination pagination, LrConfigThemeEntity queryParams)
        {
            return lrConfigThemeService.GetPageList(pagination, queryParams);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrConfigThemeEntity> GetEntity(string keyValue)
        {
            return lrConfigThemeService.GetEntity(keyValue);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await lrConfigThemeService.Delete(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues)
        {
            await lrConfigThemeService.Deletes(keyValues);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrConfigThemeEntity entity)
        {
            await lrConfigThemeService.SaveEntity(keyValue, entity);
        }
        #endregion

        #region 扩展方法
        /// <summary>
        /// 获取当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        public async Task<LrConfigThemeEntity> GetTheme()
        {
            var user = await CurrentUser();
            string key = $"config_theme_{user.F_UserId}";
            var entity = _iCache.Read<LrConfigThemeEntity>(key);
            if (entity == null)
            {
                entity = await lrConfigThemeService.GetTheme(user);
                _iCache.Write(key, entity);
            }
            return entity;
        }
        /// <summary>
        /// 保存当前登录人员的个性化主题（新增、编辑）
        /// </summary>
        /// <param name="entity">数据实体</param>
        /// <returns></returns>
        public async Task SaveTheme(LrConfigThemeEntity entity)
        {
            var theme=await GetTheme();
            if (theme!=null)//编辑
            {
                await lrConfigThemeService.SaveEntity(theme.F_Id, entity);
                _iCache.Remove($"config_theme_{theme.F_CreateUserId}");
            }
            else//新增
            {
                await lrConfigThemeService.SaveEntity(null, entity);
            }
        }
        /// <summary>
        /// 清除当前登录人员的个性化主题
        /// </summary>
        /// <returns></returns>
        public async Task DeleteTheme()
        {
            var theme = await GetTheme();
            if (theme!=null)
            {
                await lrConfigThemeService.Delete(theme.F_Id);
                _iCache.Remove($"config_theme_{theme.F_CreateUserId}");
            }
        }
        #endregion
    }
}