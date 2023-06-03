using HUST.Core.Interfaces.Service;
using HUST.Core.Models.Param;
using HUST.Core.Settings;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using MimeKit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _mailSettings;
        private readonly IWebHostEnvironment _hostEnvironment;
        public MailService(IOptions<MailSettings> mailSettings, IWebHostEnvironment hostEnvironment)
        {
            _mailSettings = mailSettings.Value;
            _hostEnvironment = hostEnvironment;
        }

        public async Task SendEmailAsync(MailParam mailParam)
        {
            var email = new MimeMessage();

            // set from address, to address
            email.Sender = MailboxAddress.Parse(_mailSettings.Mail);
            email.To.Add(MailboxAddress.Parse(mailParam.ToEmail));

            // set subject
            email.Subject = mailParam.Subject;

            // set body
            var builder = new BodyBuilder();
            if (mailParam.Attachments != null)
            {
                byte[] fileBytes;
                foreach (var file in mailParam.Attachments)
                {
                    if (file.Length > 0)
                    {
                        using (var ms = new MemoryStream())
                        {
                            file.CopyTo(ms);
                            fileBytes = ms.ToArray();
                        }
                        builder.Attachments.Add(file.FileName, fileBytes, ContentType.Parse(file.ContentType));
                    }
                }
            }
            builder.HtmlBody = mailParam.Body;
            email.Body = builder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
            smtp.Authenticate(_mailSettings.Mail, _mailSettings.Password);

            await smtp.SendAsync(email);

            smtp.Disconnect(true);
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

        public async Task SendEmailBackupData(string toEmail, string dictionaryName, IFormFile attachment, DateTime? dateTime = null)
        {
            var contentRootPath = _hostEnvironment.ContentRootPath;
            var filePath = Path.Combine(contentRootPath, "Template", "BackupDataTemplate.html");
            StreamReader str = new StreamReader(filePath);
            string mailText = str.ReadToEnd();
            
            str.Close();

            var now = dateTime ?? DateTime.Now;
            mailText = mailText
                .Replace("[DictionaryName]", dictionaryName)
                .Replace("[DateTime]", now.ToString("dddd, dd MMMM yyyy HH:mm:ss"));

            var mailParam = new MailParam
            {
                ToEmail = toEmail,
                Subject = $"HUST PVO - Backup data",
                Body = mailText,
                Attachments = new List<IFormFile> { attachment }
            };

            await SendEmailAsync(mailParam);
        }
    }
}
