using Binebase.Exchange.Common.Domain;
using Binebase.Exchange.CryptoService.Domain.Entities;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IBlockchainService
    {
        Currency Currency { get; }
        Task<string> GenerateAddress(uint index);
        Task<bool> ValidateAddress(string address);
        Task<decimal> GetBalance(string address);
        Task<(string Hash, ulong Amount)> PublishTransaction(decimal amount, string address);
        Task<Transaction[]> TransferAssets(Address[] addresses, string address);
        Task<Transaction[]> GetTransactions(string address);
        Task<Transaction> GetTransaction(string hash);
    }
}
