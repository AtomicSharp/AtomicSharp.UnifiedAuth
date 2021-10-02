using Shouldly;
using Xunit;

namespace Atomic.Extensions.DependencyInjection
{
    public class ExposedServiceHelperTest
    {
        [Fact]
        public void Should_Get_Conventional_Exposed_Types_By_Default()
        {
            var exposedServices = ExposedServiceHelper.GetExposedServices(typeof(DefaultDerivedService));

            exposedServices.Count.ShouldBe(3);
            exposedServices.ShouldContain(typeof(DefaultDerivedService));
            exposedServices.ShouldContain(typeof(IService));
            exposedServices.ShouldContain(typeof(IDerivedService));
        }

        [Fact]
        public void Should_Get_Custom_Exposed_Types_If_Available()
        {
            var exposedServices = ExposedServiceHelper.GetExposedServices(typeof(ExplicitDerivedService));

            exposedServices.Count.ShouldBe(1);
            exposedServices.ShouldContain(typeof(IDerivedService));
        }

        public class DefaultDerivedService : IDerivedService
        {
        }

        public interface IService
        {
        }

        public interface IDerivedService : IService
        {
        }

        [ExposeServices(typeof(IDerivedService))]
        public class ExplicitDerivedService : IDerivedService
        {
        }
    }
}