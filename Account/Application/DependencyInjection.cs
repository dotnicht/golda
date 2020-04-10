using Binebase.Exchange.Common.Application;
using Microsoft.Extensions.DependencyInjection;

namespace Binebase.Exchange.AccountService.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddApplicationCommon();
            return services;
        }
    }
}
