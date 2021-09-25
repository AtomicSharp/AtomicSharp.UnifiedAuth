using System;
using JetBrains.Annotations;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public abstract class CreationAuditedEntityDto<TEntityKey, TActorKey> : EntityDto<TEntityKey>
    {
        public DateTime CreationTime { get; set; }

        [CanBeNull]
        public TActorKey CreatorId { get; set; }
    }

    [Serializable]
    public abstract class CreationAuditedEntityDto : CreationAuditedEntityDto<Guid, Guid>
    {
    }
}