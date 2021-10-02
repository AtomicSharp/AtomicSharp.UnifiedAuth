using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Atomic.Modularity
{
    public static class AtomicModuleHelper
    {
        public static List<Type> FindAllModuleTypes(Type startupModuleType, ILogger logger)
        {
            var moduleTypes = new List<Type>();
            logger.Log(LogLevel.Information, "Loaded Atomic modules:");
            AddModuleAndDependenciesRecursively(moduleTypes, startupModuleType, logger, 0);
            return moduleTypes;
        }

        private static void AddModuleAndDependenciesRecursively(
            List<Type> moduleTypes,
            Type moduleType,
            ILogger logger,
            int depth
        )
        {
            AtomicModule.CheckAtomicModuleType(moduleType);

            if (moduleTypes.Contains(moduleType))
            {
                return;
            }

            moduleTypes.Add(moduleType);
            logger.Log(LogLevel.Information, $"{new string(' ', depth * 2)}- {moduleType.FullName}");

            foreach (var dependedModuleType in FindDependedModuleTypes(moduleType))
            {
                AddModuleAndDependenciesRecursively(moduleTypes, dependedModuleType, logger, depth + 1);
            }
        }

        public static IEnumerable<Type> FindDependedModuleTypes(Type moduleType)
        {
            AtomicModule.CheckAtomicModuleType(moduleType);

            var dependencies = new List<Type>();

            var dependencyDescriptors = moduleType
                .GetCustomAttributes()
                .OfType<IModuleDependencyProvider>();

            foreach (var descriptor in dependencyDescriptors)
            {
                foreach (var dependedModuleType in descriptor.GetDependencies())
                {
                    dependencies.AddIfNotContains(dependedModuleType);
                }
            }

            return dependencies;
        }

        /// <summary>
        /// Sort a list by a topological sorting, which consider their dependencies.
        /// </summary>
        /// <param name="source">A list of objects to sort</param>
        /// <returns>
        /// Returns a new list ordered by dependencies.
        /// If A depends on B, then B will come before than A in the resulting list.
        /// </returns>
        public static List<IAtomicModuleDescriptor> SortByDependencies(
            IEnumerable<IAtomicModuleDescriptor> source
        )
        {
            /* See: http://www.codeproject.com/Articles/869059/Topological-sorting-in-Csharp
             *      http://en.wikipedia.org/wiki/Topological_sorting
             */

            var sorted = new List<IAtomicModuleDescriptor>();
            var visited = new Dictionary<IAtomicModuleDescriptor, bool>();

            foreach (var item in source)
            {
                SortByDependenciesVisit(item, sorted, visited);
            }

            return sorted;
        }

        /// <param name="item">Item to resolve</param>
        /// <param name="sorted">List with the sortet items</param>
        /// <param name="visited">Dictionary with the visited items</param>
        private static void SortByDependenciesVisit(
            IAtomicModuleDescriptor item,
            List<IAtomicModuleDescriptor> sorted,
            Dictionary<IAtomicModuleDescriptor, bool> visited
        )
        {
            bool inProcess;
            var alreadyVisited = visited.TryGetValue(item, out inProcess);

            if (alreadyVisited)
            {
                if (inProcess)
                {
                    throw new ArgumentException("Cyclic dependency found! Item: " + item);
                }
            }
            else
            {
                visited[item] = true;

                var dependencies = item.Dependencies;
                if (dependencies != null)
                {
                    foreach (var dependency in dependencies)
                    {
                        SortByDependenciesVisit(dependency, sorted, visited);
                    }
                }

                visited[item] = false;
                sorted.Add(item);
            }
        }
    }
}