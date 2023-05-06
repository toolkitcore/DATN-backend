using HUST.Core.Models.Param;
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
    }
}
