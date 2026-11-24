using jcode.autofac;
using jcode.iapplication;
using jcode.util;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.RateLimiting;
using jcode.orm;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;
using System.Threading.Tasks;
using jcode.cache;
using NLog;

namespace jcode.web.util.Filters
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.17
    /// 描 述：请求拦截器
    /// </summary>
    public class RequestFilter : ActionFilterAttribute
    {
        /// <summary>
        /// 计时器
        /// </summary>
        private Stopwatch stopwatch { get; set; }
        
        /// <summary>
        /// 并发许可
        /// </summary>
        private RateLimitLease lease { get; set; }

        
        public override async Task OnActionExecutionAsync(
            ActionExecutingContext context,
            ActionExecutionDelegate next)
        {
            // 首次安装时宿主可能还没有可连的主库，人正是在这个状态下测待填的库地址：
            // 查 api_maintain 与写请求日志都要先连主库，留着就会把「测连接」自己变成 500。
            if (context.ActionDescriptor.EndpointMetadata.OfType<SkipApiMaintainAttribute>().Any())
            {
                await next();
                return;
            }

            await Executing(context);
            var res = await next();
            await Executed(res);
        }


        /// <summary>
        /// 请求前
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private async Task Executing(ActionExecutingContext context)
        {
            //计时器（开始）
            stopwatch = CommonHelper.TimerStart();
            await Task.CompletedTask;
        }
        /// <summary>
        /// 请求后
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async Task Executed(ActionExecutedContext context)
        {
            if (lease != null)
            {
                lease.Dispose();
            }

            if (context.Exception == null || context.Exception.Message.IndexOf("#NotLog#") != -1 ||
                context.Exception.Message.IndexOf("#字段重复#") != -1)
            {
                //请求写入日志
                RequestLog(context);
            }

            stopwatch.Stop();
            await Task.CompletedTask;
        }

        #region 写入日志
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="context"></param>
        public void RequestLog(ActionExecutedContext context)
        {
            if (context.HttpContext.Request.Path.Value == "/login")
            {
                return;
            }

            // 记录操作日志
            string diffLogsStr = ContextHelper.GetItem("dbDiffLogs")?.ToString();
            if (!string.IsNullOrEmpty(diffLogsStr)
                && context.HttpContext.Request.Path.Value.IndexOf("/workflow/process") == -1 // 排除流程接口
               ) {
                var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 3;
                logEntity.F_ExecuteResult = 1;
                // logEntity.F_ExecuteResultJson = diffLogsStr;
                var diffLogs = diffLogsStr.ToObject<List<DbDiffLog>>();

                string diffStr = "";
                foreach (var diffLog in diffLogs)
                {
                    if (diffLog.DiffType == DiffType.insert && diffLog.AfterData != null)
                    {
                        foreach (var tableInfo in diffLog.AfterData)
                        {
                            diffStr += $"新增{tableInfo.TableName}:";
                            foreach (var column in tableInfo.Columns)
                            {
                                
                                var oldValue  = "";
                                if (column.Value != null)
                                {
                                    oldValue = column.Value.ToString();
                                }
                                diffStr += $"【{column.ColumnName}】{oldValue};";
                            }
                            diffStr += "\r\n";
                        }
                    }
                    else if (diffLog.DiffType == DiffType.update)
                    {
                        int index = 0;
                        foreach (var tableInfo in diffLog.AfterData)
                        {
                            diffStr += $"更新{tableInfo.TableName}:";
                            int columnIndex = 0;
                            foreach (var column in tableInfo.Columns)
                            {
                                var beforeColumn = diffLog.BeforeData[index].Columns[columnIndex];
                                var newValue  = "";
                                if (column.Value != null)
                                {
                                    newValue = column.Value.ToString();
                                }
                                
                                var oldValue = "";
                                if (beforeColumn.Value != null)
                                {
                                    oldValue = beforeColumn.Value.ToString();
                                }


                                if ( oldValue != newValue)
                                {
                                    diffStr += $"【{column.ColumnName}】{oldValue}->{newValue};";
                                }
                                else if (column.IsPrimaryKey)
                                {
                                    diffStr += $"【{column.ColumnName}主键】{newValue};";
                                }

                                columnIndex += 1;
                            }
                            index += 1;
                            diffStr += "\r\n";
                        }
                    
                       
                    }
                    else if (diffLog.DiffType == DiffType.delete && diffLog.BeforeData != null)
                    {
                        foreach (var tableInfo in diffLog.BeforeData)
                        {
                            diffStr += $"删除{tableInfo.TableName}:";
                            foreach (var column in tableInfo.Columns)
                            {
                                
                                var oldValue  = "";
                                if (column.Value != null)
                                {
                                    oldValue = column.Value.ToString();
                                }
                                diffStr += $"【{column.ColumnName}】{oldValue};";
                            }
                            diffStr += "\r\n";
                        }
                    }
                }
                logEntity.F_ExecuteResultJson = diffStr;
                
                logIBLL.Write(logEntity,stopwatch);
            }
        }
        #endregion
    }
}
