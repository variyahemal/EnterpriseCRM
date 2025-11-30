using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.API.DTOs.Contacts;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using EnterpriseCRM.Infrastructure.Persistence;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/contacts")]
    [Authorize(Roles = "Admin,Editor,Sales")] // Base authorization for ContactsController
    public class ContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<IEnumerable<ContactDto>>> GetContacts(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            var contacts = await _context.Contacts
                .Where(c => !c.IsDeleted) // Only retrieve non-deleted contacts
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new ContactDto
                {
                    Id = c.Id.ToString(),
                    FirstName = c.FirstName ?? string.Empty,
                    LastName = c.LastName ?? string.Empty,
                    Email = c.Email ?? string.Empty,
                    Phone = c.Phone ?? string.Empty,
                    Company = c.Company ?? string.Empty,
                    Address = c.Address ?? string.Empty,
                    City = c.City ?? string.Empty,
                    State = c.State ?? string.Empty,
                    ZipCode = c.ZipCode ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .ToListAsync();

            return Ok(contacts);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<ContactDto>> GetContact(string id)
        {
            if (!Guid.TryParse(id, out var contactId))
            {
                return BadRequest("Invalid contact ID format.");
            }

            var contact = await _context.Contacts
                .Where(c => c.Id == contactId && !c.IsDeleted)
                .Select(c => new ContactDto
                {
                    Id = c.Id.ToString(),
                    FirstName = c.FirstName ?? string.Empty,
                    LastName = c.LastName ?? string.Empty,
                    Email = c.Email ?? string.Empty,
                    Phone = c.Phone ?? string.Empty,
                    Company = c.Company ?? string.Empty,
                    Address = c.Address ?? string.Empty,
                    City = c.City ?? string.Empty,
                    State = c.State ?? string.Empty,
                    ZipCode = c.ZipCode ?? string.Empty,
                    Country = c.Country ?? string.Empty,
                    CreatedAt = c.CreatedAt,
                    UpdatedAt = c.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (contact == null)
            {
                return NotFound("Contact not found.");
            }

            return Ok(contact);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<ContactDto>> CreateContact([FromBody] CreateContactDto createContactDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = createContactDto.FirstName ?? string.Empty,
                LastName = createContactDto.LastName ?? string.Empty,
                Email = createContactDto.Email ?? string.Empty,
                Phone = createContactDto.Phone ?? string.Empty,
                Company = createContactDto.Company ?? string.Empty,
                Address = createContactDto.Address ?? string.Empty,
                City = createContactDto.City ?? string.Empty,
                State = createContactDto.State ?? string.Empty,
                ZipCode = createContactDto.ZipCode ?? string.Empty,
                Country = createContactDto.Country ?? string.Empty,
                CreatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            var contactDto = new ContactDto
            {
                Id = contact.Id.ToString(),
                FirstName = contact.FirstName ?? string.Empty,
                LastName = contact.LastName ?? string.Empty,
                Email = contact.Email ?? string.Empty,
                Phone = contact.Phone ?? string.Empty,
                Company = contact.Company ?? string.Empty,
                Address = contact.Address ?? string.Empty,
                City = contact.City ?? string.Empty,
                State = contact.State ?? string.Empty,
                ZipCode = contact.ZipCode ?? string.Empty,
                Country = contact.Country ?? string.Empty,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, contactDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<ContactDto>> UpdateContact(string id, [FromBody] UpdateContactDto updateContactDto)
        {
            if (!Guid.TryParse(id, out var contactId))
            {
                return BadRequest("Invalid contact ID format.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var contact = await _context.Contacts.FindAsync(contactId);

            if (contact == null || contact.IsDeleted)
            {
                return NotFound("Contact not found.");
            }

            contact.FirstName = updateContactDto.FirstName ?? contact.FirstName ?? string.Empty;
            contact.LastName = updateContactDto.LastName ?? contact.LastName ?? string.Empty;
            contact.Email = updateContactDto.Email ?? contact.Email ?? string.Empty;
            contact.Phone = updateContactDto.Phone ?? contact.Phone ?? string.Empty;
            contact.Company = updateContactDto.Company ?? contact.Company ?? string.Empty;
            contact.Address = updateContactDto.Address ?? contact.Address ?? string.Empty;
            contact.City = updateContactDto.City ?? contact.City ?? string.Empty;
            contact.State = updateContactDto.State ?? contact.State ?? string.Empty;
            contact.ZipCode = updateContactDto.ZipCode ?? contact.ZipCode ?? string.Empty;
            contact.Country = updateContactDto.Country ?? contact.Country ?? string.Empty;
            contact.UpdatedAt = DateTime.UtcNow;

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contactId))
                {
                    return NotFound("Contact not found.");
                }
                else
                {
                    throw;
                }
            }

            var contactDto = new ContactDto
            {
                Id = contact.Id.ToString(),
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Phone = contact.Phone,
                Company = contact.Company,
                Address = contact.Address,
                City = contact.City,
                State = contact.State,
                ZipCode = contact.ZipCode,
                Country = contact.Country,
                CreatedAt = contact.CreatedAt,
                UpdatedAt = contact.UpdatedAt
            };

            return Ok(contactDto);
        }

        private bool ContactExists(Guid id)
        {
            return _context.Contacts.Any(e => e.Id == id && !e.IsDeleted);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can soft-delete contacts
        public async Task<IActionResult> DeleteContact(string id)
        {
            if (!Guid.TryParse(id, out var contactId))
            {
                return BadRequest("Invalid contact ID format.");
            }

            var contact = await _context.Contacts.FindAsync(contactId);
            if (contact == null || contact.IsDeleted)
            {
                return NotFound("Contact not found.");
            }

            contact.IsDeleted = true;
            contact.UpdatedAt = DateTime.UtcNow;

            _context.Entry(contact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ContactExists(contactId))
                {
                    return NotFound("Contact not found.");
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
    }
}