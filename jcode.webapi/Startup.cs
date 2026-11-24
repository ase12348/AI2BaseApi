using Autofac;
using Autofac.Extensions.DependencyInjection;
using jcode.autofac;
using jcode.application;
using jcode.cache;
using jcode.iapplication;
using jcode.operat;
using jcode.oss;
using jcode.util;
using jcode.web.util;
using jcode.web.util.JWT;
using jcode.web.util.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Extensions = jcode.autofac.Extensions;

namespace jcode.webapi
{
    /// <summary>
    /// 
    /// </summary>
    public class Startup
    {
        private const string CorsName = "SignalRCors";

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="env"></param>
        /// <param name="configuration"></param>
        public Startup(IWebHostEnvironment env,IConfiguration configuration)
        {
            Configuration = configuration;
            string baseDir = env.ContentRootPath;
            ConfigHelper.SetValue("baseDir", baseDir);
            if (env.IsDevelopment())
            {
                ConfigHelper.SetValue("env", "dev");
            }
            else
            {
                ConfigHelper.SetValue("env", "pro");
            }

            
        }

        
        /// <summary>
        /// 
        /// </summary>
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddCors(op => { op.AddPolicy(CorsName, set => { set.SetIsOriginAllowed(origin => true).AllowAnyHeader().AllowAnyMethod().AllowCredentials(); }); });

            services.AddResponseCompression();

            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss");
            services.AddHttpContextAccessor();

            //services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            #region 缓存
            if (ConfigHelper.GetConfig().CacheType == "Redis")
            {
                services.AddSingleton<ICache, CacheByRedis>();
            }
            else
            {
                services.AddMemoryCache();
                services.AddSingleton<ICache, MemoryCache>();
            }
            #endregion

            if (ConfigHelper.GetConfig().IsSwagger  || ConfigHelper.GetValue<string>("env") == "dev") {
                //配置Swagger
                //注册Swagger生成器，定义一个Swagger 文档
                services.AddSwaggerGen(c =>
                {
                    GeoJSONSchemes.ConfigureForNetTopologySuite(c);

                    // Swashbuckle 10 会输出 [DefaultValue] 默认值，DataColumn.DataType 的默认值是 System.Type，System.Text.Json 无法序列化
                    c.MapType<DataTable>(() => new OpenApiSchema { Type = JsonSchemaType.Object });

                    // 默认 schemaId 只用类型短名：不同程序集存在同全名类型时整个文档 500
                    c.CustomSchemaIds(SchemaIdConvention.Of);


                    Extensions.GetAssemblies().Where(t => t.Location.EndsWith("controllers.dll")).ToList().ForEach(assembly =>
                    {
                        c.IncludeXmlComments(assembly.Location.Replace(".dll", ".xml"));
                    });

                    c.SwaggerDoc("v2", new OpenApiInfo
                    {
                        Version = "v2",
                        Title = "接口文档",
                        Description = "极简搭AI应用开发基座webapi"
                    });


                    //swagger中控制请求的时候发是否需要在url中增加accesstoken
                    c.OperationFilter<AddAuthTokenHeaderParameter>();

                    // 为 Swagger 设置xml文档注释路径
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var basePath = Path.GetDirectoryName(typeof(Program).Assembly.Location);//获取应用程序所在目录
                    var xmlPath = Path.Combine(basePath, xmlFile);
                    c.IncludeXmlComments(xmlPath);

                    
                    // iapplication
                    xmlPath = Path.Combine(basePath, "jcode.iapplication.xml");
                    c.IncludeXmlComments(xmlPath);
                });
                
  

            }
           
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            // 在这里添加服务注册
            builder.RegisterIBLL();

            builder.RegisterOss(); // 注册文件存储
            builder.RegisterType(typeof(Operator)).As(typeof(IOperator)).SingleInstance();
            builder.RegisterType(typeof(LogBLL)).As(typeof(LogIBLL)).SingleInstance();
        }





        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public async void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            IocManager.Instance.Container = app.ApplicationServices.GetAutofacRoot();
            var prefix = string.Empty;

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                prefix = ConfigHelper.GetConfig().RoutePrefix;
            }

            // 允许跨域访问
            //app.UseCors(option => option.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
            app.UseCors(CorsName);
            // app.UseStaticFiles();
            app.UseResponseCompression();
            //app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization();

            if (ConfigHelper.GetConfig().IsSwagger || ConfigHelper.GetValue<string>("env") == "dev")
            {
                //启用中间件服务生成Swagger
                app.UseSwagger(c =>
                {
                    c.PreSerializeFilters.Add((swaggerDoc, httpReq) =>
                    {
                        if (!string.IsNullOrEmpty(prefix))
                        {
                        //api测试时增加虚拟目录 或完整路径也可以，完整路径已被webmote注释
                        swaggerDoc.Servers = new List<OpenApiServer> { new OpenApiServer {
                                Url = $"{prefix}"
                            }
                            };
                        }
                    });
                });
                //启用中间件服务生成SwaggerUI，指定Swagger JSON终结点
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint($"{prefix}/swagger/v2/swagger.json", "jcode.webapi.v2");
                    //c.SwaggerEndpoint($"{prefix}/swagger/mes/swagger.json", "jcode.webapi.mes");
                    c.RoutePrefix = string.Empty;//设置根节点访问
                });
            }
            

            
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<ChatsHub>("/ChatsHub");
                
            });
            // SQLite 主库初始化：用自带的 sqlite_base.db 元数据建表并灌种子数据。
            // 内部按版本号比对，已是最新版本会直接跳过，可安全重复执行。
            if (env.IsDevelopment())
            {
                try
                {
                    await IocManager.Instance.GetService<CodeTableIBLL>().ImportDatabaseBySqllite(
                         $"{ConfigHelper.GetValue<string>("baseDir")}/wwwroot/database/sqlite_base.db", string.Empty);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }

            try
            {
                await QuartzJobScheduler.Init();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }



            // 更新层级编码
            // await IocManager.Instance.GetService<CompanyIBLL>().InitDeepNum();
            // await IocManager.Instance.GetService<DepartmentIBLL>().InitDeepNum();
            // Console.WriteLine("更新完层级编码");
            // 消息队列案例
            /*
            MQServiceFactory.BasicConsume("test", async (mes) =>
            {
                Trace.WriteLine($" [x] Received {mes}");
            });
            MQServiceFactory.BasicPublish("测试","test");
            */



            //DocumentConverter.ToHtml("C:\\work\\CE\\ce-nuget\\ce.office.extension\\2020-01总账.xlsx", "C:\\work\\CE\\ce-nuget\\ce.office.extension\\wodeceshi");
        }
    }
}
