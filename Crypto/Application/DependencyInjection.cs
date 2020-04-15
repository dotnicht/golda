using Binebase.Exchange.Common.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Binebase.Exchange.CryptoService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddCommonApplication();
            return services;
        }
    }
}
