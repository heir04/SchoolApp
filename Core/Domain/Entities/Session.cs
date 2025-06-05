using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Session : AuditableEntity
    {
        public string SessionName { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public ICollection<Term> Terms { get; set; } = []; 
        public bool CurrentSession { get; set; }
    }
}