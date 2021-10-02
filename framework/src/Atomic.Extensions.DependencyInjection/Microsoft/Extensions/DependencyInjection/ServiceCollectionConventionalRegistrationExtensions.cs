using System;
using System.Collections.Generic;
using System.Reflection;
using Atomic.Extensions.DependencyInjection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionConventionalRegistrationExtensions
    {
        public static IServiceCollection AddConventionalRegistrar(
            this IServiceCollection services,
            IConventionalRegistrar registrar
        )
        {
            services.GetConventionalRegistrars().Add(registrar);
            return services;
        }

        public static List<IConventionalRegistrar> GetConventionalRegistrars(
            this IServiceCollection services,
            bool withDefaultIfNull = true
        )
        {
            var conventionalRegistrars =
                services.GetSingletonInstanceOrNull<IObjectAccessor<List<IConventionalRegistrar>>>()?.Value;
            if (conventionalRegistrars == null)
            {
                conventionalRegistrars = new List<IConventionalRegistrar>();
                if (withDefaultIfNull) conventionalRegistrars.Add(new DefaultConventionalRegistrar());
                services.AddObjectAccessor(conventionalRegistrars);
            }

            return conventionalRegistrars;
        }

        public static IServiceCollection AddAssembly(this IServiceCollection services, Assembly assembly)
        {
            foreach (var registrar in services.GetConventionalRegistrars())
            {
                registrar.AddAssembly(services, assembly);
            }

            return services;
        }

        public static IServiceCollection AddAssemblyOf<T>(this IServiceCollection services)
        {
            return services.AddAssembly(typeof(T).GetTypeInfo().Assembly);
        }

        public static IServiceCollection AddTypes(this IServiceCollection services, params Type[] types)
        {
            foreach (var registrar in services.GetConventionalRegistrars())
            {
                registrar.AddTypes(services, types);
            }

            return services;
        }

        public static IServiceCollection AddType<TType>(this IServiceCollection services)
        {
            return services.AddType(typeof(TType));
        }

        public static IServiceCollection AddType(this IServiceCollection services, Type type)
        {
            foreach (var registrar in services.GetConventionalRegistrars())
            {
                registrar.AddType(services, type);
            }

            return services;
        }
    }
}