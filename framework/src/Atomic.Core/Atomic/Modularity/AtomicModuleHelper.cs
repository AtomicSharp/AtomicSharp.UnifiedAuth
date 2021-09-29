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

            moduleTypes.AddIfNotContains(moduleType);
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
    }
}