using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure.Interfaces
{
    public interface ITransactionService
    {
        Task SyncTransactions(CancellationToken cancellationToken);
    }
}
