using Binebase.Exchange.Common.Application.Common.Models;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Application
{
    public interface IIdentityService
    {
        Task<string> GetUserNameAsync(string userId);

        Task<(Result Result, string UserId)> CreateUserAsync(string userName, string password);

        Task<Result> DeleteUserAsync(string userId);
    }
}
