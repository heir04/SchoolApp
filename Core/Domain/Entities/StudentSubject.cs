using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class StudentSubject : AuditableEntity
    {
        public Guid StudentId { get;set; }
        public Student? Student { get;set; }
        public Guid SubjectId { get;set; }
        public Subject? Subject { get;set; }
    }
}