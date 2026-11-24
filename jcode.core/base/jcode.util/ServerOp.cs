using System.Collections.Generic;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.09.05
    /// 描 述：服务配置
    /// </summary>
    public class ServerOp
    {
        /// <summary>
        /// 软件名称
        /// </summary>
        public string SoftName { get; set; }
        /// <summary>
        /// 软件版本号
        /// </summary>
        public string Version { get; set; }
        
        /// <summary>
        /// 数据库连接
        /// </summary>
        public string dbConn { get; set; }
        /// <summary>
        /// 数据库类型 SqlServer,Oracle,MySql
        /// </summary>
        public string dbType { get; set; }
        /// <summary>
        /// 是否属于开发环境
        /// </summary>
        public bool dbIsDev { get; set; }

        /// <summary>
        /// 缓存前缀（redis缓存有效果）
        /// </summary>
        public string RedisPrev { get; set; }
        /// <summary>
        /// 缓存地址
        /// </summary>
        public string RedisExchangeHosts { get; set; }
        /// <summary>
        /// Redis缓存DbNum
        /// </summary>
        public int RedisDbNum { get; set; }
        /// <summary>
        /// 缓存类型（默认MemoryCache）Redis MemoryCache
        /// </summary>
        public string CacheType { get; set; }


        /// <summary>
        /// 服务目录
        /// </summary>
        public string RoutePrefix { get; set; }

        /// <summary>
        /// 皮肤配置
        /// </summary>
        public string UItheme { get; set; }

        /// <summary>
        /// IM地址
        /// </summary>
        public string IMUrl { get; set; }
        /// <summary>
        /// 即时服务是否打开
        /// </summary>
        public bool IMOpen { get; set; }


        #region 邮箱设置
        /// <summary>
        /// 邮箱服务类型 exchange,其它; 默认其它
        /// </summary>
        public string MainServerType { get; set; }

        /// <summary>
        /// Exchange 服务版本 默认 Exchange2016
        /// </summary>
        public string ExchangeVer { get; set; }

        /// <summary>
        /// 发出邮箱设置邮箱主机/Exchange服务器上接口地址
        /// </summary>
        public string MailHost { get; set; }
        /// <summary>
        /// 发出邮箱设置邮箱主机端口
        /// </summary>
        public int MailPort { get; set; }
        /// <summary>
        /// 发出邮箱的名称
        /// </summary>
        public string MailName { get; set; }
        /// <summary>
        /// 发出邮箱的地址
        /// </summary>
        public string MailUserName { get; set; }
        /// <summary>
        /// 发出邮箱的密码
        /// </summary>
        public string MailPassword { get; set; }


        #endregion



        /// <summary>
        /// 企业号ID
        /// </summary>
        public string CorpId { get; set; }
        /// <summary>
        /// 企业自建应用Secret
        /// </summary>
        public string CorpSecret { get; set; }
        /// <summary>
        /// 企业应用的id，整型。可在应用的设置页面查看
        /// </summary>
        public string CorpAppId { get; set; }
        /// <summary>
        /// 企业通讯录接口调用Secret
        /// </summary>
        public string CorpContactsApiSecret { get; set; }
        /// <summary>
        /// 百度编辑器图片地址
        /// </summary>
        public string UeditorImg { get; set; }

        /// <summary>
        /// JWT认证密钥
        /// </summary>
        public string JwtSecret { get; set; }
        /// <summary>
        /// JWT认证过期时间（刷新令牌/会话的绝对有效期，小时）
        /// </summary>
        public int JwtExp { get; set; }
        /// <summary>
        /// 访问令牌有效期（分钟），未配置默认 60
        /// </summary>
        public int JwtAccessExpMinutes { get; set; }
        /// <summary>
        /// 刷新令牌轮换宽限期（秒），未配置默认 60
        /// </summary>
        public int JwtRefreshGraceSeconds { get; set; }
        /// <summary>
        /// 刷新令牌共享模式：多人共用一个账号、或同一份登录态被多个客户端持有时开启。
        /// 开启后刷新不再轮换 jti（刷新幂等，只换访问令牌 + 滑动会话），
        /// 避免先刷新的一方把会话表里的 jti 改掉、其他人的刷新令牌立刻失效甚至整条会话被吊销。
        /// 关闭 = 严格轮换（可用 jti 变化检测重放，但要求每个客户端各自登录、持有独立 sid）。
        /// </summary>
        public bool JwtRefreshSharedMode { get; set; }
        /// <summary>
        /// 共享模式下会话自创建起的最长存活时间（小时），给滑动续期加绝对上限，
        /// 防止刷新令牌一旦泄露即可无限续期。0 表示不额外限制。
        /// </summary>
        public int JwtRefreshMaxHours { get; set; }

        /// <summary>
        /// 文件存放地址
        /// </summary>
        public string FilePath { get; set; }
        
        /// <summary>
        /// 葡萄城AR报表服务地址
        /// </summary>
        public string AReportsUrl { get; set; }
        /// <summary>
        /// 葡萄城AR报表目录
        /// </summary>
        public string AReportsPath { get; set; }

        /// <summary>
        /// 是否开启IsSwagger，开发环境强制开启，生成环境根据设置值来做判断
        /// </summary>
        public bool IsSwagger { get; set; }

        /// <summary>
        /// 短信类型,暂时只支持阿里云短信
        /// </summary>
        public string SmsType { get; set; }


        /// <summary>
        /// 腾讯云用于标识 API 调用者身份，可以简单类比为用户名。
        /// </summary>
        public string TencentSecretId { get; set; }
        /// <summary>
        /// 腾讯云用于验证 API 调用者的身份，可以简单类比为密码。
        /// </summary>
        public string TencentSecretKey { get; set; }

        /// <summary>
        /// 腾讯云 短信key。
        /// </summary>
        public string TencentSmsAccessKeyId { get; set; }

        /// <summary>
        /// 短信密钥(阿里)
        /// </summary>
        public string SmsSecret { get; set; }
        /// <summary>
        /// 短信key(阿里)
        /// </summary>
        public string SmsAccessKeyId { get; set; }

        /// <summary>
        /// 短信签名名称(腾讯阿里共用)
        /// </summary>
        public string SmsSignName { get; set; }

        /// <summary>
        /// 微信小程序 AppId
        /// </summary>
        public string WeixinAppId { get; set; }
        /// <summary>
        /// 微信小程序 AppSecret
        /// </summary>
        public string WeixinAppSecret { get; set; }


        /// <summary>
        /// 是否开启Sass多租户模式
        /// </summary>
        public bool IsSass { get; set; }
        /// <summary>
        /// sass模式是否支持表字段隔离
        /// </summary>
        public bool IsSassTable { get; set; }



        /// <summary>
        /// 附件上传方式（默认Local）Db,AliYunOss,Local,TencentOss,HuaWeiOss,MinIoOss【阿里云Oss存储，系统附件上传,腾讯云Oss存储,华为云Oss存储,MinIo存储】
        /// </summary>
        public string AnnexesType { get; set; }
        /// <summary>
        /// 孤儿附件保留天数，超过该天数且业务表无引用的附件会被定时清理，未配置默认7天
        /// </summary>
        public int AnnexesOrphanDays { get; set; }
        /// <summary>
        /// 暂存附件保留时长（小时），表单关闭后未转正的暂存附件超过该时长会被定时回收，未配置默认24小时
        /// </summary>
        public int AnnexesDraftHours { get; set; }
        /// <summary>
        /// OSSBucket 存储空间名称
        /// </summary>
        public string OssBucketName { get; set; }
        /// <summary>
        /// 图片是否存数据库
        /// </summary>
        public bool IsImgSaveDb { get; set; }
        /// <summary>
        /// 开启ORM文件日志记录
        /// </summary>
        public bool IsEnableDiffLog { get; set; }

        #region 阿里云Oss对象存储配置----ADD BY SSY 20220822
        /// <summary>
        /// OSS存储区域
        /// </summary>
        public string OssRegionId { get; set; }
        /// <summary>
        /// 持续时间/秒
        /// </summary>
        public int? OssDurationSeconds { get; set; }
        /// <summary>
        /// OssAccessKeyId
        /// </summary>
        public string OssAccessKeyId { get; set; }
        /// <summary>
        /// OssAccessKeySecret
        /// </summary>
        public string OssAccessKeySecret { get; set; }
        /// <summary>
        /// 存储节点
        /// </summary>
        public string OssEndpoint { get; set; }
        /// <summary>
        /// Bucket 授权策略
        /// </summary>
        public string OssPolicyResource { get; set; }
        /// <summary>
        /// 角色ARN
        /// </summary>
        public string OssRoleArn { get; set; }
        /// <summary>
        /// 地域节点
        /// </summary>
        public string OssDf_endpoint { get; set; }
        #endregion

        #region 腾讯云Oss对象存储----ADD BY SSY 20221020
        /// <summary>
        /// 每次请求签名有效时长，单位为秒
        /// </summary>
        public int TencentOssDurationSeconds { get; set; }
        /// <summary>
        /// 设置腾讯云账户的账户标识 APPID
        /// </summary>
        public string TencentAppid { get; set; }
        /// <summary>
        /// 设置一个默认的存储桶地域
        /// </summary>
        public string TencentOssRegionId { get; set; }
        #endregion

        #region 华为云Oss对象存储----ADD BY SSY 20221024
        /// <summary>
        /// Access Key ID，接入键标识，用户在对象存储服务系统中的接入键标识，一个接入键标识唯一对应一个用户，
        /// 一个用户可以同时拥有多个接入键标识。对象存储服务系统通过接入键标识识别访问系统的用户。
        /// </summary>
        public string HuaWeiOssAK { get; set; }
        /// <summary>
        /// Secret Access Key，安全接入键，用户在对象存储服务系统中的安全接入键，是用户访问对象存储服务系统的密钥，
        /// 用户根据安全接入键和请求头域生成鉴权信息。安全接入键和接入键标识一一对应。
        /// </summary>
        public string HuaWeiOssSK { get; set; }
        /// <summary>
        /// OBS当前开通的服务地址和区域信息
        /// </summary>
        public string HuaWeiOssEndpoint { get; set; }
        #endregion

        #region MinIo对象存储----ADD BY SSY 20221031
        /// <summary>
        /// 是一个URL，域名，IPv4或者IPv6地址。以下是合法的endpoints:
        /// </summary>
        public string MinIoOssEndpoint { get; set; }
        /// <summary>
        /// 类似于用户ID，用于唯一标识你的账户。可选，为空代表匿名访问。
        /// </summary>
        public string MinIoOssAccessKey { get; set; }
        /// <summary>
        /// 是你账户的密码。可选，为空代表匿名访问。
        /// </summary>
        public string MinIoOssSecretKey { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MinIoOssRegion { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public bool MinIoIsSSL { get; set; }
        
        #endregion

        #region 七牛对象存储 
        /// <summary>
        /// 是一个URL，域名，IPv4或者IPv6地址。以下是合法的endpoints:
        /// </summary>
        public string QiniuOssDomain { get; set; }
        
        /// <summary>
        /// 七牛上传区域 华东ZONE_CN_East/华北ZONE_CN_North/华南ZONE_CN_South/北美ZONE_US_North/东南亚ZONE_AS_Singapore
        /// </summary>
        public string QiniuZone{ get; set; }
        /// <summary>
        /// 类似于用户ID，用于唯一标识你的账户。可选，为空代表匿名访问。
        /// </summary>
        public string QiniuOssAccessKey { get; set; }
        /// <summary>
        /// 是你账户的密码。可选，为空代表匿名访问。
        /// </summary>
        public string QiniuOssSecretKey { get; set; }
        #endregion
        
        #region 文件预览，用到的转化工具 Office 地址 ----ADD BY Tobin 20221203
        /// <summary>
        /// office转化工具地址
        /// </summary>
        public string OfficeToolPath { get; set; }
        /// <summary>
        /// 是否加密文件
        /// </summary>
        public bool IsEncryptFile { get; set; }
        /// <summary>
        /// 文件预览配置，未配置时全部走默认值
        /// </summary>
        public PreviewOptions Preview { get; set; } = new PreviewOptions();
        #endregion

        #region 跨库查询配置----ADD BY SSY 20230221
        /// <summary>
        /// 是否开启跨库查询配置
        /// </summary>
        public bool IsCrossDb { get; set; }
        /// <summary>
        /// 跨库查询配置-多数据库配置
        /// </summary>
        public List<CrossDbInfo> CrossDbInfo { get; set; }
        #endregion

        #region 开启主从库配置----ADD BY SSY 20230222
        /// <summary>
        /// 是否开启读写分离
        /// </summary>
        public bool IsSlave { get; set; }
        /// <summary>
        /// 从库数据库连接字符串
        /// </summary>
        public string DbConnSlave { get; set; }
        #endregion

        #region 第三方对接配置----ADD BY SSY 20230315 by tobin 202303150843
        /// <summary>
        /// 生成token的key （后续版本弃用）
        /// </summary>
        public string OtherTokenKey { get; set; }
        /// <summary>
        /// 第三方接口接口调用地址 （后续版本弃用）
        /// </summary>
        public string OtherApi { get; set; }
        /// <summary>
        /// 第三方流程接口地址（后续版本弃用）
        /// </summary>
        public string OtherWorkFlowApi { get; set; }
        /// <summary>
        /// 第三方系统的id，事先约定好 （后续版本弃用）
        /// </summary>
        public string OtherTokenAppId { get; set; }
        #endregion
        
        /// <summary>
        /// 密级设定开启 by tobin
        /// </summary>
        public bool IsSecretLevel{ get; set; }
        /// <summary>
        /// 开启流程实例自增码 by tobin（后续版本弃用）
        /// </summary>
        public bool IsWFIndex { get; set; }
        /// <summary>
        /// 开启sso单点登录 by ssy（后续版本弃用）
        /// </summary>
        public bool IsSSoLogin { get; set; }

        /// <summary>
        /// PostgreSQL数据库是否强制转小写，默认true
        /// </summary>
        public bool PgSqlIsAutoToLower { get; set; }
        /// <summary>
        /// 开启高级查询（弃用）
        /// </summary>
        public bool IsUpgradeQuery { get; set; }
        
        
        /// <summary>
        /// 日志保存天数,0 表示一直保存
        /// </summary>
        public int SaveLogDays { get; set; }

        /// <summary>
        /// 接口是否校验用户是否合法
        /// </summary>
        public bool IsApiCheckUserAuth { get; set; }

        #region 阿里云OCR文字识别
        /// <summary>
        /// OcrAccessKeyId
        /// </summary>
        public string OcrAccessKeyId { get; set; }
        /// <summary>
        /// OcrAccessKeySecret
        /// </summary>
        public string OcrAccessKeySecret { get; set; }
        /// <summary>
        /// OcrEndpoint
        /// </summary>
        public string OcrEndpoint { get; set; }
        #endregion
        
        /// <summary>
        /// 获取IP地址名关闭
        /// </summary>
        public bool GetIpNameClosed{ get; set; }


        /// <summary>
        /// 应用编码
        /// </summary>
        public string AppCode { get; set; }
        
        /// <summary>
        /// 开启应用编码过滤
        /// </summary>
        public bool OpenAppCodeFilter { get; set; }
        
        /// <summary>
        /// 接口数据是否加密
        /// </summary>
        public bool ApiDataAes { get; set; }


    }

    /// <summary>
    /// 文件预览配置
    /// </summary>
    public class PreviewOptions
    {
        /// <summary>
        /// 是否启用 NPOI 直接渲染 Excel（xls/xlsx），失败时自动回退 LibreOffice
        /// </summary>
        public bool EnableNpoi { get; set; } = true;
        /// <summary>
        /// 是否启用 LibreOffice 服务端转换，未安装时相关格式直接提示下载
        /// </summary>
        public bool EnableLibreOffice { get; set; } = true;
        /// <summary>
        /// pptx 是否优先由前端 pptx-preview 渲染（关闭后 pptx 走 LibreOffice）
        /// </summary>
        public bool EnableClientPptx { get; set; } = true;
        /// <summary>
        /// LibreOffice 单次转换超时（秒）
        /// </summary>
        public int OfficeConvertTimeoutSeconds { get; set; } = 60;
        /// <summary>
        /// NPOI 渲染最多导出的行数
        /// </summary>
        public int MaxRows { get; set; } = 200;
        /// <summary>
        /// NPOI 渲染最多导出的列数
        /// </summary>
        public int MaxCols { get; set; } = 50;
        /// <summary>
        /// 超过该大小的文件不再交给 NPOI 渲染（单位 MB）
        /// </summary>
        public int MaxFileSizeMb { get; set; } = 20;
    }

    /// <summary>
    /// 跨库查询配置-多数据库配置
    /// </summary>
    public class CrossDbInfo
    {
        /// <summary>
        /// 访问的数据库别名
        /// </summary>
        public string CrossDbConfigId { get; set; }
        /// <summary>
        /// 数据库类型
        /// </summary>
        public string CrossDbType { get; set; }
        /// <summary>
        /// 数据库连接字符串
        /// </summary>
        public string ConnectionStr { get; set; }
    }
}
