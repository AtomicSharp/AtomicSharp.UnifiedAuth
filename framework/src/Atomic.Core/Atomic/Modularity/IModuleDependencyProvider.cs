using System;
using JetBrains.Annotations;

namespace Atomic.Modularity
{
    public interface IModuleDependencyProvider
    {
        [NotNull]
        Type[] GetDependencies();
    }
}