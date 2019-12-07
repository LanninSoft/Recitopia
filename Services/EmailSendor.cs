using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Recitopia.Services
{
    public class EmailSender : IEmailSender
    {

        // Our private configuration variables
        private string PrimaryDomain;
        private int PrimaryPort;
        private string UsernameEmail;
        private string UsernamePassword;
        private string FromEmail;
        private string ToEmail;
        private string CcEmail;
        private bool EnableSsl;
       

        // Get our parameterized configuration
        public EmailSender(string primaryDomain, int primaryPort, string userEmail, string userPassword, string fromEmail, string toEmail, string ccEmail, bool enableSsl)
        {
            this.PrimaryDomain = primaryDomain;
            this.PrimaryPort = primaryPort;
            this.UsernameEmail = userEmail;
            this.UsernamePassword = userPassword;
            this.FromEmail = fromEmail;
            this.ToEmail = toEmail;
            this.CcEmail = ccEmail;
            this.EnableSsl = enableSsl;
        }

        // Use our configuration to send the email by using SmtpClient
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient(PrimaryDomain, PrimaryPort)
            {
                Credentials = new NetworkCredential(FromEmail, UsernamePassword)
                
            };
            return client.SendMailAsync(
                new MailMessage(FromEmail, email, subject, htmlMessage) { IsBodyHtml = true }
            );
        }
    }
}
