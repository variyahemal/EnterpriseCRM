using EnterpriseCRM.Domain;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Interfaces
{
    public interface IContactRepository : IRepository<Contact, Guid>
    {
        Task<Contact?> GetContactByEmailAsync(string email);
    }
}