using jcode.autofac;
using jcode.util;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.19
    /// 描 述：Excel数据导入设置
    /// </summary>
    public interface ExcelImportIBLL:IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取列表数据
        /// </summary>
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">名称</param>
        /// <param name="moduleId">功能id</param>
        /// <returns></returns>
        Task<IEnumerable<ExcelImportEntity>> GetPageList(Pagination pagination, string keyword, string moduleId);
        /// <summary>
        /// 获取导入配置列表根据模块ID
        /// </summary>
        /// <param name="moduleId">功能模块主键</param>
        /// <returns></returns>
        Task<IEnumerable<ExcelImportEntity>> GetList(string moduleId);
        /// <summary>
        /// 获取所有导入配置信息
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<ExcelImportEntity>> GetList();
        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<ExcelImportEntity> GetEntity(string keyValue);

        /// <summary>
        /// 获取配置信息实体
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        Task<ExcelImportEntity> GetEntityByCode(string code);
        /// <summary>
        /// 获取配置字段列表
        /// </summary>
        /// <param name="importId">配置信息主键</param>
        /// <returns></returns>
        Task<IEnumerable<ExcelImportFieldEntity>> GetFieldList(string importId);

        #endregion

        #region 提交数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        Task DeleteEntity(string keyValue);
        /// <summary>
        /// 保存表单（新增、修改）
        /// </summary>
        /// <param name="keyValue">主键值</param>
        /// <param name="entity">实体数据</param>
        /// <param name="filedList">字段列表</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, ExcelImportEntity entity, List<ExcelImportFieldEntity> filedList);
        /// <summary>
        /// 更新配置主表
        /// </summary>
        /// <param name="keyValue"></param>
        /// <param name="entity"></param>
        Task UpdateEntity(string keyValue, ExcelImportEntity entity);
        #endregion

        #region 扩展方法

        /// <summary>
        /// excel 数据导入（未导入数据写入缓存）
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="list"></param>
        /// <param name="dt"></param>
        /// <returns></returns>
        Task<(DataTable elist, int snum, int fnum)> ImportTable(ExcelImportEntity entity,
            List<ExcelImportFieldEntity> list, DataTable dt);
        /// <summary>
        /// excel 数据导入（未导入数据写入缓存）
        /// </summary>
        /// <param name="code">导入模板编码</param>
        /// <param name="dt">导入数据</param>
        /// <returns></returns>
        Task<(DataTable elist, int snum, int fnum)> ImportTable(string code,DataTable dt);
        #endregion
    }
}
