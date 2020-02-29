﻿using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Gateway.Application.Common.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Linq;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Binebase.Exchange.Gateway.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            if (services is null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configuration is null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }

            services.AddDbContext<IdentityDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("IdentityConnection"), b => b.MigrationsAssembly(typeof(IdentityDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(x => x.SignIn.RequireConfirmedEmail = false).AddDefaultTokenProviders().AddEntityFrameworkStores<IdentityDbContext>();

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.Principal.Identity is ClaimsIdentity claims && Guid.TryParse(claims.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value, out var userId))
                        {
                            var identity = context.HttpContext.RequestServices.GetRequiredService<IIdentityService>();
                            var user = identity.GetUser(userId).Result;
                            if (user == null)
                            {
                                context.Fail(new SecurityException($"User not found with ID {userId}."));
                            }
                        }

                        return Task.CompletedTask;
                    }
                };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("IdentityService.Configuration").Get<IdentityService.Configuration>().AuthSecret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            //if (!environment.IsEnvironment("Test"))
            {
                services.AddServices(Assembly.GetExecutingAssembly());

                services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                services.AddTransient<IBinanceClient, BinanceClient>();
            }

            services.AddAuthentication();

            return services;
        }
    }
}
