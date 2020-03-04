using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Binebase.Exchange.Gateway.Application.Services
{
    public class ExchangeService : IExchangeService, IConfigurationProvider<ExchangeService.Configuration>
    {


        public class Configuration
        {

        }
    }
}
