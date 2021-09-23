using System;
using System.Collections.Generic;
using System.Linq;

namespace Atomic.DependencyInjection
{
    public static class ExposedServiceHelper
    {
        private static readonly ExposeServicesAttribute DefaultExposeServicesAttribute =
            new ExposeServicesAttribute
            {
                IncludeDefaults = true,
                IncludeSelf = true
            };

        /// <summary>
        /// get the service types that this type can provide
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static List<Type> GetExposedServices(Type type)
        {
            return type.GetCustomAttributes(true)
                .OfType<IExposedServiceTypesProvider>()
                .DefaultIfEmpty(DefaultExposeServicesAttribute)
                .SelectMany(p => p.GetExposedServiceTypes(type))
                .Distinct()
                .ToList();
        }
    }
}