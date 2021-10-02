using JetBrains.Annotations;

namespace Atomic.Extensions.DependencyInjection
{
    public interface IObjectAccessor<out T>
    {
        [CanBeNull]
        T Value { get; }
    }
}