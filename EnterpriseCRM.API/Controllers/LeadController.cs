using Asp.Versioning;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.Application.Features.Leads.Queries.GetLead;
using EnterpriseCRM.Application.Features.Leads.Commands.CreateLead;
using EnterpriseCRM.Application.Features.Leads.Commands.UpdateLead;
using EnterpriseCRM.Application.Features.Leads.Commands.DeleteLead;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin,Editor,Sales")] // Base authorization for LeadController
    public class LeadController : ControllerBase
    {
        private readonly IMediator _mediator;

        public LeadController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> Get(Guid id)
        {
            var query = new GetLeadQuery { Id = id };
            var lead = await _mediator.Send(query);

            if (lead == null)
            {
                return NotFound();
            }

            return Ok(lead);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> Create([FromBody] CreateLeadCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var lead = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = lead.Id }, lead);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Editor,Sales")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateLeadCommand command)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (id != command.Id)
            {
                return BadRequest();
            }

            await _mediator.Send(command);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")] // Only Admin can delete leads
        public async Task<IActionResult> Delete(Guid id)
        {
            var command = new DeleteLeadCommand { Id = id };
            await _mediator.Send(command);
            return NoContent();
        }
    }
}