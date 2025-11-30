using System;

namespace EnterpriseCRM.Domain
{
    public class SaleOpportunity : BaseEntity<string>
    {
        public string? Name { get; set; }
        public SaleOpportunityStatus Status { get; set; }
        public Guid ContactId { get; set; }
        public DateTime ExpectedCloseDate { get; set; }
        public decimal Value { get; set; }

        // Navigation properties
        public Contact? Contact { get; set; }
    }

    public enum SaleOpportunityStatus
    {
        Prospect,
        Proposal,
        Won,
        Lost
    }
}