using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Entities;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface ITransactionService
    {
        Task<Transaction[]> GetTransactions(Currency currency, string address);
        Task Subscribe();
    }
}
