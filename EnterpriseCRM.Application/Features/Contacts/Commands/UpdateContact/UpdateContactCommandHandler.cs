using MediatR;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using System.Threading;
using System.Threading.Tasks;

namespace EnterpriseCRM.Application.Features.Contacts.Commands.UpdateContact
{
    public class UpdateContactCommandHandler : IRequestHandler<UpdateContactCommand, Contact?>
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateContactCommandHandler(IContactRepository contactRepository, IUnitOfWork unitOfWork)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Contact?> Handle(UpdateContactCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == null)
            {
                return null;
            }
            var contact = await _contactRepository.GetByIdAsync(Guid.Parse(request.Id));
            if (contact == null)
            {
                return null;
            }

            contact.FirstName = request.FirstName ?? contact.FirstName;
            contact.LastName = request.LastName ?? contact.LastName;
            contact.Email = request.Email ?? contact.Email;
            contact.Phone = request.Phone ?? contact.Phone;
            contact.Company = request.Company ?? contact.Company;
            contact.Address = request.Address ?? contact.Address;
            contact.City = request.City ?? contact.City;
            contact.State = request.State ?? contact.State;
            contact.ZipCode = request.ZipCode ?? contact.ZipCode;
            contact.Country = request.Country ?? contact.Country;
            contact.UpdatedAt = DateTime.UtcNow;

            await _contactRepository.UpdateAsync(contact);
            await _unitOfWork.CompleteAsync();

            return contact;
        }
    }
}