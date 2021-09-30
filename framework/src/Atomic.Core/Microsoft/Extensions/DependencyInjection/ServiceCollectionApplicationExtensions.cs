using Atomic;
using Atomic.Modularity;
using JetBrains.Annotations;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionApplicationExtensions
    {
        public static IAtomicApplicationWithExternalServiceProvider AddApplication<TStartupModule>(
            [NotNull] this IServiceCollection services
        ) where TStartupModule : IAtomicModule
        {
            return AtomicApplicationFactory.Create<TStartupModule>(services);
        }
    }
}