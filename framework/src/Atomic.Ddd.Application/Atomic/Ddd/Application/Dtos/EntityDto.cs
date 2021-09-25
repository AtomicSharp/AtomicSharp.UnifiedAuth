using System;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public abstract class EntityDto<TKey>
    {
        public TKey Id { get; set; }

        public override string ToString()
        {
            return $"[DTO: {GetType().Name}] Id = {Id}";
        }
    }

    [Serializable]
    public abstract class EntityDto : EntityDto<Guid>
    {
    }
}