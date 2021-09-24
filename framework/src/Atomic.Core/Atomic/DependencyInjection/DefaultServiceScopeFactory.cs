using Microsoft.Extensions.DependencyInjection;

namespace Atomic.DependencyInjection
{
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