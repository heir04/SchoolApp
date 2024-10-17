using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Core.Dto
{
    public class StudentDto
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? StudentId { get; set; }
        public Guid? LevelId { get; set; }
        public string? LevelName { get; set; }
        public IList<StudentSubject>? StudentSubjects { get; set; }
    }
}