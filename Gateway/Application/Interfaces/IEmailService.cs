using Binebase.Exchange.Gateway.Application.Enums;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string[] emails, string subject, string message, EmailType emailType);
    }
}
