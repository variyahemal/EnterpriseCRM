using System;

namespace EnterpriseCRM.Domain
{
    public class FollowUp : BaseEntity<string>
    {
        public Guid ContactId { get; set; }
        public DateTime Date { get; set; }
        public FollowUpType Type { get; set; }
        public string Notes { get; set; }

        // Navigation properties
        public Contact Contact { get; set; }
    }

    public enum FollowUpType
    {
        Call,
        Email,
        Meeting
    }
}