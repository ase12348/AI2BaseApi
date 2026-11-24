using System.Threading.Tasks;
using jcode.autofac;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR.Client;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：发送消息给SignalR集结器
    /// </summary>
    public static class SendHubs
    {
        /// <summary>
        /// 调用hub方法
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="args">参数</param>
        public static async Task callMethod(string methodName, params object[] args)
        {
            if (ConfigHelper.GetConfig().IMOpen) {
                var imurl = ConfigHelper.GetConfig().IMUrl;
                if (string.IsNullOrEmpty(imurl)) {
                    var accessor = IocManager.Instance.GetService<IHttpContextAccessor>();
                    if (accessor?.HttpContext != null)
                    {
                        imurl = accessor.HttpContext.Request.Host.Value;
                        if (accessor.HttpContext.Request.IsHttps)
                        {
                            imurl = $"https://{imurl}";
                        }
                        else {
                            imurl = $"http://{imurl}";
                        }
                    }
                }

                if (!string.IsNullOrEmpty(imurl))
                {
                    var connection = new HubConnectionBuilder()
                        .WithUrl($"{imurl}/ChatsHub")
                        .Build();
                    await connection.StartAsync();
                    await connection.InvokeCoreAsync(methodName, args);
                    await connection.StopAsync();
                }
            }
            
        }
    }
}
