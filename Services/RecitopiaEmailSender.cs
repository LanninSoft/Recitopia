using Microsoft.AspNetCore.Identity.UI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.AspNetCore.Mvc;
using Recitopia.Data;
using Microsoft.Extensions.Configuration;

namespace Recitopia.Services
{
    public class RecitopiaEmailSender : IEmailSender
    {
        private readonly RecitopiaDBContext _recitopiaDbContext;
        private readonly IConfiguration _config;
        private string _apiKey = null;
        public RecitopiaEmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, RecitopiaDBContext recitopiaDbContext, IConfiguration config)
        {
            Options = optionsAccessor.Value;
            _recitopiaDbContext = recitopiaDbContext ?? throw new ArgumentNullException(nameof(recitopiaDbContext));
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {

            _apiKey = _config["SendGrid:ApiKey"];

            return Execute(_apiKey, subject, htmlMessage, email);
            
        }
        public AuthMessageSenderOptions Options { get; } //set only via Secret Manager        

        public async Task<Response> Execute(string apiKey, string subject, string message, string email)
        {
           
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("recitopia@gmail.com", "Recitopia Administrator");

            ///get user name or pass it in

            var to = new EmailAddress(email, "Recitopia User");
            var plainTextContent = message;
            var htmlContent = message;
            var messageContainer = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            //var response = await client.SendEmailAsync(messageContainer);

            return await client.SendEmailAsync(messageContainer);
        }
       

        //        // Disable click tracking.
        //        // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
        //        msg.SetClickTracking(false, false);

       
    }
}
