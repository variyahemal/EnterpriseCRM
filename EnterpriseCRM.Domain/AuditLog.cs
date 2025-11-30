using System;

namespace EnterpriseCRM.Domain
{
    public class AuditLog : BaseEntity<string>
    {
        public string UserId { get; set; }
        public string EntityName { get; set; }
        public string EntityId { get; set; }
        public ActionType ActionType { get; set; }
        public DateTime Timestamp { get; set; }
        public string Changes { get; set; } // JSON string

        // Navigation properties
        public AppUser User { get; set; }
    }

    public enum ActionType
    {
        Create,
        Update,
        Delete
    }
}