using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.AspNetCore.Authorization
{
    public class AtomicAuthorizationModule : AtomicModule
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddAuthorizationCore();
        }
    }
}