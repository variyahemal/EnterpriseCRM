using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Interfaces
{
    public interface ILeadRepository : IRepository<Lead, Guid>
    {
    }
}