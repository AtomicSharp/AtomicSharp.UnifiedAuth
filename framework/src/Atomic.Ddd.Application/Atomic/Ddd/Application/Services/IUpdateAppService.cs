using System.Threading.Tasks;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Services
{
    public interface IUpdateAppService<TEntityDto, in TKey>
        : IUpdateAppService<TEntityDto, TKey, TEntityDto>
    {
    }

    public interface IUpdateAppService<TGetOutputDto, in TKey, in TUpdateInput>
        : IAppService
    {
        Task<TGetOutputDto> UpdateAsync(TKey id, TUpdateInput input);
    }
}