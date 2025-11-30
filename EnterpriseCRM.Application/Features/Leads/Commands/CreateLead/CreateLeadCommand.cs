using MediatR;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Commands.CreateLead
{
    public class CreateLeadCommand : IRequest<Lead>
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
    }
}