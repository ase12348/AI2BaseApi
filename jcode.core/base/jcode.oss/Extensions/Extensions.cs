using System;
using System.Reflection;
using Autofac;
using jcode.util;

namespace jcode.oss
{
    public static class Extensions
    {
        /// <summary>
        /// 注册业务方法
        /// </summary>
        /// <param name="builder">容器</param>
        /// <returns></returns>
        public static ContainerBuilder RegisterOss(this ContainerBuilder builder)
        {
            try
            {
                #region 附件上传
                if (ConfigHelper.GetConfig().AnnexesType == "AliYunOss")
                {
                    builder.RegisterType(typeof(AliOssAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else if (ConfigHelper.GetConfig().AnnexesType == "TencentOss")
                {
                    builder.RegisterType(typeof(TencentOssAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else if (ConfigHelper.GetConfig().AnnexesType == "HuaWeiOss")
                {
                    builder.RegisterType(typeof(HuaWeiOssAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else if (ConfigHelper.GetConfig().AnnexesType == "MinIoOss")
                {
                    builder.RegisterType(typeof(MinIoOssAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else if (ConfigHelper.GetConfig().AnnexesType == "Db")
                {
                    builder.RegisterType(typeof(DbAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else if (ConfigHelper.GetConfig().AnnexesType == "QiniuOss")
                {
                    builder.RegisterType(typeof(QiniuOssAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                else
                {
                    builder.RegisterType(typeof(FileAnnexes)).As(typeof(IAnnexes)).SingleInstance();
                }
                #endregion
                
                builder.RegisterType(typeof(FileAnnexes)).Named<IAnnexes>("Local").SingleInstance();
                builder.RegisterType(typeof(DbAnnexes)).Named<IAnnexes>("Db").SingleInstance();
                builder.RegisterType(typeof(MinIoOssAnnexes)).Named<IAnnexes>("MinIoOss").SingleInstance();
                builder.RegisterType(typeof(HuaWeiOssAnnexes)).Named<IAnnexes>("HuaWeiOss").SingleInstance();
                builder.RegisterType(typeof(TencentOssAnnexes)).Named<IAnnexes>("TencentOss").SingleInstance();
                builder.RegisterType(typeof(AliOssAnnexes)).Named<IAnnexes>("AliYunOss").SingleInstance();
                builder.RegisterType(typeof(QiniuOssAnnexes)).Named<IAnnexes>("QiniuOss").SingleInstance();
            }
            catch (Exception ex)
            {
                if (ex is ReflectionTypeLoadException)
                {
                    throw (ex as ReflectionTypeLoadException).LoaderExceptions[0];
                }
                throw;
            }
            return builder;
        }
    }
}