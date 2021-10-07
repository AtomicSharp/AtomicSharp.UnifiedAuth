using System;
using System.Linq;
using Shouldly;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionShouldlyExtensions
    {
        public static void ShouldContainService(
            this IServiceCollection services,
            Type serviceType,
            ServiceLifetime lifetime,
            Type implementationType = null
        )
        {
            var serviceDescriptor =
                services.FirstOrDefault(s => s.ServiceType == serviceType && s.Lifetime == lifetime);

            serviceDescriptor.ShouldNotBeNull();

            if (serviceDescriptor.ImplementationFactory != null)
            {
                using var provider = services.BuildServiceProvider();
                var implementService = serviceDescriptor.ImplementationFactory.Invoke(provider);
                implementService.GetType().ShouldBe(implementationType ?? serviceType);
            }
            else
                serviceDescriptor.ImplementationType.ShouldBe(implementationType ?? serviceType);

            serviceDescriptor.ImplementationInstance.ShouldBeNull();
        }

        public static void ShouldNotContainService(this IServiceCollection services, Type serviceType)
        {
            var serviceDescriptor = services.FirstOrDefault(s => s.ServiceType == serviceType);

            serviceDescriptor.ShouldBeNull();
        }

        public static void ShouldContainTransient(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType = null
        )
        {
            services.ShouldContainService(serviceType, ServiceLifetime.Transient, implementationType);
        }

        public static void ShouldContainSingleton(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType = null
        )
        {
            services.ShouldContainService(serviceType, ServiceLifetime.Singleton, implementationType);
        }

        public static void ShouldContainScoped(
            this IServiceCollection services,
            Type serviceType,
            Type implementationType = null
        )
        {
            services.ShouldContainService(serviceType, ServiceLifetime.Scoped, implementationType);
        }
    }
}