using Atomic.AspNetCore.Authorization;
using Atomic.AspNetCore.Authorization.OAuth;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionAuthorizationExtensions
    {
        public static IServiceCollection AddAtomicAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            services.AddAssemblyOf<AtomicAuthorizationModule>();
            services.Configure<AtomicAuthorizationOptions>(options =>
            {
                options.AuthorizationPolicyProviders.TryAdd<IScopeAuthorizationPolicyProvider>();
            });

            return services;
        }
    }
}