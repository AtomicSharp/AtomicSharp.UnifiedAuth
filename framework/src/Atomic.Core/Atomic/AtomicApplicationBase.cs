using System;
using System.Collections.Generic;
using Atomic.DependencyInjection;
using Atomic.ExceptionHandling;
using Atomic.Modularity;
using Atomic.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic
{
    public abstract class AtomicApplicationBase : IAtomicApplication
    {
        internal AtomicApplicationBase([NotNull] Type startupModuleType, [NotNull] IServiceCollection services)
        {
            Check.NotNull(startupModuleType, nameof(startupModuleType));
            Check.NotNull(services, nameof(services));

            StartupModuleType = startupModuleType;
            Services = services;

            services.TryAddObjectAccessor<IServiceProvider>();
            services.AddSingleton<IAtomicApplication>(this);
            services.AddSingleton<IModuleContainer>(this);

            services.AddCoreServices();
            services.AddCoreAtomicServices(this);

            Modules = LoadModules(services);
            ConfigureServices();
        }

        public IReadOnlyList<IAtomicModuleDescriptor> Modules { get; }

        [NotNull]
        public Type StartupModuleType { get; }

        public IServiceCollection Services { get; }

        public IServiceProvider ServiceProvider { get; private set; }

        public virtual void Dispose()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            foreach (var module in Modules)
            {
                module.Instance.OnApplicationShutdown(ServiceProvider);
            }
        }

        protected void SetServiceProvider(IServiceProvider serviceProvider)
        {
            ServiceProvider = serviceProvider;
            ServiceProvider.GetRequiredService<ObjectAccessor<IServiceProvider>>().Value = ServiceProvider;
        }

        protected virtual void InitializeModules()
        {
            // pre-initialize
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.OnPreApplicationInitialization(ServiceProvider);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during the pre initialize {module.Type.FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.",
                        ex);
                }
            }

            // initialize
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.OnApplicationInitialization(ServiceProvider);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during the initialize {module.Type.FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.",
                        ex);
                }
            }

            // post-initialize
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.OnPostApplicationInitialization(ServiceProvider);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during the post initialize {module.Type.FullName} phase of the module {module.Type.AssemblyQualifiedName}: {ex.Message}. See the inner exception for details.",
                        ex);
                }
            }
        }

        private IReadOnlyList<IAtomicModuleDescriptor> LoadModules(IServiceCollection services)
        {
            return services
                .GetSingletonInstance<IModuleLoader>()
                .LoadModules(
                    services,
                    StartupModuleType
                );
        }

        private void ConfigureServices()
        {
            foreach (var module in Modules)
            {
                if (module.Instance is AtomicModule atomicModule)
                {
                    atomicModule.Services = Services;
                }
            }

            // PreConfigureServices
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.PreConfigureServices(Services);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during {nameof(module.Instance.PreConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.",
                        ex);
                }
            }

            // ConfigureServices
            foreach (var module in Modules)
            {
                if (module.Instance is AtomicModule { SkipAutoServiceRegistration: false })
                {
                    Services.AddAssembly(module.Assembly);
                }

                try
                {
                    module.Instance.ConfigureServices(Services);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during {nameof(module.Instance.PreConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.",
                        ex);
                }
            }

            // PostConfigureServices
            foreach (var module in Modules)
            {
                try
                {
                    module.Instance.PostConfigureServices(Services);
                }
                catch (Exception ex)
                {
                    throw new AtomicException(
                        $"An error occurred during {nameof(module.Instance.PostConfigureServices)} phase of the module {module.Type.AssemblyQualifiedName}. See the inner exception for details.",
                        ex);
                }
            }
        }
    }
}