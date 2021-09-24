using System;
using Microsoft.Extensions.DependencyInjection;

namespace Atomic.DependencyInjection
{
    /// <summary>
    /// https://docs.abp.io/en/abp/latest/Dependency-Injection#dependency-attribute
    /// </summary>
    public class DependencyAttribute : Attribute
    {
        public DependencyAttribute()
        {
        }

        public DependencyAttribute(ServiceLifetime lifeTime)
        {
            LifeTime = lifeTime;
        }

        /// <summary>
        /// the lifetime of this service
        /// Dependency attribute has a higher priority than other dependency interfaces if it defines the Lifetime property.
        /// </summary>
        public ServiceLifetime? LifeTime { get; set; }

        /// <summary>
        /// skip if the type has been registered
        /// </summary>
        public bool TryRegister { get; set; }

        /// <summary>
        /// replace if the type has been registered
        /// </summary>
        public bool ReplaceServices { get; set; }
    }
}