using System;
using System.Collections.Generic;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos
{
    [Serializable]
    public class PagedResultDto<T> : ListResultDto<T>
    {
        public long TotalCount { get; set; }

        public PagedResultDto()
        {
        }

        public PagedResultDto(long totalCount, IReadOnlyList<T> items)
            : base(items)
        {
            TotalCount = totalCount;
        }
    }
}