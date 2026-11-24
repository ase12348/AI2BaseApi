using Autofac.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System;

namespace jcode.webapi
{
    /// <summary>
    /// 方法入口类
    /// </summary>
    public class Program
    {
        /// <summary>
        /// 入口函数
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }
        /// <summary>
        /// 构建服务
        /// </summary>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>()
                    .UseKestrel(option =>
                    {
                        option.Limits.MinRequestBodyDataRate = null;
                        // 整份上传的附件接口（system/annexesfile/{fileId}，移动端用）单请求就要装下 100MB，
                        // Kestrel 默认 30MB 会在进入控制器前直接 413。放宽到 110MB。
                        option.Limits.MaxRequestBodySize = 110L * 1024 * 1024;
                        //option.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(5);
                        //option.Limits.RequestHeadersTimeout = TimeSpan.FromMinutes(5);
                    });
                });
    }
}
