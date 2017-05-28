using System.Configuration;
using System.Net.Mail;
using System.Threading.Tasks;

namespace DSW.Utils
{
    public static class EmailService
    {
        public static void SendMail(string from, string to, string subject, string body)
        {
            var sentFrom = ConfigurationManager.AppSettings["SystemEmailSender"];

            var mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient client = new SmtpClient();
            client.Send(mail);
        }

        public static async Task SendMailAsync(string from, string to, string subject, string body)
        {
            var sentFrom = ConfigurationManager.AppSettings["SystemEmailSender"];

            var mail = new MailMessage(from, to);
            mail.Subject = subject;
            mail.Body = body;

            SmtpClient client = new SmtpClient();
            await client.SendMailAsync(mail);
        }
    }
}
