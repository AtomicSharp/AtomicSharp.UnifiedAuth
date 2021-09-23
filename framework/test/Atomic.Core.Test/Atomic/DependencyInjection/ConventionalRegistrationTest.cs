using System;
using AppTestBase.Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Atomic.DependencyInjection
{
    public class ConventionalRegistrarTest
    {
        [Fact]
        public void Should_Use_Custom_Conventions_If_Added()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new MyCustomConventionalRegistrar());
            services.AddTypes(typeof(RegisteredByCustomConvention), typeof(NoneRegisteredClass));

            services.ShouldContainSingleton(typeof(RegisteredByCustomConvention));
            services.ShouldNotContainService(typeof(NoneRegisteredClass));
        }

        [Fact]
        public void Should_Inject_In_Two_Ways()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new DefaultConventionalRegistrar());
            services.AddTypes(
                typeof(NoneRegisteredClass),
                typeof(RegisteredByDependencyAttribute),
                typeof(RegisteredByDependencyInterface)
            );

            services.ShouldNotContainService(typeof(NoneRegisteredClass));
            services.ShouldContainTransient(typeof(RegisteredByDependencyAttribute));
            services.ShouldContainScoped(typeof(RegisteredByDependencyInterface));
        }

        [Fact]
        public void Should_Attribute_Win_Defaults()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new DefaultConventionalRegistrar());
            services.AddTypes(
                typeof(RegisteredByDependencyAttributeAndInterface),
                typeof(TaxCalculatorWithExposeAttribute),
                typeof(TaxCalculator)
            );

            // DependencyAttribute should win, so lifetime is transient
            services.ShouldContainTransient(typeof(RegisteredByDependencyAttributeAndInterface));
            // ExposeServicesAttribute should win, IncludeDefaults and IncludeSelf was set false
            services.ShouldNotContainService(typeof(ICanCalculate));
            services.ShouldContainTransient(typeof(ICanCalculateWithExpose), typeof(TaxCalculatorWithExposeAttribute));
        }

        [Fact]
        public void Should_Inject_By_Convention()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new DefaultConventionalRegistrar());
            services.AddTypes(typeof(TaxCalculator));

            // IncludeDefaults and IncludeSelf was set true
            services.ShouldNotContainService(typeof(ICanCalculate));
            services.ShouldContainTransient(typeof(ICalculator), typeof(TaxCalculator));
            services.ShouldContainTransient(typeof(ITaxCalculator), typeof(TaxCalculator));
            services.ShouldContainTransient(typeof(TaxCalculator));
        }

        [Fact]
        public void Should_Inject_With_Implement_Factory_When_Not_Transient()
        {
            var services = new ServiceCollection();

            services.AddConventionalRegistrar(new DefaultConventionalRegistrar());
            services.AddTypes(typeof(ScopedTaxCalculator));

            services.ShouldContainScoped(typeof(ICalculator), typeof(ScopedTaxCalculator));
            services.ShouldContainScoped(typeof(ITaxCalculator), typeof(ScopedTaxCalculator));
            services.ShouldContainScoped(typeof(ScopedTaxCalculator), typeof(ScopedTaxCalculator));
        }

        public class MyCustomConventionalRegistrar : ConventionalRegistrarBase
        {
            public override void AddType(IServiceCollection services, Type type)
            {
                if (type == typeof(RegisteredByCustomConvention))
                {
                    services.AddSingleton<RegisteredByCustomConvention>();
                }
            }
        }

        public class NoneRegisteredClass
        {
        }

        public class RegisteredByCustomConvention
        {
        }

        [Dependency(ServiceLifetime.Transient)]
        public class RegisteredByDependencyAttribute
        {
        }

        [Dependency(ServiceLifetime.Transient)]
        public class RegisteredByDependencyAttributeAndInterface : IScopedDependency
        {
        }

        public class RegisteredByDependencyInterface : IScopedDependency
        {
        }

        public interface ICanCalculate
        {
        }

        public interface ICanCalculateWithExpose
        {
        }

        public interface ICalculator
        {
        }

        public interface ITaxCalculator
        {
        }

        public class TaxCalculator : ITaxCalculator, ICalculator, ICanCalculate, ITransientDependency
        {
        }

        public class ScopedTaxCalculator : ITaxCalculator, ICalculator, ICanCalculate, IScopedDependency
        {
        }

        [ExposeServices(typeof(ICanCalculateWithExpose))]
        public class TaxCalculatorWithExposeAttribute : ITaxCalculator, ICalculator, ICanCalculate,
            ICanCalculateWithExpose, ITransientDependency
        {
        }
    }
}