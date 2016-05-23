using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Web;

namespace WeBind.Helper
{
    public class EmailHelper
    {
        public void SendMail(EmailViewModel email, bool IsNewsLetter = false)
        {
            try
            {
                string mailFromName, emailID, password, host, bcc;
                int port;
                bool enableSSL = true;

                if (IsNewsLetter)
                {
                    mailFromName = System.Configuration.ConfigurationManager.AppSettings["newsletter_mailFromName"];
                    emailID = System.Configuration.ConfigurationManager.AppSettings["newsletter_emailID"];
                    password = System.Configuration.ConfigurationManager.AppSettings["newsletter_password"];
                    host = System.Configuration.ConfigurationManager.AppSettings["newsletter_host"];
                    bcc = System.Configuration.ConfigurationManager.AppSettings["newsletter_bcc"];
                    port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["newsletter_port"].ToString());
                    enableSSL = true;
                }
                else
                {
                    mailFromName = System.Configuration.ConfigurationManager.AppSettings["mailFromName"];
                    emailID = System.Configuration.ConfigurationManager.AppSettings["emailID"];
                    password = System.Configuration.ConfigurationManager.AppSettings["password"];
                    host = System.Configuration.ConfigurationManager.AppSettings["host"];
                    bcc = System.Configuration.ConfigurationManager.AppSettings["bcc"];
                    port = int.Parse(System.Configuration.ConfigurationManager.AppSettings["port"].ToString());
                }
                var msg = new MailMessage();
                foreach (var mailId in email.MailToList)
                {
                    msg.To.Add(mailId.Trim());
                }
                foreach (var mailId in email.MailBCC)
                {
                    msg.Bcc.Add(mailId.Trim());
                }
                //msg.CC.Add("rushil27shah@gmail.com");
                msg.From = new MailAddress(emailID, mailFromName);
                msg.Bcc.Add(bcc);
                msg.Subject = email.MailSubject;
                msg.Body = email.MailBody;
                msg.IsBodyHtml = true;
                using (var smtp = new SmtpClient())
                {
                    smtp.Port = port;
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.UseDefaultCredentials = false;
                    smtp.Host = host;
                    smtp.EnableSsl = enableSSL;
                    smtp.Credentials = new System.Net.NetworkCredential(emailID, password);
                    smtp.Send(msg);
                }
            }
            catch (Exception ex)
            {
                //throw;
            }
        }


    }
    public class EmailViewModel
    {
        public List<string> MailToList { get; set; }
        public string MailFrom { get; set; }
        public List<string> MailCC { get; set; }
        public List<string> MailBCC { get; set; }
        public string MailSubject { get; set; }
        public string MailBody { get; set; }
    }
}