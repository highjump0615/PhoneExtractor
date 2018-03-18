using log4net;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Forensics.Util
{
    public class SendEmail
    {
        public String loginName = "suyang@ecryan.com.cn";   //邮箱
        public String loginPass = "su780531";     //邮箱密码
        public const int port = 25;
        public String smtp = "smtp.263xmail.com";      //服务器地址
        public Encoding encoding = Encoding.UTF8;
        ILog log = LogManager.GetLogger(typeof(SendEmail));

        public string Send(ArrayList toMail, string subject, string body, bool isHtml)
        {
            string flag = "";
            try
            {
                MailMessage message = new MailMessage();
                message.From = new MailAddress(loginName, smtp);//必须是提供smtp服务的邮件服务器
                message.Subject = subject;
                message.IsBodyHtml = isHtml;
                message.BodyEncoding = encoding;
                message.Body = body;

                message.To.Add(loginName);
                if (toMail != null)
                {
                    for (int x = 0; x < toMail.Count; x++)
                    {
                        if (toMail[x].ToString().Trim().Length > 0)
                        {
                            message.Bcc.Add(new MailAddress(toMail[x].ToString().Trim()));
                        }
                    }
                }
                message.Priority = MailPriority.High;//优先级

                //smtp客户端
                SmtpClient client = new SmtpClient(smtp, port);
                client.Credentials = new System.Net.NetworkCredential(loginName, loginPass);//验证用户名和密码
                client.EnableSsl = false; //必须经过ssl加密

                client.Send(message);//发送邮件

                flag = "OK";
            }
            catch (Exception ex)
            {
                log.Info(ex.Message, ex);
                flag = "发送邮件失败,原因:" + ex.ToString();
            }
            return flag;
        }
    }
}
