using System.Linq;
using Atomic.AspNetCore.DependencyInjection;
using Atomic.DependencyInjection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Test.Atomic.AspNetCore.DependencyInjection
{
    public class DependencyInjectionTest
    {
        [Fact]
        public void Should_Register_Interface_From_Core()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new DefaultConventionalRegistrar());
            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTypes(
                typeof(HttpContextClientScopeServiceProviderAccessor),
                typeof(HttpContextServiceScopeFactory)
            );

            services.ShouldContainTransient(typeof(IHybridServiceScopeFactory), typeof(HttpContextServiceScopeFactory));
            services.ShouldContainSingleton(
                typeof(IClientScopeServiceProviderAccessor),
                typeof(HttpContextClientScopeServiceProviderAccessor)
            );

            // property ReplaceService in DependencyAttribute should work
            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetServices<IHybridServiceScopeFactory>().Count().ShouldBe(1);
        }
    }
}