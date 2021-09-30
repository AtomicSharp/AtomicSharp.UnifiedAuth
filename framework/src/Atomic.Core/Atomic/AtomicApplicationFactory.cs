using System;
using Atomic.Modularity;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic
{
    public static class AtomicApplicationFactory
    {
        public static IAtomicApplicationWithInternalServiceProvider Create<TStartupModule>()
            where TStartupModule : IAtomicModule
        {
            return Create(typeof(TStartupModule));
        }

        public static IAtomicApplicationWithInternalServiceProvider Create([NotNull] Type startupModuleType)
        {
            return new AtomicApplicationWithInternalServiceProvider(startupModuleType);
        }

        public static IAtomicApplicationWithExternalServiceProvider Create<TStartupModule>(
            [NotNull] IServiceCollection services
        )
            where TStartupModule : IAtomicModule
        {
            return Create(typeof(TStartupModule), services);
        }

        public static IAtomicApplicationWithExternalServiceProvider Create(
            [NotNull] Type startupModuleType,
            [NotNull] IServiceCollection services
        )
        {
            return new AtomicApplicationWithExternalServiceProvider(startupModuleType, services);
        }
    }
}