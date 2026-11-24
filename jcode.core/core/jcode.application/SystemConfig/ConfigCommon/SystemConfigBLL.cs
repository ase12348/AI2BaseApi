using jcode.autofac;
using jcode.util;
using jcode.iapplication;
using System.Collections.Generic;
using System.Threading.Tasks;
using jcode.cache;

namespace jcode.application
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-18 09:37:38
    /// 描 述： 系统配置-登录策略
    /// </summary>
    public class SystemConfigBLL : ISystemConfigBLL,BLL
    {
        private readonly ICache _iCache;
        private readonly SystemConfigService systemConfigService = new SystemConfigService();
        
        
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iCache"></param>
        public SystemConfigBLL(ICache iCache)
        {
            _iCache = iCache;
        }
        
        #region 获取数据
        
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task<SystemConfigEntity> GetEntity(string keyValue)
        {
            string key = $"systemconfig_{keyValue}";
            var entity = _iCache.Read<SystemConfigEntity>(key);
            if (entity == null) // 如果账号被锁定就再去数据库获取
            {
                entity = await systemConfigService.GetEntity(keyValue);
                _iCache.Write(key, entity);
            }
            
            return entity;
        }
        
        #endregion

        #region 提交数据
        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        public async Task Delete(string keyValue)
        {
            await systemConfigService.Delete(keyValue);
            _iCache.Remove($"systemconfig_{keyValue}");
        }
        
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveEntity(string keyValue, SystemConfigEntity entity)
        {
            await systemConfigService.SaveEntity(keyValue, entity);
            _iCache.Remove($"systemconfig_{keyValue}");
        }
        #endregion

        #region 登录配置
        /// <summary>
        /// 保存登录配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SaveLoginConfig(SystemConfigEntity entity)
        {
            var keyValue = "loginConfig";
            entity.F_Id = keyValue;

            var oldEntity = await GetEntity(keyValue);
            if (oldEntity == null)
            {
                keyValue = "";
            }
            await SaveEntity(keyValue, entity);
        }
        
        /// <summary>
        /// 获取登录配置
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigLoginModel> GetLoginConfig()
        { 
            var keyValue = "loginConfig";
            var entity = await GetEntity(keyValue);
            if (entity != null && !string.IsNullOrEmpty(entity.F_Content))
            {
                return entity.F_Content.ToObject<ConfigLoginModel>();
            }

            return null;
        }
        #endregion
        
        #region 密码配置
        /// <summary>
        /// 保存登录配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        public async Task SavePwdConfig(SystemConfigEntity entity)
        {
            var keyValue = "pwdConfig";
            entity.F_Id = keyValue;

            var oldEntity = await GetEntity(keyValue);
            if (oldEntity == null)
            {
                keyValue = "";
            }
            await SaveEntity(keyValue, entity);
        }
        
        /// <summary>
        /// 获取登录配置
        /// </summary>
        /// <returns></returns>
        public async Task<ConfigPwdModel> GetPwdConfig()
        { 
            var keyValue = "pwdConfig";
            var entity = await GetEntity(keyValue);
            if (entity != null && !string.IsNullOrEmpty(entity.F_Content))
            {
                return entity.F_Content.ToObject<ConfigPwdModel>();
            }

            return null;
        }
        #endregion

        #region 发送短信配置
        /// <summary>
        /// 保存发送短信配置。页面回显的密钥是掩码，掩码或留空都表示「不修改」，此处用旧值回填。
        /// </summary>
        /// <param name="entity">实体，F_Content 为 ConfigSmsModel 的 JSON</param>
        public async Task SaveSmsConfig(SystemConfigEntity entity)
        {
            const string keyValue = "smsConfig";
            var config = string.IsNullOrEmpty(entity.F_Content)
                ? new ConfigSmsModel()
                : entity.F_Content.ToObject<ConfigSmsModel>() ?? new ConfigSmsModel();

            var oldConfig = await GetSmsConfig();
            if (oldConfig != null)
            {
                config.SmsSecret = KeepSecret(config.SmsSecret, oldConfig.SmsSecret);
                config.TencentSecretKey = KeepSecret(config.TencentSecretKey, oldConfig.TencentSecretKey);
            }

            entity.F_Id = keyValue;
            entity.F_Content = config.ToJson();

            var oldEntity = await GetEntity(keyValue);
            await SaveEntity(oldEntity == null ? string.Empty : keyValue, entity);
        }

        /// <summary>
        /// 获取发送短信配置（原始值，只供发送链路使用）
        /// </summary>
        /// <returns>未配置时返回 null</returns>
        public async Task<ConfigSmsModel> GetSmsConfig()
        {
            var entity = await GetEntity("smsConfig");
            if (entity != null && !string.IsNullOrEmpty(entity.F_Content))
            {
                return entity.F_Content.ToObject<ConfigSmsModel>();
            }

            return null;
        }

        /// <summary>
        /// 掩码/空值保留旧密钥
        /// </summary>
        private static string KeepSecret(string value, string oldValue)
        {
            return string.IsNullOrWhiteSpace(value) || value == ConfigSmsModel.SecretMask ? oldValue : value;
        }
        #endregion
    }
}
