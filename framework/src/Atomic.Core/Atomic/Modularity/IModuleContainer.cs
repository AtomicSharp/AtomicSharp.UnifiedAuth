using System.Collections.Generic;
using JetBrains.Annotations;

namespace Atomic.Modularity
{
    public interface IModuleContainer
    {
        [NotNull]
        IReadOnlyList<IAtomicModule> Modules { get; }
    }
}