using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Logging;
using HUST.Core.Exceptions;
using HUST.Core.Settings;
using HUST.Core.Interfaces.Service;
using Microsoft.AspNetCore.Hosting;
using HUST.Core.Models.Param;
using System.IO;

namespace HUST.Core.Utils
{
    public class MailSendgridService : IMailService
    {
        private readonly SendgridMailSettings _mailSettings;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly ILogUtil _logger;

        public MailSendgridService(IOptions<SendgridMailSettings> mailSettings, IWebHostEnvironment hostEnvironment, ILogUtil logger)
        {
            _mailSettings = mailSettings.Value;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
        }    

        public async Task SendEmailAsync(MailParam mailParam)
        {
            await Execute(mailParam);
        }

        public async Task SendEmailActivateAccount(string toEmail, string callbackLink)
        {
            var contentRootPath = _hostEnvironment.ContentRootPath;
            var filePath = Path.Combine(contentRootPath, "Template", "ActivateAccountTemplate.html");
            StreamReader str = new StreamReader(filePath);
            string mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[callback]", callbackLink);

            var mailParam = new MailParam
            {
                ToEmail = toEmail,
                Subject = $"HUST PVO - Verify your account",
                Body = mailText,
            };

            await SendEmailAsync(mailParam);
        }

        public async Task SendEmailResetPassword(string toEmail, string callbackLink)
        {
            var contentRootPath = _hostEnvironment.ContentRootPath;
            var filePath = Path.Combine(contentRootPath, "Template", "ResetPasswordTemplate.html");
            StreamReader str = new StreamReader(filePath);
            string mailText = str.ReadToEnd();
            str.Close();

            mailText = mailText.Replace("[callback]", callbackLink);

            var mailParam = new MailParam
            {
                ToEmail = toEmail,
                Subject = $"HUST PVO - Reset password",
                Body = mailText,
            };

            await SendEmailAsync(mailParam);
        }

        /// <summary>
        /// Thực hiện gửi mail với nội dung truyền vào
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="toEmail"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="messageHtml"></param>
        /// <returns></returns>
        private async Task Execute(MailParam mailParam)
        {
            var client = new SendGridClient(_mailSettings.SendGridKey);
            var msg = new SendGridMessage()
            {
                From = new EmailAddress(_mailSettings.FromAddress, _mailSettings.FromName),
                Subject = mailParam.Subject,
                HtmlContent = mailParam.Body
            };
            msg.AddTo(new EmailAddress(mailParam.ToEmail));

            // Disable click tracking.
            // See https://sendgrid.com/docs/User_Guide/Settings/tracking.html
            msg.SetClickTracking(false, false);
            var response = await client.SendEmailAsync(msg);

            _logger.LogInfo(response.IsSuccessStatusCode
                                   ? $"Email to {mailParam.ToEmail} queued successfully!"
                                   : $"Failure Email to {mailParam.ToEmail}");
        }
    }
}
