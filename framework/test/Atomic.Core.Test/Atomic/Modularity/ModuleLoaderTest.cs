using DefaultNamespace;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Atomic.Modularity
{
    public class ModuleLoaderTest
    {
        [Fact]
        public void Should_Load_Modules_By_Dependency_Order()
        {
            var moduleLoader = new ModuleLoader();
            var modules = moduleLoader.LoadModules(new ServiceCollection(), typeof(MyStartupModule));
            modules.Length.ShouldBe(2);
            modules[0].Type.ShouldBe(typeof(IndependentEmptyModule));
            modules[1].Type.ShouldBe(typeof(MyStartupModule));
        }
    }

    [DependsOn(typeof(IndependentEmptyModule))]
    public class MyStartupModule : AtomicModule
    {
    }
}