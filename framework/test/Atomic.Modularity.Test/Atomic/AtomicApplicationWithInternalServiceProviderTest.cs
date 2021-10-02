using Atomic.Modularity;
using Shouldly;
using Xunit;

namespace Atomic
{
    public class AtomicApplicationWithInternalServiceProviderTest
    {
        [Fact]
        public void Should_Return_The_Same_ServiceProvider()
        {
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>();
            application.Initialize();
            var provider = application.ServiceProvider;
            application.CreateServiceProvider().ShouldBe(provider);
        }
    }
}