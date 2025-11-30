using System;

namespace EnterpriseCRM.Domain
{
    public abstract class BaseEntity<TId>
    {
        public TId Id { get; set; }
        public DateTime CreatedOn { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedOn { get; set; }
        public string? UpdatedBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}