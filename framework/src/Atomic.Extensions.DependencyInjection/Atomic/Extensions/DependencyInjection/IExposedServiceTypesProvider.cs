using System;

namespace Atomic.Extensions.DependencyInjection
{
    public interface IExposedServiceTypesProvider
    {
        Type[] GetExposedServiceTypes(Type targetType);
    }
}