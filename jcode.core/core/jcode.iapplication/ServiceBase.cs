using System;
using jcode.orm;
using jcode.util;
using SqlSugar;

namespace jcode.iapplication
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2020.04.20
    /// 描 述：服务基类
    /// </summary>
    public class ServiceBase: RepositoryFactory
    {
        /// <summary>
        /// 获取登录者用户名称
        /// </summary>
        /// <returns></returns>
        public string GetUserName() {
            return ContextHelper.GetItem("userName") as string;
        }
        /// <summary>
        /// 获取登录者用户Id
        /// </summary>
        /// <returns></returns>
        public string GetUserId()
        {
            return ContextHelper.GetItem("userId") as string;
        }
        /// <summary>
        /// 获取登录者用户账号
        /// </summary>
        /// <returns></returns>
        public string GetUserAccount()
        {
            return ContextHelper.GetItem("account") as string;
        }


        /// <summary>
        /// 获取租户号
        /// </summary>
        /// <returns></returns>
        public string GetTenantNo()
        {
            return ContextHelper.GetItem("TenantNo") as string;
        }

        /// <summary>
        /// 获取租户名称
        /// </summary>
        /// <returns></returns>
        public string GetTenantName()
        {
            return ContextHelper.GetItem("TenantName") as string;
        }


        /// <summary>
        /// 获取租户公司名
        /// </summary>
        /// <returns></returns>
        public string GetTenantCompany()
        {
            return ContextHelper.GetItem("TenantCompany") as string;
        }


        /// <summary>
        /// 获取用户安全等级 1超级用户 2三员-系统管理员、3三员-安全管理员、4三员-审计员
        /// </summary>
        /// <returns></returns>
        public string GetSecurityLevel()
        {
            return ContextHelper.GetItem("SecurityLevel") as string;
        }
        
        /// <summary>
        /// 判断是否是超级管理员
        /// </summary>
        /// <returns></returns>
        public bool IsAdmin()
        {
            var res = GetSecurityLevel();
            
            if (res == "1")
            {
                return true;
            }

            return false;
        }
        
        
        /// <summary>
        /// 判断是否是系统管理员
        /// </summary>
        /// <returns></returns>
        public bool IsSystemManager()
        {
            var res = GetSecurityLevel();
            
            if (res == "2")
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 判断是否是系统安全员
        /// </summary>
        /// <returns></returns>
        public bool IsSystemSafety()
        {
            var res = GetSecurityLevel();
            
            if (res == "3")
            {
                return true;
            }

            return false;
        }
        
        /// <summary>
        /// 判断是否是系统安全员
        /// </summary>
        /// <returns></returns>
        public bool IsSystemAuditor()
        {
            var res = GetSecurityLevel();
            
            if (res == "4")
            {
                return true;
            }

            return false;
        }
        
        
        
        


        /// <summary>
        /// 获取当前用户密级
        /// </summary>
        /// <returns></returns>
        public int GetSecretLevel()
        {
            var secretLevel = ContextHelper.GetItem("SecretLevel") as string;
            if (string.IsNullOrEmpty(secretLevel))
            {
                return 0;
            }
            else
            {
                return  Int32.Parse(secretLevel);
            }
        }
        
        /// <summary>
        /// 转换值
        /// </summary>
        /// <param name="value">数据值</param>
        /// <param name="csType">值的类型</param>
        /// <returns></returns>
        public object ConvertValue(string value, string csType)
        {
            string _csType = csType;
            switch (csType)
            {
                case "int":
                    _csType = "System.Int32";
                    break;
                case "long":
                    _csType = "System.Int64";
                    break;
                case "short":
                    _csType = "System.Int16";
                    break;
                case "bool":
                    _csType = "System.Boolean";
                    break;
                case "float":
                    _csType = "System.Single";
                    break;
                case "text":
                    _csType = "";
                    break;
                default:
                    if (!string.IsNullOrEmpty(csType))
                    {
                        _csType = $"System.{Str.FirstUpper(csType)}";
                    }
                    break;
            }
            if (_csType == "System.TimeSpan")
            {
                return TimeSpan.Parse(value);
            }

            if (_csType == "System.Guid")
            {
                return Guid.Parse(value);
            }
            
            return string.IsNullOrEmpty(_csType) ? value : Convert.ChangeType(value,System.Type.GetType(_csType));
        }
    }
}
