using System;
using System.Linq;
using System.Reflection;
using Atomic.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.DependencyInjection
{
    public abstract class ConventionalRegistrarBase : IConventionalRegistrar
    {
        /// <summary>
        /// inject all the services in an assembly to the service container
        /// </summary>
        public virtual void AddAssembly(IServiceCollection services, Assembly assembly)
        {
            var types = AssemblyHelper
                .GetAllTypes(assembly)
                .Where(
                    type => type != null &&
                            type.IsClass &&
                            !type.IsAbstract &&
                            !type.IsGenericType
                ).ToArray();

            AddTypes(services, types);
        }

        public virtual void AddTypes(IServiceCollection services, params Type[] types)
        {
            foreach (var type in types)
            {
                AddType(services, type);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <param name="type"></param>
        public abstract void AddType(IServiceCollection services, Type type);
    }
}