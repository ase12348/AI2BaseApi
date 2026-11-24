using System;
using Quartz;
using System.Threading.Tasks;
using jcode.timer;

namespace jcode.application
{
    /// <summary>
    /// 附件处理定时器
    /// </summary>
    public class FileJobScheduler
    {
        /// <summary>
        /// 开启任务调度器
        /// </summary>
        public static async Task Start()
        {
            try
            {
                IScheduler scheduler = TimerHelper.GetScheduler();
                await scheduler.Start();
                TriggerKey triggerKey = new TriggerKey("file_temp_delete_trigger", "fileGroup");
                if (!await scheduler.Exists(triggerKey))
                {
                    IJobDetail job = JobBuilder.Create<FileJob>().Build();
                    ITrigger trigger = TriggerBuilder.Create()
                        .WithIdentity("file_temp_delete_trigger", "fileGroup")
                        .WithSimpleSchedule(t =>
                            t.WithInterval(TimeSpan.FromMinutes(30)) //测试调整为每10分钟触发一次
                                .RepeatForever())
                        .Build();

                    await scheduler.ScheduleJob(job, trigger);
                }

                // 附件清理（暂存附件 + 孤儿附件），每天凌晨3点执行
                TriggerKey cleanTriggerKey = new TriggerKey("annexes_clean_trigger", "fileGroup");
                if (!await scheduler.Exists(cleanTriggerKey))
                {
                    IJobDetail cleanJob = JobBuilder.Create<AnnexesCleanJob>()
                        .WithIdentity("annexes_clean", "fileGroup")
                        .Build();
                    ITrigger cleanTrigger = TriggerBuilder.Create()
                        .WithIdentity("annexes_clean_trigger", "fileGroup")
                        .WithCronSchedule("0 0 3 * * ?")
                        .Build();

                    await scheduler.ScheduleJob(cleanJob, cleanTrigger);
                }
            }
            catch (Exception)
            {
            }

        }
    }


}
