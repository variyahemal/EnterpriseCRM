using AutoMapper;
using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Commands.CreateLead
{
    public class CreateLeadCommandHandler : IRequestHandler<CreateLeadCommand, Lead>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateLeadCommandHandler(ILeadRepository leadRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _leadRepository = leadRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Lead> Handle(CreateLeadCommand request, CancellationToken cancellationToken)
        {
            var lead = _mapper.Map<Lead>(request);
            await _leadRepository.AddAsync(lead);
            await _unitOfWork.CompleteAsync();
            return lead;
        }
    }
}