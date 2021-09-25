using System.Threading.Tasks;

namespace Atomic.Ddd.Application.Atomic.Ddd.Application.Services
{
    public interface IDeleteAppService<in TKey> : IAppService
    {
        Task DeleteAsync(TKey id);
    }
}
