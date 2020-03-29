using Binance.Net;
using Binance.Net.Interfaces;
using Binebase.Exchange.Common.Application;
using Binebase.Exchange.Common.Application.Exceptions;
using Binebase.Exchange.Common.Infrastructure;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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

            services.AddDbContext<ApplicationDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), 
                b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(x => x.SignIn.RequireConfirmedEmail = false)
                .AddDefaultTokenProviders()
                .AddEntityFrameworkStores<ApplicationDbContext>();

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

            services.AddCommonInfrastructure();
            services.AddServices(Assembly.GetExecutingAssembly());

            services.AddSingleton<IBinanceSocketClient, BinanceSocketClient>();
            services.AddTransient<IBinanceClient, BinanceClient>();

            services.AddAuthentication();

            services.AddHttpClient<IAccountService, AccountService>();
            services.AddHttpClient<ICryptoService, CryptoService>();

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
