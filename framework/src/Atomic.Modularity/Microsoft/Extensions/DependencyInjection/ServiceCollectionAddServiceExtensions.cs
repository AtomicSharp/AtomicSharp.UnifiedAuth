using Atomic;
using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    internal static class ServiceCollectionAddServiceExtensions
    {
        internal static void AddCoreServices(this IServiceCollection services)
        {
            services.AddOptions();
            services.AddLogging();
        }

        internal static void AddCoreAtomicServices(
            this IServiceCollection services
        )
        {
            var moduleLoader = new ModuleLoader();
            services.TryAddSingleton<IModuleLoader>(moduleLoader);
            services.AddAssemblyOf<IAtomicApplication>();
        }
    }
}