using Microsoft.Exchange.WebServices.Data;
using System;
using System.Collections.Generic;
using System.Net;
using Task = System.Threading.Tasks.Task;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：ssy
    /// 日 期：2022.11.09
    /// 描 述：ExChange协议发送邮件
    /// </summary>
    public class ExChangeHelper
    {
        private readonly ExchangeService service;
        private static readonly object _lock = new object();
        private static ExChangeHelper _instance = null;


        /// <summary>
        /// 实例
        /// </summary>
        /// <returns></returns>
        public static ExChangeHelper GetInStance() {
            if (_instance == null) {
                lock (_lock) {
                    if (_instance == null)
                    {
                        _instance = new ExChangeHelper();
                    }
                }
            }

            return _instance;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <returns></returns>
        public ExChangeHelper()
        {
            var setting = ConfigHelper.GetConfig(); //获取配置
            string MailServer = setting.MailHost;//发出邮箱设置邮箱主机【域，不是域名，域用户的的话就得填这个，选填】
            string MailUserName = setting.MailUserName;//登录username，邮箱登录ID或者域登录ID
            string MailPassword = setting.MailPassword;//登录username的密码
            //string MailDiscoverUrl = setting.MailDiscoverUrl;//发件人的邮箱地址，完整的(xx@dd.com)
            //string MailApiUrl = setting.MailApiUrl;//Exchange服务器上接口地址,如http://XXX/ews/Exchange.asmx

            ExchangeVersion vType = ExchangeVersion.Exchange2016;

            switch (setting.ExchangeVer) {
                case "Exchange2007_SP1":
                    vType = ExchangeVersion.Exchange2007_SP1;
                    break;
                case "Exchange2010":
                    vType = ExchangeVersion.Exchange2010;
                    break;
                case "Exchange2010_SP1":
                    vType = ExchangeVersion.Exchange2010_SP1;
                    break;
                case "Exchange2010_SP2":
                    vType = ExchangeVersion.Exchange2010_SP2;
                    break;
                case "Exchange2013":
                    vType = ExchangeVersion.Exchange2013;
                    break;
                case "Exchange2013_SP1":
                    vType = ExchangeVersion.Exchange2013_SP1;
                    break;
                case "Exchange2015":
                    vType = ExchangeVersion.Exchange2015;
                    break;
                case "Exchange2016":
                    vType = ExchangeVersion.Exchange2016;
                    break;
                case "V2015_10_05":
                    vType = ExchangeVersion.V2015_10_05;
                    break;
            }

            service = new ExchangeService(vType);//根据自己邮件服务器选择对应版本                                                       
            // 获取身份验证, 能够尝试你的邮箱名, 域用户ID等
            service.Credentials = new NetworkCredential(MailUserName, MailPassword);
            //service.TraceEnabled = true;//是否启用跟踪
            //service.AutodiscoverUrl(MailUserName, RedirectionCallback);

            service.Url = new Uri(MailServer);//Exchange服务器上接口地址,如http://XXX/ews/Exchange.asmx
        }




        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人邮箱地址</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        public async Task Send(string to, string subject, string body)
        {
            //初始化
            EmailMessage message = new EmailMessage(service);
            //使用自定义发件箱
            message.From = new EmailAddress(ConfigHelper.GetConfig().MailUserName, ConfigHelper.GetConfig().MailName);
            message.Subject = subject;// 邮件主题
            message.Body = new MessageBody//邮件内容
            {
                BodyType = BodyType.HTML,
                Text = body//邮件主体
            };
            List<EmailAddress> ToList = new List<EmailAddress>();//发送人列表
            ToList.Add(new EmailAddress(to));
            message.ToRecipients.AddRange(ToList);
            //await message.SendAndSaveCopy();//发送后保存到已发送邮件列表
            //await message.save();// 保存草稿
            await message.Send();// 只发送不保存邮件
        }
    }
}
