using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Gateway.Application.Configuration;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Binebase.Exchange.Gateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddCommonApplication();
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<IExchangeRateService, ExchangeRateService>();
            services.Configure<ExchangeRates>(configuration.GetSection("Application.ExchangeRates"));
            services.Configure<CryptoOperations>(configuration.GetSection("Application.CryptoOperations"));
            services.Configure<MiningCalculation>(configuration.GetSection("Application.MiningCalculation"));
            return services;
        }
    }
}
