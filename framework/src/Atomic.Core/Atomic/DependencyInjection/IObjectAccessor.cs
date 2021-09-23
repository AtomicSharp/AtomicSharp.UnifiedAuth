using JetBrains.Annotations;

namespace Atomic.DependencyInjection
{
    public interface IObjectAccessor<out T>
    {
        [CanBeNull]
        T Value { get; }
    }
}