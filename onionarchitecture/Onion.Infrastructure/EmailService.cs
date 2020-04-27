using Onion.Common.Utils;
using Onion.Domain.Models;
using Onion.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Infrastructure
{
    public class EmailService : IEmailService
    {
        public SmtpClient GetSmtpSettings()
        {
            return new SmtpClient();
        }

        public void SendEmail(MailMessage mailMessage)
        {
            using (var smtp = new SmtpClient())
            {
                smtp.Send(mailMessage);
            }
        }
    }
}
