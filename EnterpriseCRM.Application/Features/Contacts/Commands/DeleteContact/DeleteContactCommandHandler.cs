using MediatR;
using EnterpriseCRM.Application.Interfaces;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Features.Contacts.Commands.DeleteContact
{
    public class DeleteContactCommandHandler : IRequestHandler<DeleteContactCommand, bool>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> Handle(DeleteContactCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return false;
            }
            var contact = await _contactRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (contact == null)
            {
                return false;
            }

            await _contactRepository.DeleteAsync(Guid.Parse(request.Id));
            await _unitOfWork.CompleteAsync();

            return true;
        }
    }
}