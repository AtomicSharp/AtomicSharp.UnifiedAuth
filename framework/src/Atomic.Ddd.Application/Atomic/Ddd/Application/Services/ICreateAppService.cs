using System.Threading.Tasks;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Services
{
    public interface ICreateAppService<TEntityDto>
        : ICreateAppService<TEntityDto, TEntityDto>
    {
    }

    public interface ICreateAppService<TGetOutputDto, in TCreateInput>
        : IAppService
    {
        Task<TGetOutputDto> CreateAsync(TCreateInput input);
    }
}