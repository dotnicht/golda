using Binebase.Exchange.Common.Domain;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Application.Interfaces
{
    public interface IAddressService
    {
        Task<string> GenerateAddress(Currency currency, int index);
    }
}
