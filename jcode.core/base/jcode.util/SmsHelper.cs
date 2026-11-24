using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.Core.Profile;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Sms.V20210111;
using TencentCloud.Sms.V20210111.Models;

namespace jcode.util
{
    /// <summary>
    /// 发送短信
    /// </summary>
    public class SmsHelper
    {
        #region 统一入口
        /// <summary>
        /// 按配置发送短信（页面配置优先）。
        /// 阿里云：模板参数按 {模板变量名: 内容} 组装成 JSON；
        /// 腾讯云：内容作为模板的第一个参数。模板变量个数需与短信平台审核通过的模板一致。
        /// </summary>
        /// <param name="tel">手机号码</param>
        /// <param name="content">短信内容（模板变量的值）</param>
        /// <param name="options">短信配置，空时返回 null</param>
        /// <returns>短信平台原始响应</returns>
        public static string SendSms(string tel, string content, SmsOptions options)
        {
            if (options == null || string.IsNullOrWhiteSpace(tel))
            {
                return null;
            }

            var text = content ?? string.Empty;
            if (string.Equals(options.SmsType, "tencent", StringComparison.OrdinalIgnoreCase))
            {
                var paramSet = string.IsNullOrEmpty(text) ? new string[0] : new[] { text };
                return SendSmsByTencent(new[] { tel }, options.TemplateId, paramSet, options);
            }

            var paramName = string.IsNullOrWhiteSpace(options.TemplateParamName) ? "content" : options.TemplateParamName;
            var templateParam = JsonConvert.SerializeObject(new Dictionary<string, string> { { paramName, text } });
            return SendSmsByAli(tel, templateParam, options.TemplateCode, options);
        }
        #endregion

        #region 发送阿里短信
        /// <summary>
        /// 发送阿里短信（用 appsettings 里的配置，兼容原有调用）
        /// </summary>
        /// <param name="tel">手机号码</param>
        /// <param name="templateParam">短信模板参数</param>
        /// <param name="templateCode">短信模板编码</param>
        /// <returns></returns>
        public static string SendSmsByAli(string tel, string templateParam, string templateCode)
        {
            return SendSmsByAli(tel, templateParam, templateCode, SmsOptions.FromServerOp(ConfigHelper.GetConfig()));
        }

        /// <summary>
        /// 发送阿里短信（用指定配置，页面保存的短信配置走这条）
        /// </summary>
        /// <param name="tel">手机号码</param>
        /// <param name="templateParam">短信模板参数</param>
        /// <param name="templateCode">短信模板编码</param>
        /// <param name="options">短信配置</param>
        /// <returns></returns>
        public static string SendSmsByAli(string tel, string templateParam, string templateCode, SmsOptions options)
        {
            if (options != null
                && !string.IsNullOrEmpty(options.SmsAccessKeyId)
                && !string.IsNullOrEmpty(options.SmsSecret))
            {
                DefaultAcsClient client = new DefaultAcsClient(DefaultProfile.GetProfile("default", options.SmsAccessKeyId, options.SmsSecret));
                Aliyun.Acs.Core.CommonRequest request = new Aliyun.Acs.Core.CommonRequest();
                request.Method = new MethodType?(MethodType.POST);
                request.Domain = "dysmsapi.aliyuncs.com";
                request.Version = "2017-05-25";
                request.Action = "SendSms";
                request.AddQueryParameters("PhoneNumbers", tel);

                request.AddQueryParameters("SignName", options.SmsSignName);
                request.AddQueryParameters("TemplateCode", templateCode);
                request.AddQueryParameters("TemplateParam", templateParam);

                string result;
                try
                {
                    CommonResponse response = client.GetCommonResponse(request);
                    result = Encoding.Default.GetString(response.HttpResponse.Content);
                }
                catch (ServerException)
                {
                    result = null;
                }
                catch (ClientException)
                {
                    result = null;
                }
                return result;
            }

            return null;
        }
        #endregion

        #region 发送腾讯短信----ADD BY SSY 2022-05-31
        /// <summary>
        /// 发送腾讯短信（用 appsettings 里的配置，兼容原有调用）
        /// </summary>
        /// <param name="PhoneNumberSet">下发手机号码，采用 E.164 标准，格式为+[国家或地区码][手机号]，单次请求最多支持200个手机号且要求全为境内手机号或全为境外手机号。例如：+8613711112222， 其中前面有一个+号 ，86为国家码，13711112222为手机号。注：发送国内短信格式还支持0086、86或无任何国家或地区码的11位手机号码，前缀默认为+86。</param>
        /// <param name="TemplateId">模板 ID，必须填写已审核通过的模板 ID。</param>
        /// <param name="TemplateParamSet">模板参数，若无模板参数，则设置为空。 模板参数的个数需要与 TemplateId 对应模板的变量个数保持一致。</param>
        /// <returns></returns>
        public static string SendSmsByTencent(string[] PhoneNumberSet, string TemplateId, string[] TemplateParamSet)
        {
            return SendSmsByTencent(PhoneNumberSet, TemplateId, TemplateParamSet, SmsOptions.FromServerOp(ConfigHelper.GetConfig()));
        }

        /// <summary>
        /// 发送腾讯短信（用指定配置，页面保存的短信配置走这条）
        /// </summary>
        /// <param name="PhoneNumberSet">下发手机号码</param>
        /// <param name="TemplateId">模板 ID，必须填写已审核通过的模板 ID。</param>
        /// <param name="TemplateParamSet">模板参数</param>
        /// <param name="options">短信配置</param>
        /// <returns></returns>
        public static string SendSmsByTencent(string[] PhoneNumberSet, string TemplateId, string[] TemplateParamSet, SmsOptions options)
        {
            //以下是参数示例
            //string[] PhoneNumberSet = new string[] { "18888888888" };
            //string TemplateId = "1422222";
            //string[] TemplateParamSet = new string[] { "333650" };
            string result;
            //重要参数非空判断
            if (options != null
                && !string.IsNullOrEmpty(options.TencentSecretId)
                && !string.IsNullOrEmpty(options.TencentSecretKey)
                && !string.IsNullOrEmpty(options.TencentSmsAccessKeyId)
                && !string.IsNullOrEmpty(options.SmsSignName))
            {
                try
                {
                    //API密钥管理【非常重要，用户必须严格保管安全凭证，避免泄露，否则将危及财产安全。如已泄漏，请立刻禁用该安全凭证】
                    Credential cred = new Credential
                    {
                        SecretId = options.TencentSecretId,
                        SecretKey = options.TencentSecretKey
                    };
                    ClientProfile clientProfile = new ClientProfile();
                    HttpProfile httpProfile = new HttpProfile();
                    httpProfile.Endpoint = ("sms.tencentcloudapi.com");
                    clientProfile.HttpProfile = httpProfile;

                    //Region:[华北地区(北京):ap-beijing、华南地区(广州):ap-guangzhou、华东地区(南京):ap-nanjing]
                    SmsClient client = new SmsClient(cred, "ap-nanjing", clientProfile);
                    SendSmsRequest req = new SendSmsRequest();
                    req.PhoneNumberSet = PhoneNumberSet;//下发手机号码
                    req.SmsSdkAppId = options.TencentSmsAccessKeyId;//短信 SdkAppId，在 短信控制台 添加应用后生成的实际 SdkAppId，示例如1400006666。
                    req.SignName = options.SmsSignName;//短信签名内容，使用 UTF-8 编码，必须填写已审核通过的签名，
                    req.TemplateId = TemplateId;//模板 ID，必须填写已审核通过的模板 ID。
                    req.TemplateParamSet = TemplateParamSet;//模板参数，若无模板参数，则设置为空
                    SendSmsResponse resp = client.SendSmsSync(req);
                    result = AbstractModel.ToJsonString(resp);
                }
                catch (ServerException)
                {
                    result = null;
                }
                catch (ClientException)
                {
                    result = null;
                }
                catch (Exception)
                {
                    result = null;
                }
                return result;
            }
            else
            {
                result = "请补全重要参数!";
            }

            return result;
        }
        #endregion
    }
}
