using System.Threading.Tasks;
using jcode.autofac;

namespace jcode.iapplication
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-18 09:37:38
    /// 描 述： 系统配置-登录策略
    /// </summary>
    public interface ISystemConfigBLL : IBLL
    {
        #region 获取数据
        /// <summary>
        /// 获取实体数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task<SystemConfigEntity> GetEntity(string keyValue);
        #endregion

        #region 提交数据

        /// <summary>
        /// 删除实体
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <returns></returns>
        Task Delete(string keyValue);
        /// <summary>
        /// 保存数据
        /// </summary>
        /// <param name="keyValue">主键</param>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveEntity(string keyValue, SystemConfigEntity entity);
        #endregion
        
        #region 登录配置

        /// <summary>
        /// 保存登录配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SaveLoginConfig(SystemConfigEntity entity);

        /// <summary>
        /// 获取登录配置
        /// </summary>
        /// <returns></returns>
        Task<ConfigLoginModel> GetLoginConfig();
        #endregion
        
        #region 密码配置

        /// <summary>
        /// 保存登录配置
        /// </summary>
        /// <param name="entity">实体</param>
        /// <returns></returns>
        Task SavePwdConfig(SystemConfigEntity entity);

        /// <summary>
        /// 获取登录配置
        /// </summary>
        /// <returns></returns>
        Task<ConfigPwdModel> GetPwdConfig();

        #endregion

        #region 发送短信配置

        /// <summary>
        /// 保存发送短信配置（密钥为掩码或留空时保留原值）
        /// </summary>
        /// <param name="entity">实体，F_Content 为 ConfigSmsModel 的 JSON</param>
        /// <returns></returns>
        Task SaveSmsConfig(SystemConfigEntity entity);

        /// <summary>
        /// 获取发送短信配置（原始值，供发送链路使用；回显给页面前需调用 Masked）
        /// </summary>
        /// <returns>未配置时返回 null</returns>
        Task<ConfigSmsModel> GetSmsConfig();

        #endregion
    }
}
