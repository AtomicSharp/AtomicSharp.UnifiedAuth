using System;
using JetBrains.Annotations;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.Modularity
{
    public interface IModuleLoader
    {
        [NotNull]
        IAtomicModuleDescriptor[] LoadModules(
            [NotNull] IServiceCollection services,
            [NotNull] Type startupModuleType
        );
    }
}