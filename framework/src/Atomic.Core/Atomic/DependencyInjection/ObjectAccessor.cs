﻿using JetBrains.Annotations;

namespace Atomic.DependencyInjection
{
    public class ObjectAccessor<T> : IObjectAccessor<T>
    {
        public ObjectAccessor()
        {
        }

        public ObjectAccessor([CanBeNull] T value)
        {
            Value = value;
        }

        public T Value { get; set; }
    }
}