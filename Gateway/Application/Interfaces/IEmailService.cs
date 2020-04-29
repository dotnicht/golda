using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendConfirmRegistrationEmail(string[] emails, string subject, string message);
        Task SendResetPasswordEmail(string[] emails, string subject, string message);
        Task SendDepositNotificationEmail(string[] emails, string subject, string amount1, string amount2);
        Task SendWithdrawNotificationEmail(string[] emails, string subject, string amount);
        Task SendErrorNotificationEmail(string[] emails, string subject, string message);
    }
}
