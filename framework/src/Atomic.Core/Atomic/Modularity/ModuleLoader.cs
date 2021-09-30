using System;
using System.Collections.Generic;
using System.Linq;
using Atomic.ExceptionHandling;
using Atomic.Utils;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Atomic.Modularity
{
    public class ModuleLoader : IModuleLoader
    {
        public IAtomicModuleDescriptor[] LoadModules(IServiceCollection services, Type startupModuleType)
        {
            Check.NotNull(services, nameof(services));
            Check.NotNull(startupModuleType, nameof(startupModuleType));

            var modules = GetDescriptors(services, startupModuleType);

            modules = SortByDependency(modules, startupModuleType);

            return modules.ToArray();
        }

        private List<IAtomicModuleDescriptor> SortByDependency(
            List<IAtomicModuleDescriptor> modules,
            Type startupModuleType
        )
        {
            var sortedModules = modules.SortByDependencies(m => m.Dependencies);
            sortedModules.MoveItem(m => m.Type == startupModuleType, modules.Count - 1);
            return sortedModules;
        }

        private List<IAtomicModuleDescriptor> GetDescriptors(IServiceCollection services, Type startupModuleType)
        {
            var modules = new List<AtomicModuleDescriptor>();
            var logger = LoggerFactory.Create(builder =>
            {
            }).CreateLogger<ModuleLoader>();

            // get all the depended modules
            foreach (var moduleType in AtomicModuleHelper.FindAllModuleTypes(startupModuleType, logger))
            {
                var module = (IAtomicModule)Activator.CreateInstance(moduleType);
                services.AddSingleton(moduleType, module);

                var descriptor = new AtomicModuleDescriptor(moduleType, module);
                modules.AddIfNotContains(descriptor);
            }

            // set dependency for all the modules
            foreach (var module in modules)
            {
                SetDependencies(modules, module);
            }

            return modules.Cast<IAtomicModuleDescriptor>().ToList();
        }

        protected virtual void SetDependencies(List<AtomicModuleDescriptor> modules, AtomicModuleDescriptor module)
        {
            foreach (var dependedModuleType in AtomicModuleHelper.FindDependedModuleTypes(module.Type))
            {
                var dependedModule = modules.FirstOrDefault(m => m.Type == dependedModuleType);
                if (dependedModule == null)
                {
                    throw new AtomicException("Could not find a depended module " +
                                              dependedModuleType.AssemblyQualifiedName +
                                              " for " + module.Type.AssemblyQualifiedName);
                }

                module.AddDependency(dependedModule);
            }
        }
    }
}