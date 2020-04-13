using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ITransactionService
    {
        Task SyncTransactions(CancellationToken cancellationToken);
    }
}
