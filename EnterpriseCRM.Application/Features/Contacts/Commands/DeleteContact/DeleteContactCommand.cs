using MediatR;

namespace EnterpriseCRM.Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommand : IRequest<bool>
    {
        public string? Id { get; set; }
    }
}