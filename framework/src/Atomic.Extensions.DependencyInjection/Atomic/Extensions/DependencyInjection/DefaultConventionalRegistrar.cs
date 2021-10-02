using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Atomic.Utils;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Atomic.Extensions.DependencyInjection
{
    public class DefaultConventionalRegistrar : ConventionalRegistrarBase
    {
        public override void AddType(IServiceCollection services, Type type)
        {
            var dependencyAttribute = GetDependencyAttributeOrNull(type);
            var lifeTime = GetLifeTimeOrNull(type, dependencyAttribute);

            if (lifeTime == null)
            {
                return;
            }

            var exposedServiceTypes = GetExposedServiceTypes(type);

            foreach (var exposedServiceType in exposedServiceTypes)
            {
                var serviceDescriptor = CreateServiceDescriptor(
                    type,
                    exposedServiceType,
                    exposedServiceTypes,
                    lifeTime.Value
                );

                if (dependencyAttribute?.ReplaceServices == true)
                {
                    services.Replace(serviceDescriptor);
                }
                else if (dependencyAttribute?.TryRegister == true)
                {
                    services.TryAdd(serviceDescriptor);
                }
                else
                {
                    services.Add(serviceDescriptor);
                }
            }
        }

        protected virtual List<Type> GetExposedServiceTypes(Type type)
        {
            return ExposedServiceHelper.GetExposedServices(type);
        }

        protected virtual ServiceDescriptor CreateServiceDescriptor(
            Type implementationType,
            Type exposingServiceType,
            List<Type> allExposingServiceTypes,
            ServiceLifetime lifeTime
        )
        {
            if (lifeTime.IsIn(ServiceLifetime.Singleton, ServiceLifetime.Scoped))
            {
                var redirectedType = GetRedirectedTypeOrNull(
                    implementationType,
                    exposingServiceType,
                    allExposingServiceTypes
                );

                if (redirectedType != null)
                {
                    return ServiceDescriptor.Describe(
                        exposingServiceType,
                        provider => provider.GetService(redirectedType),
                        lifeTime
                    );
                }
            }

            return ServiceDescriptor.Describe(
                exposingServiceType,
                implementationType,
                lifeTime
            );
        }

        /// <summary>
        /// find the exact service type that will be used to implement the target type
        /// </summary>
        /// <param name="implementationType">the service type to implement the target type</param>
        /// <param name="exposingServiceType">the target type to be implemented</param>
        /// <param name="allExposingServiceTypes">all the service types that the implementationType can implement</param>
        protected virtual Type GetRedirectedTypeOrNull(
            Type implementationType,
            Type exposingServiceType,
            List<Type> allExposingServiceTypes
        )
        {
            if (allExposingServiceTypes.Count < 2)
            {
                return null;
            }

            if (exposingServiceType == implementationType)
            {
                return null;
            }

            if (allExposingServiceTypes.Contains(implementationType))
            {
                return implementationType;
            }

            return allExposingServiceTypes.FirstOrDefault(
                t => t != exposingServiceType && exposingServiceType.IsAssignableFrom(t)
            );
        }

        protected virtual DependencyAttribute GetDependencyAttributeOrNull(Type type)
        {
            return type.GetCustomAttribute<DependencyAttribute>(true);
        }

        protected virtual ServiceLifetime? GetLifeTimeOrNull(
            Type type,
            [CanBeNull] DependencyAttribute dependencyAttribute
        )
        {
            return dependencyAttribute?.LifeTime ?? GetServiceLifetimeFromClassHierarchy(type);
        }

        protected virtual ServiceLifetime? GetServiceLifetimeFromClassHierarchy(Type type)
        {
            if (typeof(ITransientDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Transient;
            }

            if (typeof(ISingletonDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Singleton;
            }

            if (typeof(IScopedDependency).GetTypeInfo().IsAssignableFrom(type))
            {
                return ServiceLifetime.Scoped;
            }

            return null;
        }
    }
}