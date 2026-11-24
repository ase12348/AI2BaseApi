using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.util;

namespace jcode.iquartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    public interface ITSSchemeIBLL : IBLL
    {

        #region 获取数据
        /// <summary> 
        /// 获取页面显示列表数据 
        /// </summary> 
        /// <param name="pagination">分页参数</param>
        /// <param name="keyword">查询参数</param> 
        /// <returns></returns> 
        Task<IEnumerable<TSSchemeInfoNoSchemeEntity>> GetPageList(Pagination pagination, string keyword, string appId = null);
        /// <summary> 
        /// 获取列表数据 
        /// </summary> 
        /// <returns></returns> 
        Task<IEnumerable<TSSchemeInfoEntity>> GetList();
        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="keyValue">主键</param> 
        /// <returns></returns> 
        Task<TSSchemeInfoEntity> GetEntity(string keyValue);


        /// <summary> 
        /// 获取表实体数据
        /// </summary> 
        /// <param name="runId">运行id</param> 
        /// <returns></returns> 
        Task<TSSchemeInfoEntity> GetEntityByRunId(string runId);
        #endregion

        #region 提交数据 
        /// <summary> 
        /// 删除实体数据
        /// </summary>
        /// <param name="keyValue">主键</param> 
        /// <returns></returns> 
        Task DeleteEntity(string keyValue);

        /// <summary> 
        /// 保存实体数据（新增、修改）
        /// </summary> 
        /// <param name="keyValue">主键</param>
        /// <param name="schemeInfoEntity">实体</param> 
        /// <returns></returns>
        Task SaveEntity(string keyValue, TSSchemeInfoEntity schemeInfoEntity);

        /// <summary>
        /// 获取运行id
        /// </summary>
        /// <param name="id">任务主键</param>
        /// <returns></returns>
        Task<string> GetRunId(string id);
        #endregion

        #region 扩展应用

        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="keyValue">任务主键</param>
        Task FinishJob(string keyValue);
        /// <summary>
        /// 暂停任务
        /// </summary>
        /// <param name="keyValue">任务主键</param>
        Task PauseJob(string keyValue);
        /// <summary>
        /// 启动任务
        /// </summary>
        /// <param name="keyValue">任务进程主键</param>
        Task EnAbleJob(string keyValue);

        /// <summary>
        /// 更新
        /// </summary>
        /// <param name="entity"></param>
        Task UpdateEntity(TSSchemeInfoEntity entity);
        #endregion

        #region 执行sql语句和存储过程
        /// <summary>
        /// 执行sql语句
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="sql">sql语句</param>
        /// <returns></returns>
        Task ExecuteBySql(string code, string sql);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="code">数据库编码</param>
        /// <param name="name">名称</param>
        /// <returns></returns>
        Task ExecuteProc(string code, string name);

        #endregion
    }
}
