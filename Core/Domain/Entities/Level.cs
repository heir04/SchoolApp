using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Level : AuditableEntity
    {
        public string? LevelName { get; set; }
        public IList<Student>? Students { get; set;}
    }
}
