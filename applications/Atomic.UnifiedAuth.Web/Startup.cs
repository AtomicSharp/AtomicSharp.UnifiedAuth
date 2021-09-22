﻿using System.Threading.Tasks;
using Atomic.UnifiedAuth.Web.Controllers.Account;
using Atomic.UnifiedAuth.Web.Controllers.Consent;
using Atomic.UnifiedAuth.Web.Data;
using IdentityServer4;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Atomic.UnifiedAuth.Web
{
    public class Startup
    {
        public Startup(IWebHostEnvironment environment, IConfiguration configuration)
        {
            Environment = environment;
            Configuration = configuration;
        }

        private IWebHostEnvironment Environment { get; }
        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(Configuration.GetConnectionString("Identity"))
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                    options.EmitStaticAudienceClaim = true;
                })
                .AddAspNetIdentity<IdentityUser>()
                .AddConfigurationStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(
                        Configuration.GetConnectionString("IdentityServer"));
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(
                        Configuration.GetConnectionString("IdentityServer"));
                })
                // not recommended for production - you need to store your key material somewhere secure
                .AddDeveloperSigningCredential();

            services.AddAuthentication()
                .AddGitHub(options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = Configuration["ExternalIdp:GitHub:ClientId"];
                    options.ClientSecret = Configuration["ExternalIdp:GitHub:ClientSecret"];
                });

            services.Configure<AccountOptions>(Configuration.GetSection("Account"));
            services.Configure<ConsentOptions>(Configuration.GetSection("Consent"));

            services.AddLocalization();
        }

        public void Configure(IApplicationBuilder app)
        {
            InitializeDatabase(app).GetAwaiter().GetResult();

            if (Environment.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseStaticFiles();

            app.UseRouting();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDefaultControllerRoute();
            });
        }

        private async Task InitializeDatabase(IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();

            var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            if (await userManager.FindByNameAsync("alice") == null)
            {
                await userManager.CreateAsync(new IdentityUser("alice"), "Pass123$");
            }

            if (await userManager.FindByNameAsync("bob") == null)
            {
                await userManager.CreateAsync(new IdentityUser("bob"), "Pass123$");
            }
        }
    }
}