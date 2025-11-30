using MediatR;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Contacts.Queries.GetContact
{
    public class GetContactQuery : IRequest<Contact>
    {
        public string? Id { get; set; }
    }
}