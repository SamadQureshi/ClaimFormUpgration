using Onion.Common.Utils;
using Onion.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TCO.TFM.WDMS.ViewModels.ViewModels;

namespace Onion.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendEmail(MailMessage mailMessage);
    }
}
