using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Entities;
using Binebase.Exchange.CryptoService.Domain.Enums;
using System.Threading;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task Subscribe(Currency currency, AddressType type, CancellationToken cancellationToken);
    }
}
