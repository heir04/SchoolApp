using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Level : AuditableEntity
    {
        public string? LevelName { get; set; }
        //public Guid TeacherId { get; set; }
        //public Teacher? Teacher { get; set; }
        public IList<Student>? Students { get; set;}
    }
}
