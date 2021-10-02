using Atomic.Modularity;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.DependencyInjection
{
    public class LazyServiceProviderTest
    {
        [Fact]
        public void Should_Get_Service_Lazily()
        {
            using var application = AtomicApplicationFactory.Create<IndependentEmptyModule>();
            application.Services.AddType(typeof(LazyClass));

            application.Initialize();

            var lazyClass = application.ServiceProvider.GetRequiredService<LazyClass>();
            lazyClass.ModuleLoader.ShouldNotBeNull();
            lazyClass.ModuleLoader.ShouldNotBeNull();
        }

        public class LazyClass : ITransientDependency
        {
            public LazyClass(ILazyServiceProvider lazyServiceProvider)
            {
                LazyServiceProvider = lazyServiceProvider;
            }

            public ILazyServiceProvider LazyServiceProvider { get; set; }

            public IModuleLoader ModuleLoader => LazyServiceProvider.LazyGetRequiredService<IModuleLoader>();
        }
    }
}