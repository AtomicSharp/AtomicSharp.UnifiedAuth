using System;
using JetBrains.Annotations;

namespace Atomic.Modularity
{
    public class DependsOnAttribute : Attribute, IModuleDependencyProvider
    {
        public DependsOnAttribute(params Type[] dependedTypes)
        {
            DependedTypes = dependedTypes ?? Type.EmptyTypes;
        }

        [NotNull]
        public Type[] DependedTypes { get; set; }

        public virtual Type[] GetDependencies()
        {
            return DependedTypes;
        }
    }
}