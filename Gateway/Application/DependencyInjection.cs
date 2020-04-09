using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Binebase.Exchange.Gateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddApplicationCommon();
            services.AddTransient<ICalculationService, CalculationService>();
            services.AddTransient<IExchangeRateService, ExchangeRateService>();
            return services;
        }
    }
}
