using System;
using System.Collections.Generic;
using System.Reflection;

namespace Atomic.Modularity
{
    public interface IAtomicModuleDescriptor
    {
        Type Type { get; }

        Assembly Assembly { get; }

        IAtomicModule Instance { get; }

        IReadOnlyList<IAtomicModuleDescriptor> Dependencies { get; }
    }
}