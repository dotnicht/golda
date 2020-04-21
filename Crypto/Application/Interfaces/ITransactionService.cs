using Binebase.Exchange.Common.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task Subscribe(Currency currency, CancellationToken cancellationToken);
    }
}
