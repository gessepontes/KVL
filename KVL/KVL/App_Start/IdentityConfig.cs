using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using System.Net.Mail;

namespace LigaWeb
{
    public class EmailService : IIdentityMessageService
    {
        public async Task SendAsync(IdentityMessage message)
        {
            await configSMTPasync(message);
        }

        // send email via smtp service
        private async Task configSMTPasync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            var credentialUserName = "postmaster@societypro.com.br";
            var sentFrom = message.Destination;
            var pwd = "123456q!";

            // Configure the client:
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();

            //System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            client.Port = 25;
            client.Host = "mail.societypro.com.br";
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;

            // Creatte the credentials:
            System.Net.NetworkCredential credentials = new System.Net.NetworkCredential(credentialUserName, pwd);
            client.EnableSsl = false;
            client.Credentials = credentials;

            // Create the message:
            var mail = new MailMessage(sentFrom, message.Destination);
            mail.From = new MailAddress(credentialUserName);
            mail.Subject = message.Subject;
            mail.Body = message.Body;
            mail.IsBodyHtml = true;

            await client.SendMailAsync(mail);
        }
    }
}
