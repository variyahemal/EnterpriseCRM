using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Interfaces
{
    public interface IRepository<TEntity, TId>
    {
        Task<TEntity?> GetByIdAsync(TId id);
        Task<IEnumerable<TEntity>> GetAllAsync();
        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TId id);
    }
}