using System;
using System.ComponentModel.DataAnnotations;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public class PagedResultRequestDto : LimitedResultRequestDto
    {
        [Range(0, int.MaxValue)]
        public virtual int SkipCount { get; set; }
    }
}