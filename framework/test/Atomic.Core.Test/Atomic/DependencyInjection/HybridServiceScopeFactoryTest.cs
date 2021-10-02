using System;
using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.DependencyInjection
{
    public class HybridServiceScopeFactoryTest
    {
        [Fact]
        public void Should_Use_Default_ServiceScopeFactory_By_Default()
        {
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>();
            application.Services.AddType(typeof(MyService));

            application.Initialize();

            var serviceScopeFactory = application.ServiceProvider.GetRequiredService<IHybridServiceScopeFactory>();

            using (var scope = serviceScopeFactory.CreateScope())
            {
                scope.ServiceProvider.GetRequiredService<MyService>();
            }

            MyService.DisposeCount.ShouldBe(1);
        }

        private class MyService : ITransientDependency, IDisposable
        {
            public static int DisposeCount { get; private set; }

            public void Dispose()
            {
                ++DisposeCount;
            }
        }
    }
}