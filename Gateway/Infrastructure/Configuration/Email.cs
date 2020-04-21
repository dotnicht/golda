using Binebase.Exchange.Common.Application.Interfaces;

namespace Binebase.Exchange.Gateway.Infrastructure.Configuration
{
    public class Email : IConfig
    {
        public string ApiKey { get; set; }
        public string FromAddress { get; set; }
        public string ConfirmRegistrationTemplateKey { get; set; }
        public string ResetPasswordTemplateKey { get; set; }
        public string DepositConfirmTemplateKey { get; set; }
        public string WithdrawRequestTemplateKey { get; set; }
        public string ErrorNotificationTemplateKey { get; set; }
    }
}
