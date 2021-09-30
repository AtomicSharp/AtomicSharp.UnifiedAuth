using System;
using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;

namespace DefaultNamespace
{
    public class IndependentEmptyModule : AtomicModule
    {
        public bool PreConfigureServicesIsCalled { get; set; }

        public bool ConfigureServicesIsCalled { get; set; }

        public bool PostConfigureServicesIsCalled { get; set; }

        public bool OnPreApplicationInitializeIsCalled { get; set; }

        public bool OnApplicationInitializeIsCalled { get; set; }

        public bool OnPostApplicationInitializeIsCalled { get; set; }

        public bool OnApplicationShutdownIsCalled { get; set; }

        public override void PreConfigureServices(IServiceCollection services)
        {
            PreConfigureServicesIsCalled = true;
            SkipAutoServiceRegistration = true;
        }

        public override void ConfigureServices(IServiceCollection services)
        {
            ConfigureServicesIsCalled = true;
        }

        public override void PostConfigureServices(IServiceCollection services)
        {
            PostConfigureServicesIsCalled = true;
        }

        public override void OnPreApplicationInitialization(IServiceProvider serviceProvider)
        {
            OnPreApplicationInitializeIsCalled = true;
        }

        public override void OnApplicationInitialization(IServiceProvider serviceProvider)
        {
            OnApplicationInitializeIsCalled = true;
        }

        public override void OnPostApplicationInitialization(IServiceProvider serviceProvider)
        {
            OnPostApplicationInitializeIsCalled = true;
        }

        public override void OnApplicationShutdown(IServiceProvider serviceProvider)
        {
            OnApplicationShutdownIsCalled = true;
        }
    }
}