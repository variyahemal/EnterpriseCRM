using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.API.DTOs.Leads;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using EnterpriseCRM.Infrastructure.Persistence;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/leads")]
    [Authorize(Roles = "Admin,Editor,Sales")] // Base authorization for LeadsController
    public class LeadsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public LeadsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<IEnumerable<LeadDto>>> GetLeads(
            [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            var leads = await _context.Leads
                .Where(l => !l.IsDeleted) // Only retrieve non-deleted leads
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(l => new LeadDto
                {
                    Id = l.Id.ToString(),
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    Phone = l.Phone,
                    Company = l.Company,
                    Source = l.Source,
                    Status = l.Status,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .ToListAsync();

            return Ok(leads);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<LeadDto>> GetLead(string id)
        {
            if (!Guid.TryParse(id, out var leadId))
            {
                return BadRequest("Invalid lead ID format.");
            }

            var lead = await _context.Leads
                .Where(l => !l.IsDeleted && l.Id == leadId)
                .Select(l => new LeadDto
                {
                    Id = l.Id.ToString(),
                    FirstName = l.FirstName,
                    LastName = l.LastName,
                    Email = l.Email,
                    Phone = l.Phone,
                    Company = l.Company,
                    Source = l.Source,
                    Status = l.Status,
                    CreatedAt = l.CreatedAt,
                    UpdatedAt = l.UpdatedAt
                })
                .FirstOrDefaultAsync();

            if (lead == null)
            {
                return NotFound("Lead not found.");
            }

            return Ok(lead);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<LeadDto>> CreateLead([FromBody] CreateLeadDto createLeadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var lead = new Lead
            {
                Id = Guid.NewGuid(),
                FirstName = createLeadDto.FirstName ?? string.Empty,
                LastName = createLeadDto.LastName ?? string.Empty,
                Email = createLeadDto.Email ?? string.Empty,
                Phone = createLeadDto.Phone ?? string.Empty,
                Company = createLeadDto.Company ?? string.Empty,
                Source = createLeadDto.Source ?? string.Empty,
                Status = "New", // Default status for a new lead
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Leads.Add(lead);
            await _context.SaveChangesAsync();

            var leadDto = new LeadDto
            {
                Id = lead.Id.ToString(),
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                Source = lead.Source,
                Status = lead.Status,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return CreatedAtAction(nameof(GetLead), new { id = leadDto.Id }, leadDto);
        }

        [HttpPut("{id}/status")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<LeadDto>> UpdateLeadStatus(string id, [FromBody] UpdateLeadStatusDto updateLeadStatusDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(id, out var leadId))
            {
                return BadRequest("Invalid lead ID format.");
            }

            var lead = await _context.Leads.FindAsync(leadId);

            if (lead == null || lead.IsDeleted)
            {
                return NotFound("Lead not found.");
            }

            lead.Status = updateLeadStatusDto.Status ?? lead.Status;
            lead.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var leadDto = new LeadDto
            {
                Id = lead.Id.ToString(),
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                Source = lead.Source,
                Status = lead.Status,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return Ok(leadDto);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult<LeadDto>> UpdateLead(string id, [FromBody] UpdateLeadDto updateLeadDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!Guid.TryParse(id, out var leadId))
            {
                return BadRequest("Invalid lead ID format.");
            }

            var lead = await _context.Leads.FindAsync(leadId);

            if (lead == null || lead.IsDeleted)
            {
                return NotFound("Lead not found.");
            }

            lead.FirstName = updateLeadDto.FirstName ?? lead.FirstName;
            lead.LastName = updateLeadDto.LastName ?? lead.LastName;
            lead.Email = updateLeadDto.Email ?? lead.Email;
            lead.Phone = updateLeadDto.Phone ?? lead.Phone;
            lead.Company = updateLeadDto.Company ?? lead.Company;
            lead.Source = updateLeadDto.Source ?? lead.Source;
            lead.Status = updateLeadDto.Status ?? lead.Status;
            lead.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            var leadDto = new LeadDto
            {
                Id = lead.Id.ToString(),
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                Source = lead.Source,
                Status = lead.Status,
                CreatedAt = lead.CreatedAt,
                UpdatedAt = lead.UpdatedAt
            };

            return Ok(leadDto);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete leads
        public async Task<IActionResult> DeleteLead(string id)
        {
            if (!Guid.TryParse(id, out var leadId))
            {
                return BadRequest("Invalid lead ID format.");
            }

            var lead = await _context.Leads.FindAsync(leadId);

            if (lead == null || lead.IsDeleted)
            {
                return NotFound("Lead not found.");
            }

            lead.IsDeleted = true;
            lead.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpPost("{id}/convert")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<ActionResult> ConvertLead(string id)
        {
            if (!Guid.TryParse(id, out var leadId))
            {
                return BadRequest("Invalid lead ID format.");
            }

            var lead = await _context.Leads.FindAsync(leadId);

            if (lead == null || lead.IsDeleted)
            {
                return NotFound("Lead not found.");
            }

            // Create a new Contact from the Lead
            var contact = new Contact
            {
                Id = Guid.NewGuid(),
                FirstName = lead.FirstName,
                LastName = lead.LastName,
                Email = lead.Email,
                Phone = lead.Phone,
                Company = lead.Company,
                Address = string.Empty,
                City = string.Empty,
                State = string.Empty,
                ZipCode = string.Empty,
                Country = string.Empty,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            _context.Contacts.Add(contact);

            // Optionally, create an Opportunity from the Lead
            // For simplicity, we'll just create a contact for now.
            // In a real CRM, you might have logic to decide if an opportunity is created.

            // Soft delete the lead
            lead.IsDeleted = true;
            lead.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Lead converted to contact successfully.", contactId = contact.Id });
        }
    }
}