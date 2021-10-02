using Atomic.ExceptionHandling;
using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic
{
    public class AtomicApplicationWithExternalServiceProviderTest
    {
        [Fact]
        public void Should_Initialize_Successfully()
        {
            var services = new ServiceCollection();
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>(services);

            var serviceProvider = services.BuildServiceProviderFromFactory().CreateScope().ServiceProvider;
            application.Initialize(serviceProvider);

            application.Services.ShouldBe(services);
            application.ServiceProvider.ShouldBe(serviceProvider);
        }

        [Fact]
        public void Should_Set_With_The_Same_ServiceProvider()
        {
            var services = new ServiceCollection();
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>(services);

            var serviceProvider = services.BuildServiceProviderFromFactory().CreateScope().ServiceProvider;
            application.SetServiceProvider(serviceProvider);
            application.SetServiceProvider(serviceProvider);
            application.Initialize(serviceProvider);

            var anotherServiceProvider = services.BuildServiceProviderFromFactory().CreateScope().ServiceProvider;
            Should.Throw<AtomicException>(() =>
            {
                application.SetServiceProvider(anotherServiceProvider);
            });
        }
    }
}