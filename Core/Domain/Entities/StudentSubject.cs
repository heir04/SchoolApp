using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class StudentSubject : AuditableEntity
    {
        public int StudentId { get;set; }
        public Student? Student { get;set; }
        public int SubjectId { get;set; }
        public Subject? Subject { get;set; }
    }
}
