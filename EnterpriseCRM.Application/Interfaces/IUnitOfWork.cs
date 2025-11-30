using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Interfaces
{
    public interface IUnitOfWork
    {
        Task<int> CompleteAsync();
    }
}