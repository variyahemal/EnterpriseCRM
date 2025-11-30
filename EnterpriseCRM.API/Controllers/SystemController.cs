using Asp.Versioning;
using EnterpriseCRM.API.DTOs.System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EnterpriseCRM.Infrastructure.Persistence;
using EnterpriseCRM.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace EnterpriseCRM.API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize(Roles = "Admin")] // Base authorization for SystemController - only Admin
    public class SystemController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<AppUser> _userManager;

        public SystemController(ApplicationDbContext context, IConfiguration configuration, UserManager<AppUser> userManager)
        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpGet("settings")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetSettings()
        {
            // Retrieve system settings from configuration
            var settings = new SettingsDto
            {
                Settings = new Dictionary<string, string>
                {
                    { "JwtSettings:ValidIssuer", _configuration["JwtSettings:ValidIssuer"] ?? string.Empty },
                    { "JwtSettings:ValidAudience", _configuration["JwtSettings:ValidAudience"] ?? string.Empty },
                    { "JwtSettings:ExpiryMinutes", _configuration["JwtSettings:ExpiryMinutes"] ?? string.Empty },
                    { "JwtSettings:RefreshTokenValidityInDays", _configuration["JwtSettings:RefreshTokenValidityInDays"] ?? string.Empty },
                    { "ConnectionStrings:DefaultConnection", "***HIDDEN***" } // Don't expose connection string
                }
            };

            return Ok(settings);
        }

        [HttpPut("settings")]
        [Authorize(Roles = "Admin")]
        public IActionResult UpdateSettings([FromBody] UpdateSettingsDto updateSettingsDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Note: In a production system, you would typically store settings in a database
            // For now, we'll return a message indicating that settings updates require configuration file changes
            // In a real implementation, you'd have a SystemSettings entity in the database

            if (string.IsNullOrWhiteSpace(updateSettingsDto.SettingName) || string.IsNullOrWhiteSpace(updateSettingsDto.SettingValue))
            {
                return BadRequest("SettingName and SettingValue are required.");
            }

            // For security, don't allow updating sensitive settings like connection strings or security keys via API
            var restrictedSettings = new[] { "ConnectionStrings", "SecurityKey", "JwtSettings:SecurityKey" };
            if (restrictedSettings.Any(rs => updateSettingsDto.SettingName.Contains(rs, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("This setting cannot be updated via the API for security reasons.");
            }

            // In a real implementation, you would update settings in a database table
            // For now, return a message indicating the limitation
            return Ok(new { 
                Message = "Settings update received. Note: In production, settings should be stored in a database. Configuration file updates require application restart.",
                SettingName = updateSettingsDto.SettingName,
                SettingValue = updateSettingsDto.SettingValue
            });
        }

        [HttpGet("audit-logs")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogs(
            [FromQuery] int page = 1, 
            [FromQuery] int pageSize = 50,
            [FromQuery] string? entityName = null,
            [FromQuery] string? actionType = null)
        {
            if (page < 1 || pageSize < 1)
            {
                return BadRequest("Page and pageSize must be positive integers.");
            }

            if (pageSize > 100)
            {
                return BadRequest("Page size cannot exceed 100.");
            }

            var query = _context.AuditLogs
                .Where(a => !a.IsDeleted)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(entityName))
            {
                query = query.Where(a => a.EntityName == entityName);
            }

            if (!string.IsNullOrWhiteSpace(actionType) && Enum.TryParse<ActionType>(actionType, true, out var actionTypeEnum))
            {
                query = query.Where(a => a.ActionType == actionTypeEnum);
            }

            var auditLogs = await query
                .OrderByDescending(a => a.Timestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = _context.Users.Where(u => u.Id == a.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault() ?? string.Empty,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId,
                    ActionType = a.ActionType.ToString(),
                    Timestamp = a.Timestamp,
                    Changes = a.Changes
                })
                .ToListAsync();

            return Ok(auditLogs);
        }

        [HttpGet("audit-logs/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<AuditLogDto>> GetAuditLog(string id)
        {
            var auditLog = await _context.AuditLogs
                .Where(a => a.Id == id && !a.IsDeleted)
                .Select(a => new AuditLogDto
                {
                    Id = a.Id,
                    UserId = a.UserId,
                    UserName = _context.Users.Where(u => u.Id == a.UserId).Select(u => u.FirstName + " " + u.LastName).FirstOrDefault() ?? string.Empty,
                    EntityName = a.EntityName,
                    EntityId = a.EntityId,
                    ActionType = a.ActionType.ToString(),
                    Timestamp = a.Timestamp,
                    Changes = a.Changes
                })
                .FirstOrDefaultAsync();

            if (auditLog == null)
            {
                return NotFound("Audit log not found.");
            }

            return Ok(auditLog);
        }
    }
}