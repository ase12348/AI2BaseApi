using System;
using System.Diagnostics;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.cache;
using Quartz;
using System.Threading;
using jcode.application;
using jcode.quartz;
using jcode.timer;
using jcode.web.util;
using Quartz.Impl.Triggers;


namespace jcode.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class QuartzJobScheduler
    {


        /// <summary>
        /// 
        /// </summary>
        public static async Task Init()
        {
            await Start(IdHelper.Id);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public static async Task Start(string id)
        {
            try
            {
                string jobName = "jcode_timer_job";
                IScheduler scheduler = TimerHelper.GetScheduler();
                TriggerKey triggerKey = new TriggerKey(jobName, "jcode_timer_group");
                if (await scheduler.GetTrigger(triggerKey) != null)
                {
                    jobName += '1';
                }

                IJobDetail job = JobBuilder.Create<TimerJob>().UsingJobData("id", id).Build();
                //开始时间处理
                DateTimeOffset starRunTime = DateTimeOffset.Now.AddSeconds(60);
                //结束时间处理
                DateTimeOffset endRunTime = DateTimeOffset.MaxValue.AddDays(-1);
                // 创建一个时间触发器
                SimpleTriggerImpl trigger = new SimpleTriggerImpl
                {
                    Key = new TriggerKey(jobName, "jcode_timer_group"),
                    StartTimeUtc = starRunTime,
                    EndTimeUtc = endRunTime,
                    RepeatCount = 0
                };

                await scheduler.ScheduleJob(job, trigger);
            }
            catch (System.Exception)
            {
            }
        }
    }
}