using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.AccountService.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
