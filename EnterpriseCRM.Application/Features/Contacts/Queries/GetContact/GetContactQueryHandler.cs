using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Features.Contacts.Queries.GetContact
{
    public class GetContactQueryHandler : IRequestHandler<GetContactQuery, Contact?>
    {
        private readonly IContactRepository _contactRepository;

        public GetContactQueryHandler(IContactRepository contactRepository)
        {
            _contactRepository = contactRepository;
        }

        public async Task<Contact?> Handle(GetContactQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return null;
            }
            return await _contactRepository.GetByIdAsync(Guid.Parse(request.Id));
        }
    }
}