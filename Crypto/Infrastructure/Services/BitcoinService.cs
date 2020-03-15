using AutoMapper.Configuration;
using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.CryptoService.Application.Interfaces;
using NBitcoin;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.CryptoService.Infrastructure.Services
{
    public class BitcoinService : IBitcoinService, IConfigurationProvider<BitcoinService.Configuration>, ITransient<IBitcoinService>
    {
        public class Configuration
        {
        }
    }
}
