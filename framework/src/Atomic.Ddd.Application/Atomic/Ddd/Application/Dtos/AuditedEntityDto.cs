using System;
using JetBrains.Annotations;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public abstract class AuditedEntityDto<TEntityKey, TActorKey> : CreationAuditedEntityDto<TEntityKey, TActorKey>
    {
        public DateTime? LastModificationTime { get; set; }

        [CanBeNull]
        public TActorKey LastModifierId { get; set; }
    }

    [Serializable]
    public abstract class AuditedEntityDto : AuditedEntityDto<Guid, Guid>
    {
    }
}