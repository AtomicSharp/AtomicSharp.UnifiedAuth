using System.Threading.Tasks;
using Atomic.Ddd.Application.Atomic.Ddd.Application.Dtos;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Services
{
    public interface IReadOnlyAppService<TEntityDto, in TKey>
        : IReadOnlyAppService<TEntityDto, TEntityDto, TKey, PagedAndSortedResultRequestDto>
    {
    }

    public interface IReadOnlyAppService<TEntityDto, in TKey, in TGetListInput>
        : IReadOnlyAppService<TEntityDto, TEntityDto, TKey, TGetListInput>
    {
    }

    public interface IReadOnlyAppService<TGetOutputDto, TGetListOutputDto, in TKey, in TGetListInput>
        : IAppService
    {
        Task<TGetOutputDto> GetAsync(TKey id);

        Task<PagedResultDto<TGetListOutputDto>> GetListAsync(TGetListInput input);
    }
}