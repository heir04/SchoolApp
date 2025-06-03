using SchoolApp.Core.Domain.Common;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Core.Domain.Entities
{
    public class Student: AuditableEntity
    {
        public string? FirstName {get;set;}
        public string? LastName {get;set;}
        public string? StudentId {get;set;}
        public string? Email { get;set;}
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public Guid LevelId { get; set; }
        public Level? Level { get; set; }
        public User? User { get; set; }
        public Guid UserId { get; set; }
        public IList<StudentSubject>? StudentSubjects { get; set; }
        public ICollection<Result> Results { get; set; } = new HashSet<Result>();
    }
}