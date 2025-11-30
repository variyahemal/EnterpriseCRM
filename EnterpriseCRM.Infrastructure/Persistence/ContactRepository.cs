using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace EnterpriseCRM.Infrastructure.Persistence
{
    public class ContactRepository : Repository<Contact, Guid>, IContactRepository
    {
        public ContactRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Contact?> GetContactByEmailAsync(string email)
        {
            return await _context.Contacts.FirstOrDefaultAsync(c => c.Email == email);
        }
    }
}