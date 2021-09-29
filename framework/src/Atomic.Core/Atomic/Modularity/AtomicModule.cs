using System;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.Modularity
{
    public abstract class AtomicModule : IAtomicModule
    {
        /// <summary>
        /// whether register service types automatically
        /// </summary>
        protected internal bool SkipAutoServiceRegistration { get; protected set; }

        public virtual void PreConfigureServices(IServiceCollection services)
        {
        }

        public virtual void ConfigureServices(IServiceCollection services)
        {
        }

        public virtual void PostConfigureServices(IServiceCollection services)
        {
        }

        public virtual void OnPreApplicationInitialization(IServiceProvider serviceProvider)
        {
        }

        public virtual void OnPostApplicationInitialization(IServiceProvider serviceProvider)
        {
        }

        public virtual void OnApplicationInitialization(IServiceProvider serviceProvider)
        {
        }

        public virtual void OnApplicationShutdown(IServiceProvider serviceProvider)
        {
        }

        public static bool IsAtomicModule(Type type)
        {
            var typeInfo = type.GetTypeInfo();

            return typeInfo.IsClass &&
                   !typeInfo.IsAbstract &&
                   !typeInfo.IsGenericType &&
                   typeof(IAtomicModule).GetTypeInfo().IsAssignableFrom(type);
        }

        internal static void CheckAtomicModuleType(Type moduleType)
        {
            if (!IsAtomicModule(moduleType))
            {
                throw new ArgumentException("Given type is not an Atomic module: " + moduleType.AssemblyQualifiedName);
            }
        }
    }
}