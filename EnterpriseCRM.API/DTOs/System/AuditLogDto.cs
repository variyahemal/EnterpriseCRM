namespace EnterpriseCRM.API.DTOs.System
{
    public class AuditLogDto
    {
        public string Id { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string EntityName { get; set; } = string.Empty;
        public string EntityId { get; set; } = string.Empty;
        public string ActionType { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string Changes { get; set; } = string.Empty;
    }
}

