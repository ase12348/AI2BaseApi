using jcode.autofac;
using jcode.util;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace jcode.application {
    /// <summary>
    /// 极简搭AI应用开发基座项目-打印
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：超级管理员
    /// 日 期： 2023-05-06 14:06:35
    /// 描 述： word打印模版设置
    /// </summary>
    public class LrPrintWordBLL: BLLBase, ILrPrintWordBLL, BLL {
        private readonly LrPrintWordService lrPrintWordService = new LrPrintWordService();
        #region 获取数据
        /// <summary>
        /// 获取word打印模版设置的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintWordEntity>>GetList(LrPrintWordEntity queryParams) {
            return lrPrintWordService.GetList(queryParams);
        }
        /// <summary>
        /// 获取word打印模版设置的分页数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<LrPrintWordEntity>>GetPageList(Pagination pagination, LrPrintWordEntity queryParams, string tags = null) {
            return lrPrintWordService.GetPageList(pagination, queryParams, tags);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public Task<LrPrintWordEntity>GetEntity(string keyValue) {
            return lrPrintWordService.GetEntity(keyValue);
        }
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public Task<LrPrintWordEntity>GetEntityByCode(string code) {
            return lrPrintWordService.GetEntityByCode(code);
        }
        #endregion
        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue) {
            await lrPrintWordService.Delete(keyValue);
        }
        /// <summary>
        /// 批量删除实体
        /// </summary>
        /// <param name="keyValues">主键集</param>
        /// <returns></returns>
        public async Task Deletes(string keyValues) {
            await lrPrintWordService.Deletes(keyValues);
        }
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, LrPrintWordEntity entity) {
            await lrPrintWordService.SaveEntity(keyValue, entity);
        }
        #endregion
        
    }
}