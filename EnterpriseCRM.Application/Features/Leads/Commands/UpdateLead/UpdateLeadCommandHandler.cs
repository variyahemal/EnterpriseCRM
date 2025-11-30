using AutoMapper;
using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Commands.UpdateLead
{
    public class UpdateLeadCommandHandler : IRequestHandler<UpdateLeadCommand, Unit>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UpdateLeadCommandHandler(ILeadRepository leadRepository, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _leadRepository = leadRepository;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateLeadCommand request, CancellationToken cancellationToken)
        {
            var lead = await _leadRepository.GetByIdAsync(request.Id);

            if (lead == null)
            {
                throw new KeyNotFoundException($"Lead with ID {request.Id} not found.");
            }

            _mapper.Map(request, lead);
            await _leadRepository.UpdateAsync(lead);
            await _unitOfWork.CompleteAsync();

            return Unit.Value;
        }
    }
}