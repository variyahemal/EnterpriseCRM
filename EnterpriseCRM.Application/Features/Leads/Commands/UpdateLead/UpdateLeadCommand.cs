using MediatR;

namespace EnterpriseCRM.Application.Features.Leads.Commands.UpdateLead
{
    public class UpdateLeadCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
    }
}