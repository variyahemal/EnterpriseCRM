using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;

namespace EnterpriseCRM.Application.Features.Leads.Commands.DeleteLead
{
    public class DeleteLeadCommandHandler : IRequestHandler<DeleteLeadCommand, Unit>
    {
        private readonly ILeadRepository _leadRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteLeadCommandHandler(ILeadRepository leadRepository, IUnitOfWork unitOfWork)
        {
            _leadRepository = leadRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteLeadCommand request, CancellationToken cancellationToken)
        {
            var lead = await _leadRepository.GetByIdAsync(request.Id);

            if (lead == null)
            {
                throw new KeyNotFoundException($"Lead with ID {request.Id} not found.");
            }

            await _leadRepository.DeleteAsync((Guid)lead.Id);
            await _unitOfWork.CompleteAsync();

            return Unit.Value;
        }
    }
}