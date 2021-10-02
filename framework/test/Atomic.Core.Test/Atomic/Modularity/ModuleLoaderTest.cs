using System;
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

        [Fact]
        public void Should_Throw_If_Has_Cycle_Dependency()
        {
            var moduleLoader = new ModuleLoader();
            Should.Throw<ArgumentException>(() =>
            {
                moduleLoader.LoadModules(new ServiceCollection(), typeof(ModuleA));
            });
        }
    }

    [DependsOn(typeof(IndependentEmptyModule))]
    public class MyStartupModule : AtomicModule
    {
    }

    [DependsOn(typeof(ModuleB))]
    public class ModuleA : AtomicModule
    {
    }

    [DependsOn(typeof(ModuleC))]
    public class ModuleB : AtomicModule
    {
    }

    [DependsOn(typeof(ModuleA))]
    public class ModuleC : AtomicModule
    {
    }
}