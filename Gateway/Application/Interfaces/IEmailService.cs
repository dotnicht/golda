using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string[] emails, string subject, string message);
    }
}
