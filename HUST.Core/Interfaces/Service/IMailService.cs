using HUST.Core.Models.Param;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUST.Core.Interfaces.Service
{
    public interface IMailService
    {
        Task SendEmailAsync(MailParam mailParam);

        Task SendEmailActivateAccount(string toEmail, string callbackLink);

        Task SendEmailResetPassword(string toEmail, string callbackLink);

        Task SendEmailBackupData(string toEmail, string dictionaryName, IFormFile attachment, DateTime? dateTime = null);
    }
}
