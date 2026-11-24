using NLog;
using System;


namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2024.09.20
    /// 描 述：日志帮助类
    /// </summary>
    public class NLogHelper
    {
        /// <summary>
        /// 写入文本
        /// </summary>
        private  static readonly Logger FileLogger = LogManager.GetLogger("logfile");
        
        
        /// <summary>
        /// 写日志到文件
        /// </summary>
        /// <param name="logLevel">日志等级</param>
        /// <param name="message">信息</param>
        public static void WriteFileLog(LogLevel logLevel,string message)
        {
            LogEventInfo theEvent = new LogEventInfo(logLevel, FileLogger.Name, message);
            var userId = ContextHelper.GetItem("userId")?.ToString();
            if (string.IsNullOrEmpty(userId))
            {
                userId = "soft";
            }

            theEvent.Properties["path"] = ConfigHelper.GetConfig().FilePath;
            theEvent.Properties["userId"] = userId;
            theEvent.Properties["dateTime"] = DateTime.Now.ToString("yyyy-MM-dd--HHmm");
            theEvent.Properties["message"] = message;//请求标识
            FileLogger.Log(theEvent);
        }
    }
}
