using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Interfaces
{
    public interface IAggregateService
    {
        Task PopulateBalances(CancellationToken cancellationToken);
    }
}
