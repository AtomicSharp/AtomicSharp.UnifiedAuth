using Microsoft.Extensions.DependencyInjection;

namespace Atomic.DependencyInjection
{
    [ExposeServices(
        typeof(IServiceScopeFactory),
        typeof(IHybridServiceScopeFactory),
        typeof(DefaultServiceScopeFactory)
    )]
    public class DefaultServiceScopeFactory : IHybridServiceScopeFactory, ITransientDependency
    {
        public DefaultServiceScopeFactory(IServiceScopeFactory factory)
        {
            Factory = factory;
        }

        protected IServiceScopeFactory Factory { get; }

        public IServiceScope CreateScope()
        {
            return Factory.CreateScope();
        }
    }
}