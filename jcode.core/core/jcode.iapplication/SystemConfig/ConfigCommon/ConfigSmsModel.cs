using jcode.util;

namespace jcode.iapplication
{
    /// <summary>
    /// 系统配置-发送短信配置。
    /// 与登录策略、密码规范同存一张 lr_base_config 表，主键固定为 smsConfig，正文是本模型的 JSON。
    /// </summary>
    public class ConfigSmsModel
    {
        /// <summary>
        /// 密钥在回显时的掩码：页面不改密钥时原样提交，服务端用旧值回填
        /// </summary>
        public const string SecretMask = "******";

        /// <summary>
        /// 是否启用短信发送（1 启用，0 停用）
        /// </summary>
        public int IsEnabled { get; set; } = 1;

        /// <summary>
        /// 服务商：ali=阿里云短信，tencent=腾讯云短信
        /// </summary>
        public string SmsType { get; set; } = "ali";

        /// <summary>
        /// 短信签名（阿里云、腾讯云共用）
        /// </summary>
        public string SmsSignName { get; set; }

        /// <summary>
        /// 阿里云 AccessKeyId
        /// </summary>
        public string SmsAccessKeyId { get; set; }

        /// <summary>
        /// 阿里云 AccessKeySecret（读回时为掩码）
        /// </summary>
        public string SmsSecret { get; set; }

        /// <summary>
        /// 阿里云短信模板 CODE
        /// </summary>
        public string TemplateCode { get; set; }

        /// <summary>
        /// 腾讯云 SecretId
        /// </summary>
        public string TencentSecretId { get; set; }

        /// <summary>
        /// 腾讯云 SecretKey（读回时为掩码）
        /// </summary>
        public string TencentSecretKey { get; set; }

        /// <summary>
        /// 腾讯云短信 SdkAppId
        /// </summary>
        public string TencentSmsAccessKeyId { get; set; }

        /// <summary>
        /// 腾讯云短信模板 ID
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// 短信模板里的变量名（阿里云 JSON 参数的 key；腾讯云按顺序取第一个参数）
        /// </summary>
        public string TemplateParamName { get; set; } = "content";

        /// <summary>
        /// 返回一份可安全回显到页面的副本，密钥统一打码；不改动原对象（原对象可能来自缓存）
        /// </summary>
        /// <returns>脱敏副本</returns>
        public ConfigSmsModel Masked()
        {
            var copy = (ConfigSmsModel)MemberwiseClone();
            copy.SmsSecret = string.IsNullOrEmpty(SmsSecret) ? string.Empty : SecretMask;
            copy.TencentSecretKey = string.IsNullOrEmpty(TencentSecretKey) ? string.Empty : SecretMask;
            return copy;
        }

        /// <summary>
        /// 转成发送短信用的运行期配置
        /// </summary>
        /// <returns>短信配置</returns>
        public SmsOptions ToSmsOptions()
        {
            return new SmsOptions
            {
                SmsType = SmsType,
                SmsSignName = SmsSignName,
                SmsAccessKeyId = SmsAccessKeyId,
                SmsSecret = SmsSecret,
                TemplateCode = TemplateCode,
                TencentSecretId = TencentSecretId,
                TencentSecretKey = TencentSecretKey,
                TencentSmsAccessKeyId = TencentSmsAccessKeyId,
                TemplateId = TemplateId,
                TemplateParamName = TemplateParamName,
            };
        }
    }

    /// <summary>
    /// 短信配置测试发送请求
    /// </summary>
    public class SmsTestModel
    {
        /// <summary>
        /// 接收测试短信的手机号
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 测试短信内容（作为模板变量的值）
        /// </summary>
        public string Content { get; set; }
    }
}
