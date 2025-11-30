using MediatR;

namespace EnterpriseCRM.Application.Features.Leads.Commands.DeleteLead
{
    public class DeleteLeadCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
    }
}