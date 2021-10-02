using System;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.Modularity
{
    public abstract class AtomicModule : IAtomicModule
    {
        /// <summary>
        /// whether register service types automatically
        /// </summary>
        protected internal bool SkipAutoServiceRegistration { get; protected set; }

        protected internal IServiceCollection Services { get; internal set; }

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

        public virtual void OnApplicationInitialization(IServiceProvider serviceProvider)
        {
        }

        public virtual void OnPostApplicationInitialization(IServiceProvider serviceProvider)
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

        protected void Configure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.Configure(configureOptions);
        }

        protected void Configure<TOptions>(string name, Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.Configure(name, configureOptions);
        }

        protected void Configure<TOptions>(IConfiguration configuration)
            where TOptions : class
        {
            Services.Configure<TOptions>(configuration);
        }

        protected void Configure<TOptions>(IConfiguration configuration, Action<BinderOptions> configureBinder)
            where TOptions : class
        {
            Services.Configure<TOptions>(configuration, configureBinder);
        }

        protected void Configure<TOptions>(string name, IConfiguration configuration)
            where TOptions : class
        {
            Services.Configure<TOptions>(name, configuration);
        }

        protected void PostConfigure<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.PostConfigure(configureOptions);
        }

        protected void PostConfigureAll<TOptions>(Action<TOptions> configureOptions)
            where TOptions : class
        {
            Services.PostConfigureAll(configureOptions);
        }
    }
}