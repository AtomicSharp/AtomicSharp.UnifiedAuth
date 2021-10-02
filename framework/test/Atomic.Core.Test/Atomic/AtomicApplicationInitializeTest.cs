using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic
{
    public class AtomicApplicationInitializeTest
    {
        [Fact]
        public void Should_Initialize_Single_Module()
        {
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>();

            var module = application.Services.GetSingletonInstance<IndependentEmptyModule>();
            module.PreConfigureServicesIsCalled.ShouldBeTrue();
            module.ConfigureServicesIsCalled.ShouldBeTrue();
            module.PostConfigureServicesIsCalled.ShouldBeTrue();

            application.Initialize();
            application.ServiceProvider.GetRequiredService<IndependentEmptyModule>().ShouldBeSameAs(module);
            module.OnPreApplicationInitializeIsCalled.ShouldBeTrue();
            module.OnApplicationInitializeIsCalled.ShouldBeTrue();
            module.OnPostApplicationInitializeIsCalled.ShouldBeTrue();

            application.Shutdown();
            module.OnApplicationShutdownIsCalled.ShouldBeTrue();
        }
    }
}