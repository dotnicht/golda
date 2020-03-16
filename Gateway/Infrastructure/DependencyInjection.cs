using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Common.Application;
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
using Microsoft.Extensions.Logging;
using Binebase.Exchange.Common.Infrastructure.Services;

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
                    OnTokenValidated = HandleTokenValidation
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
                services.AddServices(typeof(DateTimeService).Assembly);

                services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
                services.AddTransient<IBinanceClient, BinanceClient>();
            }

            services.AddAuthentication();

            return services;
        }

        private static async Task HandleTokenValidation(TokenValidatedContext context) 
        {
            if (context.Principal.Identity is ClaimsIdentity claims 
                && Guid.TryParse(claims.Claims.Single(x => x.Type == ClaimTypes.NameIdentifier).Value, out var userId))
            {
                var user = await context.HttpContext.RequestServices
                    .GetRequiredService<IIdentityService>()
                    .GetUser(userId);

                if (user == null)
                {
                    context.Fail(new SecurityException($"User not found with ID {userId}."));
                }
                else
                {
                    context.HttpContext.RequestServices
                        .GetRequiredService<ILogger<IIdentityService>>()
                        .LogInformation($"Token validated. User ID {user.Id} ({user.Email}).");


                }
            }

            await Task.CompletedTask;
        }
    }
}
