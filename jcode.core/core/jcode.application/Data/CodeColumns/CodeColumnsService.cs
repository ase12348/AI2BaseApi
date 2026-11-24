using jcode.iapplication;
using jcode.util;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 代码生成器
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：施赛一
    /// 日 期： 2021-09-18 10:13:07
    /// 描 述： 数据库表字段信息
    /// </summary>
    public class CodeColumnsService : ServiceBase
    {
        #region 获取数据
        /// <summary>
        /// 获取CodeColumns的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeColumnsEntity>> GetList(CodeColumnsEntity queryParams)
        {
            var expression = LinqExtensions.True<CodeColumnsEntity>();
            if (!string.IsNullOrEmpty(queryParams.F_CodeTableId))
            {
                expression = expression.And(t => t.F_CodeTableId == queryParams.F_CodeTableId);
            }
            if (!string.IsNullOrEmpty(queryParams.F_DbType))
            {
                expression = expression.And(t => t.F_DbType.Contains(queryParams.F_DbType));
            }
            if (!string.IsNullOrEmpty(queryParams.F_DbColumnName))
            {
                expression = expression.And(t => t.F_DbColumnName.Contains(queryParams.F_DbColumnName));
            }
            if (queryParams.F_IsIdentity != null)
            {
                expression = expression.And(t => t.F_IsIdentity == queryParams.F_IsIdentity);
            }
            if (queryParams.F_IsPrimaryKey != null)
            {
                expression = expression.And(t => t.F_IsPrimaryKey == queryParams.F_IsPrimaryKey);
            }
            return this.BaseRepository().FindList(expression, "F_Sort");
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableId">表格主键</param>
        public async Task Delete(string tableId)
        {
            await this.BaseRepository().Delete<CodeColumnsEntity>(t=>t.F_CodeTableId == tableId);
        }
        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public async Task Add(List<CodeColumnsEntity> list) {
            await this.BaseRepository().Inserts(list);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public async Task Update(List<CodeColumnsEntity> list)
        {
            await this.BaseRepository().Updates(list);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public async Task Deletes(List<CodeColumnsEntity> list)
        {
            await this.BaseRepository().Deletes(list);
        }
        #endregion
    }
}
