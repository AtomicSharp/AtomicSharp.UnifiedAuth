using System;
using System.Threading.Tasks;
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

            var connectStringFromTye = Configuration.GetConnectionString("postgres-db");
            if (string.IsNullOrEmpty(connectStringFromTye))
                throw new Exception("ConnectString is missed from Tye!");
            services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseNpgsql(connectStringFromTye + ";Database=Identity", builder =>
                    {
                        builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                    });
                }
            );

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            connectStringFromTye += ";Database=IdentityServer";
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
                    options.ConfigureDbContext = b => b.UseNpgsql(connectStringFromTye, builder =>
                    {
                        builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                    });
                })
                .AddOperationalStore(options =>
                {
                    options.ConfigureDbContext = b => b.UseNpgsql(connectStringFromTye, builder =>
                    {
                        builder.EnableRetryOnFailure(10, TimeSpan.FromSeconds(30), null);
                    });
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
            await serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>().Database.EnsureCreatedAsync();

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