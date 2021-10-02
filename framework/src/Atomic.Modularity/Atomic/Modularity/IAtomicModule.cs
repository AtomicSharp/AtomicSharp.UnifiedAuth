using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.Modularity
{
    public interface IAtomicModule
    {
        void PreConfigureServices(IServiceCollection services);

        void ConfigureServices(IServiceCollection services);

        void PostConfigureServices(IServiceCollection services);

        void OnPreApplicationInitialization([NotNull] IServiceProvider serviceProvider);

        void OnApplicationInitialization([NotNull] IServiceProvider serviceProvider);

        void OnPostApplicationInitialization([NotNull] IServiceProvider serviceProvider);

        void OnApplicationShutdown([NotNull] IServiceProvider serviceProvider);
    }
}