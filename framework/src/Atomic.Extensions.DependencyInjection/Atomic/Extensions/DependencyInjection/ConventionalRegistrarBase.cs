using System;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.Extensions.DependencyInjection
{
    public abstract class ConventionalRegistrarBase : IConventionalRegistrar
    {
        public virtual void AddAssembly(IServiceCollection services, Assembly assembly)
        {
            var types = assembly
                .GetAllTypes()
                .Where(type => type is { IsClass: true, IsAbstract: false, IsGenericType: false })
                .ToArray();

            AddTypes(services, types);
        }

        public virtual void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        public abstract void AddType(IServiceCollection services, Type type);
    }
}