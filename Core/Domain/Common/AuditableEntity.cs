namespace SchoolApp.Core.Domain.Common
{
    public abstract class AuditableEntity: BaseEntity, IAuditableEntity,ISoftDelete
    {
        public Guid CreatedBy { get; set; } = new Guid();
        public DateTime? CreatedOn { get; set; } = DateTime.UtcNow;
        public Guid LastModifiedBy { get; set; } = new Guid();
        public DateTime? LastModifiedOn { get; set; }
        public DateTime? IsDeleteOn { get; set; }
        public Guid IsDeleteBy { get; set; }
        public bool IsDeleted { get; set; }
    }
}