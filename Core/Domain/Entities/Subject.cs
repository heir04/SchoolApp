using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Subject : AuditableEntity
    {
        public string? Name { get; set; }
        public string? Category { get; set; }
        public IList<StudentSubject>? StudentSubjects { get; set; }
    }
}