using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Reflection;
using JetBrains.Annotations;

namespace Atomic.Modularity
{
    public class AtomicModuleDescriptor : IAtomicModuleDescriptor
    {
        private readonly List<IAtomicModuleDescriptor> _dependencies;

        public AtomicModuleDescriptor(
            [NotNull] Type type,
            [NotNull] IAtomicModule instance
        )
        {
            Type = type;
            Assembly = type.Assembly;
            Instance = instance;

            _dependencies = new List<IAtomicModuleDescriptor>();
        }

        public Type Type { get; }

        public Assembly Assembly { get; }

        public IAtomicModule Instance { get; }

        public IReadOnlyList<IAtomicModuleDescriptor> Dependencies => _dependencies.ToImmutableList();

        public void AddDependency(IAtomicModuleDescriptor moduleDescriptor)
        {
            _dependencies.AddIfNotContains(moduleDescriptor);
        }

        public override string ToString()
        {
            return $"[AtomicModuleDescriptor {Type.FullName}]";
        }
    }
}