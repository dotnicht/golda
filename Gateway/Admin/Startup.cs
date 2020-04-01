using Binebase.Exchange.Common.Application.Interfaces;
using Binebase.Exchange.Common.Infrastructure.Services;
using Binebase.Exchange.Gateway.Api.Services;
using Binebase.Exchange.Gateway.Application.Interfaces;
using Binebase.Exchange.Gateway.Infrastructure.Identity;
using Binebase.Exchange.Gateway.Infrastructure.Persistence;
using Binebase.Exchange.Gateway.Infrastructure.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Admin
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));


            services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = false)

                .AddEntityFrameworkStores<ApplicationDbContext>();
            services.AddRazorPages();
            services.AddControllersWithViews();
            services.AddMvc().AddRazorOptions(options =>
            {
                options.ViewLocationFormats.Add("/{0}.cshtml");
            });

            services.AddOptions();
            services.AddHttpContextAccessor();
            services.AddHttpClient<IAccountService, AccountService>();
            services.AddTransient<ICurrentUserService, CurrentUserService>();
            services.AddTransient<IDateTime, DateTimeService>();
            services.Configure<AccountService.Configuration>(Configuration.GetSection("AccountService.Configuration"));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });
        }
    }
}