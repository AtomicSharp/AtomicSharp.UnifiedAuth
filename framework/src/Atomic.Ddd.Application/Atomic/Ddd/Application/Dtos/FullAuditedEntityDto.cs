using System;
using JetBrains.Annotations;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public abstract class FullAuditedEntityDto<TEntityKey, TActorKey> : AuditedEntityDto<TEntityKey, TActorKey>
    {
        public bool IsDeleted { get; set; }

        [CanBeNull]
        public TActorKey DeleterId { get; set; }

        public DateTime? DeletionTime { get; set; }
    }

    [Serializable]
    public abstract class FullAuditedEntityDto : FullAuditedEntityDto<Guid, Guid>
    {
    }
}