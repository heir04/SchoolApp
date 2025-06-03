using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class StudentDto
    {
        public Guid? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? StudentId { get; set; }
        public Guid LevelId { get; set; }
        public string? LevelName { get; set; }
        public DateTime? CreatedOn { get; set; }
        // public IList<StudentSubject>? StudentSubjects { get; set; }
    }
}