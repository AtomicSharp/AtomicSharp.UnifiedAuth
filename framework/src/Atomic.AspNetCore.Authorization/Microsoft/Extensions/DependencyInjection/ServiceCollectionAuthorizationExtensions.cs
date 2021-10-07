using Atomic.AspNetCore.Authorization;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionAuthorizationExtensions
    {
        public static IServiceCollection AddAtomicAuthorization(this IServiceCollection services)
        {
            services.AddAuthorizationCore();
            return services.AddAssemblyOf<AtomicAuthorizationModule>();
        }
    }
}