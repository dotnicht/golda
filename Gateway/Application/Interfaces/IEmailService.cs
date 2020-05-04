using Binebase.Exchange.Common.Domain;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmRegistrationEmail(string[] emails, string subject, string message);
        Task SendResetPasswordEmail(string[] emails, string subject, string message);
        Task SendDepositNotificationEmail(string[] emails, string subject, decimal amount1, Currency currency1, decimal amount2, Currency currency2);
        Task SendWithdrawNotificationEmail(string[] emails, string subject, decimal amount1, Currency currency1);
        Task SendErrorNotificationEmail(string[] emails, string subject, string message);
    }
}
