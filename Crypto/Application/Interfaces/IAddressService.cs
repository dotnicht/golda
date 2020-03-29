using Binebase.Exchange.Common.Domain;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IAddressService
    {
        Task<string> GenerateAddress(Currency currency, uint index);
    }
}
