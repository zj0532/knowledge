using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Mail;
using System.Net.NetworkInformation;

namespace RtCMCollector
{
    public class qqMail
    {
        ///   <summary> 
        ///   发送邮件 
        ///   </summary> 
        ///   <param   name= "strSmtpServer "> smtp地址 </param> 
        ///   <param   name= "Password "> 密码 </param> 
        ///   <param   name= "strFrom "> 发信人地址 </param> 
        ///   <param   name= "strto "> 收信人地址 </param> 
        ///   <param   name= "strSubject "> 邮件标题 </param> 
        ///   <param   name= "strBody "> 邮件正文 </param> 
        public static void SendMail(string strSmtpServer, string Password, string strFrom, string strto, string strSubject, string strBody)
        {
            //log4net.ILog logger = RtCMCollector.Core.LogUtils.getClientLogger();//日志对象
            #region 方法一
            ////生成一个   使用SMTP发送邮件的客户端对象 
            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient(strSmtpServer);

            ////表示以当前登录用户的默认凭据进行身份验证 
            //client.UseDefaultCredentials = true;
            ////包含用户名和密码 
            //string name = strFrom.Substring(0, strFrom.IndexOf('@'));
            //client.Credentials = new System.Net.NetworkCredential(name, Password);

            ////指定如何发送电子邮件。 
            ////Network                                             电子邮件通过网络发送到   SMTP   服务器。     
            ////PickupDirectoryFromIis               将电子邮件复制到挑选目录，然后通过本地   Internet   信息服务   (IIS)   传送。     
            ////SpecifiedPickupDirectory           将电子邮件复制到   SmtpClient.PickupDirectoryLocation   属性指定的目录，然后由外部应用程序传送。     

            //client.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;

            ////建立邮件对象   
            //System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage(strFrom, strto, strSubject, strBody);

            ////定义邮件正文，主题的编码方式 
            //message.BodyEncoding = System.Text.Encoding.UTF8;
            //message.SubjectEncoding = System.Text.Encoding.UTF8;

            ////获取或设置一个值，该值指示电子邮件正文是否为   HTML。   
            //message.IsBodyHtml = true;

            ////指定邮件优先级 

            //message.Priority = System.Net.Mail.MailPriority.Normal;

            ////增加附件 
            //////System.Web.Mail.MailAttachment   mailAttachment=new   System.Web.Mail.MailAttachment(@ "f:/baihe.txt ");   
            ////if (strFileName != " " && strFileName != null)
            ////{
            ////    Attachment data = new Attachment(strFileName);
            ////    message.Attachments.Add(data);
            ////}


            ////发件人身份验证,否则163   发不了 
            //client.Credentials = new System.Net.NetworkCredential(strFrom, Password);

            ////发送 
            //client.Send(message);
            #endregion
            #region 方法二
            MailMessage objMailMessage = new MailMessage();
            //string fromAddress = ConfigurationManager.AppSettings["FromAddress"];//你在web.config中配置的发件人地址，就是你的邮箱地址。           
            //string mailHost = ConfigurationManager.AppSettings["MailHost"];//邮件服务器，如mail.qq.com           
            objMailMessage.From = new MailAddress(strFrom);//发送方地址           
            objMailMessage.To.Add(new MailAddress(strto));//收信人地址           
            objMailMessage.BodyEncoding = System.Text.Encoding.UTF8;//邮件编码           
            objMailMessage.Subject = strSubject;//邮件标题           
            objMailMessage.Body = strBody;//邮件内容           
            objMailMessage.IsBodyHtml = true;//邮件正文是否为html格式           
            SmtpClient objSmtpClient = new SmtpClient();
            objSmtpClient.Host = strSmtpServer;//邮件服务器地址           
            objSmtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;//通过网络发送到stmp邮件服务器
            string name = strFrom.Substring(0, strFrom.IndexOf('@'));
            objSmtpClient.Credentials = new System.Net.NetworkCredential(name, Password);//发送方的邮件地址，密码  
            try
            {
                objSmtpClient.Send(objMailMessage);
            }
            catch (Exception ex)
            {
                //logger.Error("发送邮件时出错", ex);         
            }
            #endregion
        }
    }
}
