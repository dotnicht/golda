﻿using Binebase.Exchange.Common.Application;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Binebase.Exchange.Gateway.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddApplicationCommon();
            services.AddMediatR(Assembly.GetExecutingAssembly());
            services.AddServices(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}
