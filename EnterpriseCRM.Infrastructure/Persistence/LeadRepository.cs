using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using EnterpriseCRM.Infrastructure.Persistence;

namespace EnterpriseCRM.Infrastructure.Persistence
{
    public class LeadRepository : Repository<Lead, Guid>, ILeadRepository
    {
        public LeadRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}