using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class TeacherSubject : AuditableEntity
    {
        public Guid TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public Guid SubjectId { get; set; }
        public Subject? Subject { get; set; }
    }
}