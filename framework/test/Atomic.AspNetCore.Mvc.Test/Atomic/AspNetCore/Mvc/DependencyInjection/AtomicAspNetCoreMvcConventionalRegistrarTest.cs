using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.AspNetCore.Mvc.DependencyInjection
{
    public class AtomicAspNetCoreMvcConventionalRegistrarTest
    {
        [Fact]
        public void Should_Register_Mvc_Services()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new AtomicAspNetCoreMvcConventionalRegistrar());
            services.AddTypes(typeof(TestController), typeof(TestPageModel), typeof(TestViewComponent));

            services.ShouldContainTransient(typeof(TestController));
            services.ShouldContainTransient(typeof(TestPageModel));
            services.ShouldContainTransient(typeof(TestViewComponent));

            var serviceProvider = services.BuildServiceProvider();
            serviceProvider.GetServices<TestController>().Count().ShouldBe(1);
            serviceProvider.GetServices<TestPageModel>().Count().ShouldBe(1);
            serviceProvider.GetServices<TestViewComponent>().Count().ShouldBe(1);
        }

        public class TestPageModel : PageModel
        {
        }

        public class TestController : Controller
        {
        }

        public class TestViewComponent : ViewComponent
        {
        }
    }
}