using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Configuration;
using System;
using System.Net.Mail;

namespace LigaWeb
{
    public class EmailService : IIdentityMessageService
    {
        public object ConfigurationManager { get; private set; }

        public async Task SendAsync(IdentityMessage message)
        {
            await configSMTPasync(message);
        }

        // send email via smtp service
        private async Task configSMTPasync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            var credentialUserName = ReadSetting("KVL.Portal.Email.User");
            var sentFrom = message.Destination;
            var pwd = ReadSetting("KVL.Portal.Email.Password");

            // Configure the client:
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Port = Convert.ToInt16(ReadSetting("KVL.Portal.Email.Port"));
            client.Host = ReadSetting("KVL.Portal.Email.Host");
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = Convert.ToBoolean(ReadSetting("KVL.Portal.Email.UseDefaultCredentials"));

            // Creatte the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
            client.EnableSsl = Convert.ToBoolean(ReadSetting("KVL.Portal.Email.HttpsEnabled"));
            client.Credentials = credentials;

            // Create the message:
            var mail = new MailMessage(sentFrom, message.Destination);
            mail.From = new MailAddress(credentialUserName);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = Convert.ToBoolean(ReadSetting("KVL.Portal.Email.IsBodyHtml"));

            await client.SendMailAsync(mail);
        }

        private string ReadSetting(string key)
        {
            string result = "";

            try
            {
                result = System.Configuration.ConfigurationManager.AppSettings[key].ToString();
            }
            catch (ConfigurationErrorsException)
            {
            }

            return result;

        }
    }
}
