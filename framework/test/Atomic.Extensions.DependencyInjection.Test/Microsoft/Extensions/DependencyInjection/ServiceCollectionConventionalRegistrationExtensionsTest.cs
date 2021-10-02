using Atomic.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Microsoft.Extensions.DependencyInjection
{
    public class ServiceCollectionConventionalRegistrationExtensionsTest
    {
        [Fact]
        public void Should_Get_Conventional_Register_Without_Default()
        {
            var services = new ServiceCollection();

            services.GetConventionalRegistrars(false).ShouldBeEmpty();
        }

        [Fact]
        public void Should_Add_Assembly_With_Default_Conventional_Register()
        {
            var services = new ServiceCollection();

            services.AddAssembly(typeof(ISingletonDependency).Assembly);

            services.ShouldContainTransient(typeof(ILazyServiceProvider), typeof(DefaultLazyServiceProvider));
        }

        [Fact]
        public void Should_Add_Type_With_Generic()
        {
            var services = new ServiceCollection();

            services.AddType<DefaultLazyServiceProvider>();

            services.ShouldContainTransient(typeof(ILazyServiceProvider), typeof(DefaultLazyServiceProvider));
        }
    }
}