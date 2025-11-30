using EnterpriseCRM.Domain;
using MediatR;

namespace EnterpriseCRM.Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommand : IRequest<Contact>
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? Country { get; set; }
        public Guid? LeadId { get; set; }
        public string? OwnerId { get; set; }
    }
}