namespace jcode.util
{
    /// <summary>
    /// 发送短信所需的运行期配置。
    /// 独立于 appsettings：页面保存到 lr_base_config(smsConfig) 后，发送链路优先使用这份配置；
    /// 未保存过配置时仍用 appsettings 的 ServerOp 兜底。
    /// </summary>
    public class SmsOptions
    {
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
        /// 阿里云 AccessKeySecret
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
        /// 腾讯云 SecretKey
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
        /// 用 appsettings 的 ServerOp 构造一份配置（未保存页面配置时的兜底）
        /// </summary>
        /// <param name="config">服务端配置</param>
        /// <returns>短信配置；config 为空时返回 null</returns>
        public static SmsOptions FromServerOp(ServerOp config)
        {
            if (config == null)
            {
                return null;
            }

            return new SmsOptions
            {
                SmsType = string.IsNullOrEmpty(config.SmsType) ? "ali" : config.SmsType,
                SmsSignName = config.SmsSignName,
                SmsAccessKeyId = config.SmsAccessKeyId,
                SmsSecret = config.SmsSecret,
                TencentSecretId = config.TencentSecretId,
                TencentSecretKey = config.TencentSecretKey,
                TencentSmsAccessKeyId = config.TencentSmsAccessKeyId,
                TemplateParamName = "content",
            };
        }
    }
}
