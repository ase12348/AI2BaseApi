using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace jcode.util
{
    /// <summary>
    /// 版 本 极简搭AI应用开发基座
    /// Copyright (c) 2019-present 苏州极简搭信息技术有限公司
    /// 创建人：tobin
    /// 日 期：2019.11.05
    /// 描 述：发送邮件
    /// </summary>
    public static class MailHelper
    {
        /// <summary>
        /// 邮件服务器地址
        /// </summary>
        private static readonly string MailServer = ConfigHelper.GetConfig().MailHost;
        /// <summary>
        /// 用户名
        /// </summary>
        private static readonly string MailUserName = ConfigHelper.GetConfig().MailUserName;
        /// <summary>
        /// 密码
        /// </summary>
        private static readonly string MailPassword = ConfigHelper.GetConfig().MailPassword;
        /// <summary>
        /// 名称
        /// </summary>
        private static readonly string MailName = ConfigHelper.GetConfig().MailName;
        
        

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to">收件人邮箱地址</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <returns>是否成功</returns>
        public static async Task Send(string to, string subject, string body)
        {
            if (ConfigHelper.GetConfig().MainServerType == "exchange")
            {
                await ExChangeHelper.GetInStance().Send(to, subject, body);
            }
            else {
                MailMessage mailMsg = new MailMessage
                {
                    From = new MailAddress(MailUserName, MailName) //源邮件地址和发件人
                };//实例化对象
                mailMsg.To.Add(new MailAddress(to));//收件人地址

                mailMsg.SubjectEncoding = Encoding.GetEncoding("UTF-8");
                mailMsg.Subject = subject;
                mailMsg.IsBodyHtml = true;
                mailMsg.Body = body;

                //指定smtp服务地址（根据发件人邮箱指定对应SMTP服务器地址）
                SmtpClient smtpclient = new SmtpClient(MailServer, ConfigHelper.GetConfig().MailPort)
                {
                    //加密
                    EnableSsl = true,
                    //通过用户名和密码验证发件人身份
                    Credentials = new NetworkCredential(MailUserName, MailPassword)
                };

                await smtpclient.SendMailAsync(mailMsg);
            }


            
        }
    }
}
