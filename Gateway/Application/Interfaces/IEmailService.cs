using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IEmailService
    {
        Task SendEmail(string[] emails, string subject, string message, EmailType emailType);
    }
    public enum EmailType
    {
        ConfirmRegistration,
        ResetPassword,
        DepositNotification,
        WithdrawNotification,
        ErrorNotification
    }
}
