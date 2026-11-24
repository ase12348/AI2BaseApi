using jcode.autofac;
using jcode.iquartz;
using jcode.util;
using Quartz;
using System;
using System.Threading;
using System.Threading.Tasks;
using jcode.iapplication;

namespace jcode.quartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务计划模板信息
    /// </summary>
    public class SchedulingHelper : IJob
    {
        private readonly ITSSchemeIBLL tSSchemeIBLL;
        /// <summary>
        /// 
        /// </summary>
        public SchedulingHelper()
        {
            tSSchemeIBLL = IocManager.Instance.GetService<ITSSchemeIBLL>();
        }

        /// <summary>
        /// 任务执行方法
        /// </summary>
        /// <param name="keyValue">任务进程主键</param>
        /// <returns></returns>
        private async Task<bool> Execute(string keyValue,bool isOne)
        {
            bool isOk = true;
            string msg = "";
            var stopwatch = CommonHelper.TimerStart();

            TSSchemeInfoEntity tSSchemeInfoEntity = null;

            // 获取一个任务进程
            try
            {
                tSSchemeInfoEntity = await tSSchemeIBLL.GetEntityByRunId(keyValue);
            }
            catch (Exception ex)
            {
                isOk = false;
                msg = "获取任务异常：" + ex.Message;
            }

            if (tSSchemeInfoEntity == null || tSSchemeInfoEntity.F_IsActive == 2 || tSSchemeInfoEntity.F_IsActive == 3)
            {
                if (tSSchemeInfoEntity == null) {
                    stopwatch.Stop();
                }
                return true;
            }

            string name = tSSchemeInfoEntity.F_Name;
            
            // 执行任务
            if (isOk)
            {
                try
                {
                    TSSchemeModel tSSchemeModel = tSSchemeInfoEntity.F_Scheme.ToObject<TSSchemeModel>();
                    switch (tSSchemeModel.MethodType)
                    {
                        case 1:// sql
                            await tSSchemeIBLL.ExecuteBySql(tSSchemeModel.DbId, tSSchemeModel.StrSql);
                            msg = "执行sql语句【" + tSSchemeModel.DbId + "】" + tSSchemeModel.StrSql;
                            break;
                        case 2:// 存储过程
                            msg = "执行存储过程【" + tSSchemeModel.DbId + "】" + tSSchemeModel.ProcName;
                            await tSSchemeIBLL.ExecuteProc(tSSchemeModel.DbId, tSSchemeModel.ProcName);
                            break;
                        case 3:// 接口
                            msg = "调用接口";
                            if (tSSchemeModel.UrlType == "1")
                            {
                                msg += "[GET]";
                                msg += tSSchemeModel.Url;
                                await HttpMethods.Get(tSSchemeModel.Url,TimeSpan.FromMinutes(20));
                            }
                            else
                            {
                                msg += "[POST]";
                                msg += tSSchemeModel.Url;
                                await HttpMethods.Post(tSSchemeModel.Url);
                            }
                          
                            break;
                        case 4:// 依赖注入
                            msg = "依赖注入" + tSSchemeModel.IocName;
                            if (!string.IsNullOrEmpty(tSSchemeModel.IocName) && IocManager.Instance.IsRegistered<ITsMethod>(tSSchemeModel.IocName))
                            {
                                ITsMethod iTsMethod = IocManager.Instance.GetService<ITsMethod>(tSSchemeModel.IocName);
                                ContextHelper.SetAsyncLocal();
                                msg += await iTsMethod.Execute();
                            }
                            break;
                    }
                }
                catch (Exception ex)
                {
                    isOk = false;
                    msg += "执行方法出错：" + ex.Message;
                }
            }


            stopwatch.Stop();
            try
            {
                // 任务执行记录统一进系统日志（lr_base_log 分类 10），控制台「系统日志」按类型就能看全。
                // 任务在调度线程里跑，没有 HttpContext，LogBLL.Write 不会用请求信息覆盖下面的字段
                LogEntity sysLogEntity = new LogEntity()
                {
                    F_CategoryId = 10,
                    // 账号写成 Job 而不是 System：非超级管理员在系统日志里看不到 F_OperateUserId = "System" 的行
                    F_OperateUserId = "Job",
                    F_OperateAccount = "Job",
                    // 姓名列放任务名，列表和关键词搜索都靠它
                    F_OperateName = name,
                    // 来源列放所属应用编码：lr_base_log 没有 F_AppCode，任务的应用归属只能挂在这里
                    F_Device = tSSchemeInfoEntity.F_AppCode,
                    F_ExecuteResult = isOk ? 1 : 2,
                    F_ExecuteResultJson = msg,
                    // 调度线程没有 HttpContext，这些请求态字段会留 null；SQLite 侧是 NOT NULL 列，
                    // 留 null 会直接插入失败（SQLite Error 19），这里显式给空值
                    F_OperateType = "JOB",
                    F_Url = string.Empty,
                    F_Host = string.Empty,
                    F_IPAddress = string.Empty,
                    F_Browser = string.Empty
                };
                IocManager.Instance.GetService<LogIBLL>().Write(sysLogEntity, stopwatch);
            }
            catch (Exception e)
            {
                var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = -1;
                logEntity.F_ExecuteResultJson = logIBLL.ExceptionFormat(e);
                logIBLL.Write(logEntity,null);
            }

            // 如果任务只执行一次，需要将任务设置成结束
            if (isOne && isOk)
            {
               await tSSchemeIBLL.FinishJob(tSSchemeInfoEntity.F_Id);
            }


            return isOk;
        }

        /// <summary>
        /// 任务执行方法
        /// </summary>
        /// <param name="context"></param>
        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            try
            {
                JobDataMap dataMap = context.JobDetail.JobDataMap;
                string keyValue = dataMap.GetString("keyValue");
                
                string isOne = string.Empty;
                if (dataMap.Keys.Contains("isOne"))
                {
                    isOne = dataMap.GetString("isOne");
                }

                TSSchemeInfoEntity tSSchemeInfoEntity = null;
                if (!await Execute(keyValue,isOne=="1"))
                {
                    // 如果异常，需要重新执行一次
                    tSSchemeInfoEntity = await tSSchemeIBLL.GetEntityByRunId(keyValue);
                    if (tSSchemeInfoEntity != null)
                    {
                        TSSchemeModel tSSchemeModel = tSSchemeInfoEntity.F_Scheme.ToObject<TSSchemeModel>();
                        var flag = false;
                        if (tSSchemeModel.IsRestart == 1)
                        {
                            for (int i = 0; i < tSSchemeModel.RestartNum; i++)
                            {
                                Thread.Sleep(60 * 1000 * tSSchemeModel.RestartMinute);  // 停顿1000毫秒
                                if (await Execute(keyValue,isOne=="1"))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                        }
                        
                        
                        if(!flag)
                        {
                            tSSchemeInfoEntity.F_IsActive = 3;
                            tSSchemeInfoEntity.F_RunActive = 2;
                            tSSchemeInfoEntity.F_RunTestTime =DateTime.Now;
                            await tSSchemeIBLL.UpdateEntity(tSSchemeInfoEntity);
                        }
                    }
                    else
                    {
                        tSSchemeInfoEntity.F_IsActive = 3;
                        tSSchemeInfoEntity.F_RunActive = 2;
                        tSSchemeInfoEntity.F_RunTestTime =DateTime.Now;
                        await tSSchemeIBLL.UpdateEntity(tSSchemeInfoEntity);
                    }
                }
            }
            catch (Exception e)
            {
                var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = -1;
                logEntity.F_ExecuteResultJson = logIBLL.ExceptionFormat(e);
                logIBLL.Write(logEntity,null);
            }

        }
    }
}
