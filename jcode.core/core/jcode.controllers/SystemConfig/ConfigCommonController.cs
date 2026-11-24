using jcode.iapplication;
using jcode.util;
using jcode.web.util;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace jcode.controllers.SystemConfig
{
    /// <summary>
    /// 框架DEV开发-系统配置
    /// Copyright © 2021-present 苏州极简搭信息技术有限公司出品
    /// 创建人：ssy
    /// 日 期： 2023-05-18 09:37:38
    /// 描 述： 系统配置-登录策略
    /// </summary>
    public class ConfigCommonController : BaseApiController
    {
        private readonly ISystemConfigBLL _iSystemConfigBLL;
        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="iSystemConfigBLL">系统配置-登录策略【lr_systemconfig_login】接口</param>
        public ConfigCommonController(ISystemConfigBLL iSystemConfigBLL)
        {
            _iSystemConfigBLL = iSystemConfigBLL?? throw new ArgumentNullException(nameof(iSystemConfigBLL));
        }

        
        #region 获取数据
        /// <summary>
        /// 获取表单数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns></returns>
        [HttpGet("systemconfig/configcommon/{id}")]
        [ProducesResponseType(typeof(ResponseDto<SystemConfigEntity>), 200)]
        public async Task<IActionResult>GetForm(string id)
        {
            var data = await _iSystemConfigBLL.GetEntity(id);
            // 短信配置里有密钥，回显统一打码；另建对象，避免改动缓存里的实体引用
            if (id == "smsConfig" && data != null && !string.IsNullOrEmpty(data.F_Content))
            {
                var config = data.F_Content.ToObject<ConfigSmsModel>();
                if (config != null)
                {
                    return Success(new SystemConfigEntity
                    {
                        F_Id = data.F_Id,
                        F_Content = config.Masked().ToJson(),
                        F_CreateDate = data.F_CreateDate,
                        F_ModifyDate = data.F_ModifyDate,
                    });
                }
            }
            return Success(data);
        }
        #endregion

        #region 提交数据
        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("systemconfig/configcommon/login")]
        [ProducesResponseType(typeof(ResponseDto<SystemConfigEntity>), 200)]
        public async Task<IActionResult>SaveLoginConfig(SystemConfigEntity entity)
        {
            await _iSystemConfigBLL.SaveLoginConfig(entity);
            return Success("保存成功！",entity);
        }
        
        /// <summary>
        /// 新增表单数据
        /// </summary>
        /// <param name="entity">表单数据</param>
        /// <returns></returns>
        [HttpPost("systemconfig/configcommon/pwd")]
        [ProducesResponseType(typeof(ResponseDto<SystemConfigEntity>), 200)]
        public async Task<IActionResult>SavePwdConfig(SystemConfigEntity entity)
        {
            await _iSystemConfigBLL.SavePwdConfig(entity);
            return Success("保存成功！",entity);
        }

        /// <summary>
        /// 保存发送短信配置
        /// </summary>
        /// <param name="entity">表单数据，F_Content 为 ConfigSmsModel 的 JSON</param>
        /// <returns></returns>
        [HttpPost("systemconfig/configcommon/sms")]
        [ProducesResponseType(typeof(ResponseDto<SystemConfigEntity>), 200)]
        public async Task<IActionResult>SaveSmsConfig(SystemConfigEntity entity)
        {
            await _iSystemConfigBLL.SaveSmsConfig(entity);
            return Success("保存成功！",entity);
        }

        /// <summary>
        /// 用已保存的配置发一条测试短信（密钥不出后端）
        /// </summary>
        /// <param name="model">测试手机号与内容</param>
        /// <returns>短信平台原始响应</returns>
        [HttpPost("systemconfig/configcommon/sms/test")]
        [ProducesResponseType(typeof(ResponseDto<string>), 200)]
        public async Task<IActionResult>SendTestSms(SmsTestModel model)
        {
            if (model == null || string.IsNullOrWhiteSpace(model.Phone))
            {
                return Fail("请输入测试手机号");
            }

            var config = await _iSystemConfigBLL.GetSmsConfig();
            if (config == null)
            {
                return Fail("请先保存短信配置");
            }

            var content = string.IsNullOrWhiteSpace(model.Content) ? "短信配置测试" : model.Content;
            var result = SmsHelper.SendSms(model.Phone, content, config.ToSmsOptions());
            if (string.IsNullOrEmpty(result) || result == "请补全重要参数!")
            {
                return Fail("发送失败，请检查短信平台参数与模板");
            }

            return Success("已提交短信平台", result);
        }
        #endregion

    }
}