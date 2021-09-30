using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic
{
    internal class AtomicApplicationWithInternalServiceProvider : AtomicApplicationBase,
        IAtomicApplicationWithInternalServiceProvider
    {
        public AtomicApplicationWithInternalServiceProvider(
            [NotNull] Type startupModuleType
        ) : this(startupModuleType, new ServiceCollection())
        {
        }

        public AtomicApplicationWithInternalServiceProvider(
            [NotNull] Type startupModuleType,
            [NotNull] IServiceCollection services
        ) : base(startupModuleType, services)
        {
            services.AddSingleton<IAtomicApplicationWithInternalServiceProvider>(this);
        }

        public IServiceScope ServiceScope { get; private set; }

        public IServiceProvider CreateServiceProvider()
        {
            if (ServiceProvider != null)
            {
                return ServiceProvider;
            }

            ServiceScope = Services.BuildServiceProviderFromFactory().CreateScope();
            SetServiceProvider(ServiceScope.ServiceProvider);

            return ServiceProvider;
        }

        public void Initialize()
        {
            CreateServiceProvider();
            InitializeModules();
        }

        public override void Dispose()
        {
            base.Dispose();
            ServiceScope.Dispose();
        }
    }
}