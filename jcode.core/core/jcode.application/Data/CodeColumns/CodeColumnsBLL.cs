using jcode.autofac;
using jcode.util;
using jcode.iapplication;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：超级开发者
    /// 日 期： 2021-09-18 10:13:07
    /// 描 述： 数据库表字段信息
    /// </summary>
    class CodeColumnsBLL : BLLBase, CodeColumnsIBLL, BLL
    {
        private readonly CodeColumnsService codeColumnsService = new CodeColumnsService();

        #region 获取数据
        /// <summary>
        /// 获取CodeColumns的列表数据
        /// </summary>
        /// <param name="queryParams">查询参数</param>
        /// <returns></returns>
        public Task<IEnumerable<CodeColumnsEntity>> GetList(CodeColumnsEntity queryParams)
        {
            return codeColumnsService.GetList(queryParams);
        }

        #endregion

        #region 提交数据

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="tableId">表格主键</param>
        public async Task Delete(string tableId)
        {
            await codeColumnsService.Delete(tableId);
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public  Task Add(List<CodeColumnsEntity> list)
        {
            return this.codeColumnsService.Add(list);
        }
        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public Task Update(List<CodeColumnsEntity> list)
        {
            return this.codeColumnsService.Update(list);
        }


        /// <summary>
        /// 更新数据
        /// </summary>
        /// <param name="list">字段数据集</param>
        /// <returns></returns>
        public Task Deletes(List<CodeColumnsEntity> list) {
            return this.codeColumnsService.Deletes(list);
        }

        #endregion
    }
}
