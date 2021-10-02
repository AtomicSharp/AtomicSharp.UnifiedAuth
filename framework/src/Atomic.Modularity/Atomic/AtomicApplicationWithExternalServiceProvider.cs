using System;
using Atomic.ExceptionHandling;
using Atomic.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic
{
    public class AtomicApplicationWithExternalServiceProvider : AtomicApplicationBase,
        IAtomicApplicationWithExternalServiceProvider
    {
        public AtomicApplicationWithExternalServiceProvider(
            [NotNull] Type startupModuleType,
            [NotNull] IServiceCollection services
        ) : base(startupModuleType, services)
        {
            services.AddSingleton<IAtomicApplicationWithExternalServiceProvider>(this);
        }

        void IAtomicApplicationWithExternalServiceProvider.SetServiceProvider(IServiceProvider serviceProvider)
        {
            if (ServiceProvider != null)
            {
                if (ServiceProvider != serviceProvider)
                {
                    throw new AtomicException(
                        "Service provider was already set before to another service provider instance.");
                }

                return;
            }

            SetServiceProvider(serviceProvider);
        }

        public void Initialize(IServiceProvider serviceProvider)
        {
            Check.NotNull(serviceProvider, nameof(serviceProvider));

            SetServiceProvider(serviceProvider);

            InitializeModules();
        }

        public override void Dispose()
        {
            base.Dispose();

            if (ServiceProvider is IDisposable disposableServiceProvider)
            {
                disposableServiceProvider.Dispose();
            }
        }
    }
}