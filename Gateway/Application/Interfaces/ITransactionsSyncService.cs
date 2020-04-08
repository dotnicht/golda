using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Application.Interfaces
{
    public interface ITransactionsSyncService
    {
        Task SyncTransactions();
    }
}
