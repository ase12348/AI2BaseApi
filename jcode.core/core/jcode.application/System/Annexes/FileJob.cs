using jcode.autofac;
using jcode.iapplication;
using jcode.operat;
using jcode.util;
using Quartz;
using System.Threading;
using System.Threading.Tasks;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.02.12
    /// 描 述：流程定时执行任务
    /// </summary>
    public class FileJob : IJob
    {
        private readonly AnnexesFileIBLL annexesFileIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        public FileJob()
        {
            annexesFileIBLL = IocManager.Instance.GetService<AnnexesFileIBLL>();
        }
        /// <summary>
        /// 执行任务方法
        /// </summary>
        /// <param name="context"></param>
        public ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            try
            {
                // 删除临时文件
                annexesFileIBLL.DeleteTempFile();
            }
            catch(System.Exception ex)
            {
                var logIBLL = IocManager.Instance.GetService<LogIBLL>();
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = -1;
                logEntity.F_ExecuteResultJson ="【删除临时文件】" + logIBLL.ExceptionFormat(ex);
                logIBLL.Write(logEntity,null);
            }

            return ValueTask.CompletedTask;
        }
    }
}
