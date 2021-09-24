using System;

namespace Atomic.DependencyInjection
{
    public interface IClientScopeServiceProviderAccessor
    {
        IServiceProvider ServiceProvider { get; }
    }
}