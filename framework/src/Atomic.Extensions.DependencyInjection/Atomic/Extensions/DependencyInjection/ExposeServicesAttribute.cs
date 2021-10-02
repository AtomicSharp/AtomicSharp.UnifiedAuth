using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Atomic.Extensions.DependencyInjection
{
    /// <summary>
    /// https://docs.abp.io/en/abp/latest/Dependency-Injection#exposeservices-attribute
    /// </summary>
    public class ExposeServicesAttribute : Attribute, IExposedServiceTypesProvider
    {
        public ExposeServicesAttribute(params Type[] serviceTypes)
        {
            ServiceTypes = serviceTypes;
        }

        /// <summary>
        /// the service types that this class can provide
        /// </summary>
        public Type[] ServiceTypes { get; set; }

        /// <summary>
        /// provide service types by convention
        /// </summary>
        public bool IncludeDefaults { get; set; }

        /// <summary>
        /// provide self
        /// </summary>
        public bool IncludeSelf { get; set; }

        public Type[] GetExposedServiceTypes(Type targetType)
        {
            var serviceTypes = ServiceTypes.ToList();

            if (IncludeDefaults)
            {
                var types = GetDefaultServices(targetType);
                foreach (var type in types)
                {
                    serviceTypes.AddIfNotContains(type);
                }
            }

            if (IncludeSelf)
            {
                serviceTypes.AddIfNotContains(targetType);
            }

            return serviceTypes.ToArray();
        }

        /// <summary>
        /// get valid service types by convention
        /// </summary>
        private static List<Type> GetDefaultServices(Type type)
        {
            var serviceTypes = new List<Type>();

            foreach (var interfaceType in type.GetTypeInfo().GetInterfaces())
            {
                var interfaceName = interfaceType.Name;

                if (interfaceName.StartsWith("I"))
                {
                    interfaceName = interfaceName.Substring(1, interfaceName.Length - 1);
                }

                if (type.Name.EndsWith(interfaceName))
                {
                    serviceTypes.Add(interfaceType);
                }
            }

            return serviceTypes;
        }
    }
}