using Asp.Versioning;
using AutoMapper;
using EnterpriseCRM.Application.Features.Contacts.Commands.CreateContact;
using EnterpriseCRM.Application.Features.Contacts.Commands.UpdateContact;
using EnterpriseCRM.Application.Features.Contacts.Commands.DeleteContact;
using EnterpriseCRM.Application.Features.Contacts.Queries.GetContact;
using EnterpriseCRM.Application.Interfaces;
using EnterpriseCRM.Domain;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,Editor,Sales")] // Base authorization for ContactController
    public class ContactController : ControllerBase
    {
        private readonly IContactRepository _contactRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public ContactController(IContactRepository contactRepository, IUnitOfWork unitOfWork, IMediator mediator, IMapper mapper)
        {
            _contactRepository = contactRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> GetAllContacts()
        {
            var contacts = await _contactRepository.GetAllAsync();
            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<Contact>> Get(string id)
        {
            var query = new GetContactQuery { Id = id };
            var contact = await _mediator.Send(query);
            if (contact == null)
            {
                return NotFound();
            }
            return Ok(contact);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> CreateContact([FromBody] CreateContactCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var contact = _mapper.Map<Contact>(command);
            await _contactRepository.AddAsync(contact);
            await _unitOfWork.CompleteAsync();
            return CreatedAtAction(nameof(Get), new { id = contact.Id }, contact);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateContactCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != command.Id)
            {
                return BadRequest();
            }

            var contact = await _mediator.Send(command);
            if (contact == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete contacts
        public async Task<IActionResult> Delete(string id)
        {
            var command = new DeleteContactCommand { Id = id };
            var result = await _mediator.Send(command);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}