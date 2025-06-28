using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Level : AuditableEntity
    {
        public string? LevelName { get; set; } 
        public string? Category { get; set; } // "Junior Secondary", "Senior Secondary"
        //public Guid TeacherId { get; set; }
        //public Teacher? Teacher { get; set; }
        public IList<Student>? Students { get; set;}
    }
}
