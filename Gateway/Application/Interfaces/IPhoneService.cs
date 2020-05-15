using System.Collections.Generic;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface IPhoneService
    {
        Task<(string Sid, bool IsValid, List<string> Errors)> StartVerificationAsync(string phoneNumber);
        Task<(string Sid, bool IsValid, List<string> Errors)> CheckVerificationAsync(string phoneNumber, string code);
    }
}
