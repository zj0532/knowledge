using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Net.Mail;

namespace Pactera.Common.EMail
{
    public class EMailSend
    {
        /// <summary>
        /// 邮件发送功能
        /// </summary>
        /// <param name="IsOutlook">是否启用邮件发送功能</param>
        /// <param name="IsTestEmail">是否测试账号</param>
        /// <param name="MailServer">邮件服务器</param>
        /// <param name="MailAccount">邮件发送者账号</param>
        /// <param name="MailPassword">邮件发送者账号密码</param>
        /// <param name="Mail_suffix">邮件后缀</param>
        /// <param name="m_strRecipientMail">收件人地址如: P000000,请勿加入@GNPJVC.COM.CN, 多个帐号之间请使用 , 或 ; 号隔开</param>
        /// <param name="m_strCC">抄送人地址如: pcitzs,请勿加入@GNPJVC.COM.CN, 多个帐号之间请使用 , 或 ; 号隔开</param>
        /// <param name="m_strarrayMailAttachmentList">附件完整地址</param>
        /// <param name="m_strMailSubject">主题</param>
        /// <param name="m_strMailBody">内容</param>
        /// <returns></returns>
        public static bool SendMail(bool IsOutlook, bool IsTestEmail, string MailServer, string MailAccount, string MailPassword, 
            string Mail_suffix, string m_strRecipientMail, string m_strCC, string m_strarrayMailAttachmentList, 
            string m_strMailSubject, string m_strMailBody)
        {
            if (!IsOutlook)
            {
                return true;
            }

            string m_strMailServerName = MailServer;  //邮件服务器
            string adSuffix = Mail_suffix;            //邮件后缀,如： "@gnpjvc.com.cn";
            string m_strSenderMail = MailAccount + adSuffix;  //邮件发送地址：邮件账号+邮件后缀
            string m_lMailUsername = MailAccount;    //邮件发送者账号
            string m_lMailUserPassword = MailPassword;  //邮件发送者账号密码

            if (IsTestEmail)  //启用邮件发送功能,发总一封测试邮件
            {
                m_strMailSubject = "[测试邮件]" + m_strMailSubject;
                m_strMailBody = "这是一封测试邮件，抱歉占用您宝贵的时间，此邮件可直接删除，谢谢。<br>" + m_strMailBody;
            }
            //接收者列表
            string[] ArrsToUser, ArrsCCUser;
            ArrsToUser = m_strRecipientMail.Replace(';', ',').Split(',');
            m_strRecipientMail = "";
            for (int i = 0; i < ArrsToUser.Length; i++)
            {
                if (i == ArrsToUser.Length-1)
                {
                    m_strRecipientMail += ArrsToUser[i] + adSuffix;
                }
                else
                {
                    m_strRecipientMail += ArrsToUser[i] + adSuffix + ",";
                }
            }
            //抄送人列表
            ArrsCCUser = m_strCC.Replace(';', ',').Split(',');
            m_strCC = "";
            for (int i = 0; i < ArrsCCUser.Length; i++)
            {
                if (ArrsCCUser[i] != "")
                {
                    if (i == ArrsCCUser.Length - 1)
                    {
                        m_strCC += ArrsCCUser[i] + adSuffix;
                    }
                    else
                    {
                        m_strCC += ArrsCCUser[i] + adSuffix + ",";
                    }
                }
            }

            System.Net.Mail.MailMessage aMessage;
            System.Net.Mail.SmtpClient Client = new SmtpClient();
            if (m_strSenderMail == "" || m_strSenderMail == null)
            {
                return false;
            }
            if (m_strRecipientMail == "" || m_strRecipientMail == null)
            {
                return false;
            }

            //Create a new message
            try
            {
                aMessage = new MailMessage(m_strSenderMail, m_strRecipientMail);
                if (m_strCC != "")
                {
                    aMessage.CC.Add(m_strCC);     //抄送人
                }
                if (m_strMailSubject != null) 
                    aMessage.Subject = m_strMailSubject;   //主题
                aMessage.BodyEncoding = System.Text.Encoding.UTF8;   //正文编码

                aMessage.IsBodyHtml = true;

                if (m_strMailBody != null)     
                    aMessage.Body = m_strMailBody;    //正文

                //add the attachment
                if (m_strarrayMailAttachmentList != "")
                {
                    aMessage.Attachments.Add(new System.Net.Mail.Attachment(m_strarrayMailAttachmentList));
                }

                //create a smtpMail

                Client.Host = m_strMailServerName;  //发送服务器

                Client.UseDefaultCredentials = false;
                Client.Credentials = new System.Net.NetworkCredential(m_lMailUsername, m_lMailUserPassword, "domain");

            }

            catch (Exception err)
            {
                return false;
            }

            // send email
            try
            {
                Client.Send(aMessage);
                return true;
            }
            catch (Exception err)
            {
                return false;
            }


        }

    }
}
