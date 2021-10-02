using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.Extensions.DependencyInjection
{
    public class LazyServiceProviderTest
    {
        [Fact]
        public void Should_Get_Service_Lazily()
        {
            var services = new ServiceCollection();

            services.AddAssembly(typeof(ILazyServiceProvider).Assembly);
            services.AddType<LazyClass>();
            services.AddType<DependencyClass>();
            var lazyClass = services.BuildServiceProvider().GetRequiredService<LazyClass>();

            var dependency = lazyClass.Dependency;
            dependency.ShouldNotBeNull(); // ILazyServiceProvider works
            lazyClass.Dependency.ShouldBe(dependency); // ILazyServiceProvider should get from cache
        }

        public class LazyClass : ITransientDependency
        {
            public LazyClass(ILazyServiceProvider lazyServiceProvider)
            {
                LazyServiceProvider = lazyServiceProvider;
            }

            public ILazyServiceProvider LazyServiceProvider { get; set; }

            public DependencyClass Dependency =>
                LazyServiceProvider.LazyGetRequiredService<DependencyClass>();
        }

        public class DependencyClass : ITransientDependency
        {
        }
    }
}