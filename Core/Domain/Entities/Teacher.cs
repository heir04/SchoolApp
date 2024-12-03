using SchoolApp.Core.Domain.Common;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Core.Domain.Entities
{
    public class Teacher:AuditableEntity
    {
        public string? FirstName {get;set;}
        public string? LastName {get;set;}
        public string? Email { get;set;}

        // public Guid LevelId { get; set; }
        // public Level? Level { get; set; }
        public User? User { get; set; }
        public Guid UserId { get; set; }
        public ICollection<TeacherSubject> TeacherSubjects {get;set;} = new HashSet<TeacherSubject>();
    }
}