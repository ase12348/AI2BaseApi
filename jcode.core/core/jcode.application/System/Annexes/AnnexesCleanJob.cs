using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using jcode.autofac;
using jcode.iapplication;
using jcode.operat;
using Quartz;

namespace jcode.application
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 描 述：附件清理定时任务，回收遗留的暂存附件与业务表已无引用的孤儿附件
    /// </summary>
    public class AnnexesCleanJob : IJob
    {
        private readonly AnnexesFileIBLL annexesFileIBLL;

        /// <summary>
        /// 构造方法
        /// </summary>
        public AnnexesCleanJob()
        {
            annexesFileIBLL = IocManager.Instance.GetService<AnnexesFileIBLL>();
        }

        /// <summary>
        /// 执行任务方法
        /// </summary>
        /// <param name="context"></param>
        /// <param name="cancellationToken"></param>
        public async ValueTask Execute(IJobExecutionContext context, CancellationToken cancellationToken)
        {
            var logIBLL = IocManager.Instance.GetService<LogIBLL>();
            int draftCount = 0;
            int orphanCount = 0;
            var errors = new List<string>();

            // 暂存附件夹不会被业务数据引用，先回收遗留暂存附件，再判断孤儿附件。
            // 两段各自兜住异常，任何一段失败都不能跳过另一段
            try
            {
                draftCount = await annexesFileIBLL.CleanDraftFiles();
            }
            catch (Exception ex)
            {
                errors.Add("【暂存附件清理】" + logIBLL.ExceptionFormat(ex));
            }

            try
            {
                orphanCount = await annexesFileIBLL.CleanOrphanFiles();
            }
            catch (Exception ex)
            {
                errors.Add("【孤儿附件清理】" + logIBLL.ExceptionFormat(ex));
            }

            if (draftCount > 0 || orphanCount > 0)
            {
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = 1;
                logEntity.F_ExecuteResultJson = $"【附件清理】暂存附件 {draftCount} 个，孤儿附件 {orphanCount} 个";
                logIBLL.Write(logEntity, null);
            }

            foreach (var error in errors)
            {
                LogEntity logEntity = new LogEntity();
                logEntity.F_CategoryId = 4;
                logEntity.F_ExecuteResult = -1;
                logEntity.F_ExecuteResultJson = error;
                logIBLL.Write(logEntity, null);
            }
        }
    }
}
