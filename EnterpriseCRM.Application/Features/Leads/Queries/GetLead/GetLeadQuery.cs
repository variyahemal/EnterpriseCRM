using MediatR;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Queries.GetLead
{
    public class GetLeadQuery : IRequest<Lead>
    {
        public Guid Id { get; set; }
    }
}