using System.Threading.Tasks;
using Quartz;


namespace jcode.timer
{
    public class TimerHelper
    {
        
        /// <summary>
        /// 定时任务
        /// </summary>
        /// <returns></returns>
        public delegate Task TimerMethod(bool isInit);
        
        private static ISchedulerFactory _schedulerFactory = null;
        private static IScheduler _Scheduler = null;

        private static async Task CreateScheduler() {
            _schedulerFactory = QuartzSchedulerBuilder.Create().Build();//构建调度工厂
            _Scheduler = await _schedulerFactory.GetScheduler();
            await _Scheduler.Start();
        }
        
        
        
        /// <summary>
        /// 获取任务调度器
        /// </summary>
        /// <returns></returns>
        public static IScheduler GetScheduler()
        {
            if (_Scheduler == null)
            {
                CreateScheduler().GetAwaiter();
            }
            return _Scheduler;
        }


    }
}