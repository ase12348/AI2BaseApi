using jcode.autofac;
using jcode.util;
using jcode.iapplication;
using System;
using jcode.cache;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座-核心模块
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2024-03-01 10:15:24
    /// 描 述： 图标管理
    /// </summary>
    public class LrBaseIconsBLL: BLLBase, ILrBaseIconsBLL, BLL {
        private readonly LrBaseIconsService lrBaseIconsService = new LrBaseIconsService();
        private readonly ICache _iCache;
        private readonly string key = "jcode_icons_list";
        private readonly string keyVer = "jcode_icons_ver";
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public LrBaseIconsBLL(ICache iCache)
        {
            _iCache = iCache;
        }
        
        #region 获取数据
        /// <summary>
        /// 获取图标管理的列表数据
        /// </summary>
        /// <returns></returns>
        public async Task<IconDto>GetALLList(string ver)
        {
            var res = new IconDto();
            res.Ver = await _iCache.ReadAsync<string>(keyVer);
            if (string.IsNullOrEmpty(res.Ver))
            {
                res.Ver = await lrBaseIconsService.GetVer();
                if (!string.IsNullOrEmpty(res.Ver))
                {
                    await _iCache.WriteAsync(keyVer,res.Ver);
                }
            }
            
            if (string.IsNullOrEmpty(res.Ver) || res.Ver == ver)
            {
                return res;
            }
            
            res.List =await _iCache.ReadAsync<IEnumerable<LrBaseIconsEntity>>(key);
            if (res.List == null)
            {
                res.List = await lrBaseIconsService.GetALLList();
                await _iCache.WriteAsync(key,res.List);
            }
            
            return res;
        }
        /// <summary>
        /// 获取图标管理的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrBaseIconsEntity>>GetPageList(Pagination pagination, LrBaseIconsEntity queryParams) {
            return lrBaseIconsService.GetPageList(pagination, queryParams);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrBaseIconsEntity>GetEntity(string keyValue) {
            return lrBaseIconsService.GetEntity(keyValue);
        }
        #endregion
        
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await lrBaseIconsService.Delete(keyValue);
            await _iCache.RemoveAsync(key);
            await lrBaseIconsService.SetNewVer();
            await _iCache.RemoveAsync(keyVer);
            
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues) {
            await lrBaseIconsService.Deletes(keyValues);
            await _iCache.RemoveAsync(key);
            await lrBaseIconsService.SetNewVer();
            await _iCache.RemoveAsync(keyVer);
           
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrBaseIconsEntity entity) {
            await lrBaseIconsService.SaveEntity(keyValue, entity);
            await _iCache.RemoveAsync(key);
            await lrBaseIconsService.SetNewVer();
            await _iCache.RemoveAsync(keyVer);
        }
        #endregion
    }
}