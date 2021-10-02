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

            var exception = Should.Throw<ArgumentException>(() =>
                moduleLoader.LoadModules(new ServiceCollection(), typeof(ModuleA))
            );
            exception.Message.ShouldBe(
                "Cyclic dependency found! Item: [AtomicModuleDescriptor Atomic.Modularity.ModuleA]");
        }

        [Fact]
        public void Should_Throw_If_Not_Depends_On_Module()
        {
            var moduleLoader = new ModuleLoader();

            var exception = Should.Throw<ArgumentException>(() =>
                moduleLoader.LoadModules(new ServiceCollection(), typeof(ErrorModule))
            );
            exception.Message.ShouldBe(
                $"Given type is not an Atomic module: {typeof(NotAtomicModuleClass).AssemblyQualifiedName}");
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

    [DependsOn(typeof(NotAtomicModuleClass))]
    public class ErrorModule : AtomicModule
    {
    }

    public class NotAtomicModuleClass
    {
    }
}