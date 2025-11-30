using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using MediatR;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Features.Contacts.Commands.CreateContact
{
    public class CreateContactCommandHandler : IRequestHandler<CreateContactCommand, Contact>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Contact> Handle(CreateContactCommand request, CancellationToken cancellationToken)
        {
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = request.FirstName ?? string.Empty,
                LastName = request.LastName ?? string.Empty,
                Email = request.Email ?? string.Empty,
                Phone = request.Phone ?? string.Empty,
                Company = request.Company ?? string.Empty,
                Address = request.Address ?? string.Empty,
                City = request.City ?? string.Empty,
                State = request.State ?? string.Empty,
                ZipCode = request.ZipCode ?? string.Empty,
                Country = request.Country ?? string.Empty,
                LeadId = request.LeadId,
                OwnerId = request.OwnerId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _contactRepository.AddAsync(contact);
            await _unitOfWork.CompleteAsync();

            return contact;
        }
    }
}