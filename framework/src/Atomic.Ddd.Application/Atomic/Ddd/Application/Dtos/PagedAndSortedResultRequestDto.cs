using System;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public class PagedAndSortedResultRequestDto : PagedResultRequestDto
    {
        public virtual string Sorting { get; set; }
    }
}