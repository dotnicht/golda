using Binebase.Exchange.Common.Domain;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IBlockchainService
    {
        Task<string> PublishTransaction(Currency currency, decimal amount, string address);
        Task<ulong> CurrentIndex(Currency currency);
    }
}
