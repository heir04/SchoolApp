using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class TeacherDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        // public string? StudentId { get; set; }
        public int UserId { get; set; }
        public List<Guid>? SubjectIds { get; set; }
        public IList<TeacherSubject>? TeacherSubjects { get; set; }
        public string? Password { get; internal set; }
    }
}