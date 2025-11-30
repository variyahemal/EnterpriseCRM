using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Queries.GetLead
{
    public class GetLeadQueryHandler : IRequestHandler<GetLeadQuery, Lead?>
    {
        private readonly IRepository<Lead, Guid> _leadRepository;

        public GetLeadQueryHandler(IRepository<Lead, Guid> leadRepository)
        {
            _leadRepository = leadRepository;
        }

        public async Task<Lead?> Handle(GetLeadQuery request, CancellationToken cancellationToken)
        {
            return await _leadRepository.GetByIdAsync(request.Id);
        }
    }
}