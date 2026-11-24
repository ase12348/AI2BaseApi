using jcode.autofac;
using jcode.iquartz;
using jcode.util;
using Quartz;
using Quartz.Impl.Triggers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.iapplication;
using jcode.timer;


namespace jcode.quartz
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.01
    /// 描 述：任务调度操作类
    /// </summary>
    public class QuartzHelper
    {

        #region 新增任务
        /// <summary>
        /// 添加只执行一次的任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="starTime">任务开始时间</param>
        /// <param name="endTime">任务结束时间</param>
        /// <param name="keyValue">任务主键</param>
        public static async Task AddRepeatOneJob(string jobName, string starTime, string endTime, string keyValue)
        {
            IJobDetail job = JobBuilder.Create<SchedulingHelper>()//新建任务执行类
                .WithIdentity(jobName, "lrts") // name "myJob", group "group1"
                .UsingJobData("keyValue", keyValue)//传递参数
                .UsingJobData("isOne", "1")//传递参数
                .Build();
            //开始时间处理
            DateTime now = DateTime.Now;
            DateTime sStarTime = now.AddMicroseconds(10);
            if (!string.IsNullOrEmpty(starTime))
            {
                sStarTime = Convert.ToDateTime(starTime);
            }
            //结束时间处理
            DateTime sEndTime = DateTime.MaxValue.AddDays(-1);
            if (!string.IsNullOrEmpty(endTime))
            {
                sEndTime = Convert.ToDateTime(endTime);
            }
            if (sStarTime <= sEndTime) {
                DateTimeOffset starRunTime = new DateTimeOffset(sStarTime);
                DateTimeOffset endRunTime = new DateTimeOffset(sEndTime);
                
                // 创建一个时间触发器
                SimpleTriggerImpl trigger = new SimpleTriggerImpl
                {
                    Key = new TriggerKey(jobName, "lrts"),
                    StartTimeUtc = starRunTime,
                    EndTimeUtc = endRunTime,
                    RepeatCount = 0
                };
                IScheduler sched = TimerHelper.GetScheduler();
                await sched.ScheduleJob(job, trigger);
            }

           
        }
        /// <summary>
        /// 添加简单重复执行任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="keyValue">任务主键</param>
        /// <param name="value">间隔值</param>
        /// <param name="type">间隔类型</param>
        public static async Task AddRepeatJob(string jobName, string starTime, string endTime, string keyValue, int value, string type)
        {

            IJobDetail job = JobBuilder.Create<SchedulingHelper>()//新建任务执行类
                .DisallowConcurrentExecution()//禁止并发地执行同一个JobDetail实例
                .WithIdentity(jobName, "lrts") // name "myJob", group "group1"
                .UsingJobData("keyValue", keyValue)//传递参数
                .Build();
            //开始时间处理
            DateTime StarTime = DateTime.Now;
            if (!string.IsNullOrEmpty(starTime))
            {
                StarTime = Convert.ToDateTime(starTime);
            }
            DateTimeOffset starRunTime = new DateTimeOffset(StarTime);
            //结束时间处理
            DateTime EndTime = DateTime.MaxValue.AddDays(-1);
            if (!string.IsNullOrEmpty(endTime))
            {
                EndTime = Convert.ToDateTime(endTime);
            }
            DateTimeOffset endRunTime = new DateTimeOffset(EndTime);
            ITrigger trigger = null;

            switch (type)
            {
                case "minute": // 分钟
                    trigger = TriggerBuilder.Create().WithIdentity(jobName, "lrts")
                        .StartAt(starRunTime)
                        .EndAt(endRunTime)
                        .WithSimpleSchedule(t =>
                        t.WithInterval(TimeSpan.FromMinutes(value))
                        .WithMisfireInstruction(SimpleTriggerMisfireInstruction.NextWithExistingCount)
                        .RepeatForever())
                        .Build();
                    break;
                case "hours": // 小时
                    trigger = TriggerBuilder.Create()
                        .WithIdentity(jobName, "lrts")
                        .StartAt(starRunTime)
                        .EndAt(endRunTime)
                        .WithSimpleSchedule(t =>
                        t.WithInterval(TimeSpan.FromHours(value))
                        .WithMisfireInstruction(SimpleTriggerMisfireInstruction.NextWithExistingCount)
                        .RepeatForever())
                        .Build();
                    break;
                case "day": // 天
                    trigger = TriggerBuilder.Create()
                         .WithIdentity(jobName, "lrts")
                         .StartAt(starRunTime)
                         .EndAt(endRunTime)
                         .WithSchedule(
                                    CalendarIntervalScheduleBuilder.Create()
                                    .WithInterval(value, IntervalUnit.Day)
                                    .WithMisfireInstruction(CalendarIntervalTriggerMisfireInstruction.DoNothing)
                                     )
                         .Build();
                    break;
                case "week":// 周
                    trigger = TriggerBuilder.Create()
                     .WithIdentity(jobName, "lrts")
                     .StartAt(starRunTime)
                     .EndAt(endRunTime)
                     .WithSchedule(
                                CalendarIntervalScheduleBuilder.Create()
                                 .WithInterval(value, IntervalUnit.Week)
                                 .WithMisfireInstruction(CalendarIntervalTriggerMisfireInstruction.DoNothing)
                                 )
                     .Build();
                    break;
            }
            //实例化
            IScheduler sched = TimerHelper.GetScheduler();
            if (trigger != null)
            {
                await sched.ScheduleJob(job, trigger);
            }

            
        }
        /// <summary>
        /// 添加Cron表达式任务（一个表达式）
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="keyValue">主键名称</param>
        /// <param name="corn">Cron表达式</param>
        public static async Task AddCronJob(string jobName, string starTime, string endTime, string keyValue, string corn)
        {
            IJobDetail job = JobBuilder.Create<SchedulingHelper>()//新建任务执行类
                .DisallowConcurrentExecution()//禁止并发地执行同一个JobDetail实例
                .WithIdentity(jobName, "lrts") // name "myJob", group "group1"
                .UsingJobData("keyValue", keyValue)//传递参数
                .Build();
            //开始时间处理
            DateTime StarTime = DateTime.Now;
            if (!string.IsNullOrEmpty(starTime))
            {
                StarTime = Convert.ToDateTime(starTime);
            }
            DateTimeOffset starRunTime = new DateTimeOffset(StarTime);
            //结束时间处理
            DateTime EndTime = DateTime.MaxValue.AddDays(-1);
            if (!string.IsNullOrEmpty(endTime))
            {
                EndTime = Convert.ToDateTime(endTime);
            }
            DateTimeOffset endRunTime = new DateTimeOffset(EndTime);

            // 
            CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.Create(corn)
                .WithMisfireInstruction(CronTriggerMisfireInstruction.DoNothing);

            // 创建一个时间触发器
            ICronTrigger trigger = (ICronTrigger)TriggerBuilder.Create()
                                             .StartAt(starRunTime)
                                             .EndAt(endRunTime)
                                             .WithIdentity(jobName, "lrts")
                                             .WithSchedule(scheduleBuilder)
                                             .Build();
           
            IScheduler sched = TimerHelper.GetScheduler();
            await sched.ScheduleJob(job, trigger);
        }
        /// <summary>
        /// 添加多触发器任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        /// <param name="starTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="keyValue">任务主键</param>
        /// <param name="list">cron数组</param>
        public static async Task AddListCronJob(string jobName, string starTime, string endTime, string keyValue, List<string> list)
        {
            IJobDetail job = JobBuilder.Create<SchedulingHelper>()//新建任务执行类
                .DisallowConcurrentExecution()//禁止并发地执行同一个JobDetail实例
                .WithIdentity(jobName, "lrts") // name "myJob", group "group1"
                .UsingJobData("keyValue", keyValue)//传递参数
                .StoreDurably()
                .Build();
            //开始时间处理
            DateTime StarTime = DateTime.Now;
            if (!string.IsNullOrEmpty(starTime))
            {
                StarTime = Convert.ToDateTime(starTime);
            }
            DateTimeOffset starRunTime = new DateTimeOffset(StarTime);
            //结束时间处理
            DateTime EndTime = DateTime.MaxValue.AddDays(-1);
            if (!string.IsNullOrEmpty(endTime))
            {
                EndTime = Convert.ToDateTime(endTime);
            }
            DateTimeOffset endRunTime = new DateTimeOffset(EndTime);
            IScheduler sched = TimerHelper.GetScheduler();
            await sched.AddJob(job, new AddJobOptions { Replace = true });
            // 创建一个时间触发器
            for (var i = 0; i < list.Count; i++)
            {
                // 
                CronScheduleBuilder scheduleBuilder = CronScheduleBuilder.Create(list[i])
                    .WithMisfireInstruction(CronTriggerMisfireInstruction.DoNothing);

                ITrigger trigger = TriggerBuilder.Create()
                                   .WithIdentity("trigger" + Guid.NewGuid().ToString())
                                   .StartAt(starRunTime)
                                   .EndAt(endRunTime)
                                   .ForJob(job)
                                   .WithSchedule(scheduleBuilder)
                                   .Build();
                await sched.ScheduleJob(trigger);
            }
        }
        #endregion

        #region 删除任务
        /// <summary>
        /// 删除任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        public static async Task DeleteJob(string jobName)
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                IScheduler sched = TimerHelper.GetScheduler();
                TriggerKey triggerKey = new TriggerKey(jobName, "lrts");
                await sched.PauseTrigger(triggerKey);// 停止触发器  
                await sched.UnscheduleJob(triggerKey);// 移除触发器
                await sched.PauseJob(new JobKey(jobName, "lrts"));// 暂停任务
                await sched.DeleteJob(new JobKey(jobName, "lrts"));// 删除任务  
            }
        }
        #endregion

        #region 任务暂停和启动
        /// <summary>
        /// 暂停一个job任务
        /// </summary>
        /// <param name="jobName">任务名称</param>
        public static async Task PauseJob(string jobName)
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                IScheduler sched = TimerHelper.GetScheduler();
                await sched.PauseJob(new JobKey(jobName, "lrts"));
            }
        }
        /// <summary>
        /// 重启启动一个job
        /// </summary>
        /// <param name="jobName">任务名称</param>
        public static async Task ResumeJob(string jobName)
        {
            if (!string.IsNullOrEmpty(jobName))
            {
                IScheduler sched = TimerHelper.GetScheduler();
                await sched.ResumeJob(new JobKey(jobName, "lrts"));
            }
        }
        #endregion

        #region 扩展应用
        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="schemeInfoId">模板信息主键</param>
        /// <param name="runId">任务运行id</param>
        /// <param name="scheme">任务模板</param>
        public static async Task AddJob(string schemeInfoId,string runId, TSSchemeModel scheme)
        {
            string startTime = "";
            string endTime = "";
            if (scheme.StartType == 2 && scheme.StartTime != null)
            {
                startTime = ((DateTime)scheme.StartTime).ToString("yyyy-MM-dd HH:mm:ss");
            }


            if (scheme.EndType == 2 && scheme.EndTime != null)
            {
                endTime = ((DateTime)scheme.EndTime).ToString("yyyy-MM-dd HH:mm:ss");
            }


            switch (scheme.ExecuteType)
            {
                case 1:// 只执行一次
                    await AddRepeatOneJob(schemeInfoId, startTime, endTime, runId);//加入只执行一次的任务
                    break;
                case 2:// 简单重复执行
                    await AddRepeatJob(schemeInfoId, startTime, endTime, runId, scheme.SimpleValue, scheme.SimpleType);
                    break;
                case 3:// 明细频率执行
                    List<string> cornlist = new List<string>();
                    foreach (var fre in scheme.FrequencyList)
                    {
                        string cron = "0 ";
                        cron += fre.Minute + " " + fre.Hour + " ";
                        switch (fre.Type)
                        {
                            case "day":
                                cron += "* ";
                                break;
                            case "week":
                                cron += "? ";
                                break;
                            case "month":
                                cron += fre.Day + " ";
                                break;

                        }
                        cron += fre.Mounth + " ";
                        if (fre.Type == "week")
                        {
                            cron += fre.Week + " ";
                        }
                        else
                        {
                            cron += "? ";
                        }
                        cron += "*";
                        cornlist.Add(cron);
                    }
                    await AddListCronJob(schemeInfoId, startTime, endTime, runId, cornlist);
                    break;
                case 4:// corn表达式
                    await AddCronJob(schemeInfoId, startTime, endTime, runId, scheme.CornValue);
                    break;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task InitJob()
        {
            try
            {
                ITSSchemeIBLL tSSchemeIBLL = IocManager.Instance.GetService<ITSSchemeIBLL>();
                var list = await tSSchemeIBLL.GetList();
                IScheduler sched = TimerHelper.GetScheduler();
                
                Dictionary<JobKey, string> JobKeyMap = new Dictionary<JobKey, string>();
                foreach (var item in list)
                {
                    try
                    {
                    
                        var jobKey = new JobKey(item.F_Id, "lrts");
                        JobKeyMap.Add(jobKey,item.F_Id);
                        if (await sched.Exists(jobKey))
                        {
                            var job = await sched.GetJobDetail(jobKey);
                            var keyValue = job.JobDataMap.GetString("keyValue");
                            if (keyValue != item.F_RunId)
                            {
                                await DeleteJob(item.F_Id);
                                await AddJob(item.F_Id, item.F_RunId, item.F_Scheme.ToObject<TSSchemeModel>());
                            }
                        }
                        else
                        {
                            var runId =await tSSchemeIBLL.GetRunId(item.F_Id);
                            await AddJob(item.F_Id, runId, item.F_Scheme.ToObject<TSSchemeModel>());
                        }
                        await tSSchemeIBLL.UpdateEntity(new TSSchemeInfoEntity(){F_Id = item.F_Id,F_RunActive= 1,F_RunTestTime = DateTime.Now});
                    }
                    catch (Exception e)
                    {
                        var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                        // 执行记录统一进系统日志（lr_base_log 分类 10）
                        LogEntity sysLogEntity = new LogEntity()
                        {
                            F_CategoryId = 10,
                            F_OperateUserId = "Job",
                            F_OperateAccount = "Job",
                            F_OperateName = item.F_Name,
                            F_Device = item.F_AppCode,
                            F_ExecuteResult = 2,
                            F_ExecuteResultJson = logIBLL.ExceptionFormat(e),
                            // 后台调度线程无请求上下文，请求态字段必须给空值（SQLite 侧是 NOT NULL 列）
                            F_OperateType = "JOB",
                            F_Url = string.Empty,
                            F_Host = string.Empty,
                            F_IPAddress = string.Empty,
                            F_Browser = string.Empty
                        };
                        logIBLL.Write(sysLogEntity, null);
                        await tSSchemeIBLL.UpdateEntity(new TSSchemeInfoEntity(){F_Id = item.F_Id,F_IsActive = 3,F_RunActive= 2,F_RunTestTime = DateTime.Now});
                    }
                }

                var jobKeys = await sched.GetJobKeys(GroupMatcher<JobKey>.GroupContains("lrts"));
                foreach (var jobKey in jobKeys)
                {
                    if (!JobKeyMap.ContainsKey(jobKey))
                    {
                        await DeleteJob(jobKey.Name);
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
        
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static async Task RemoveJob()
        {
            try
            {
                IScheduler sched = TimerHelper.GetScheduler();
                var jobKeys = await sched.GetJobKeys(GroupMatcher<JobKey>.GroupContains("lrts"));
                foreach (var jobKey in jobKeys)
                {
                    await DeleteJob(jobKey.Name);
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
        #endregion
    }
}
