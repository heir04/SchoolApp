using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class TeacherDto
    {
        public Guid Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Address { get; set; }
        public List<Guid>? SubjectIds { get; set; }
        public ICollection<string> Subjects { get; set; } = new HashSet<string>();
        public string? Password { get; set; }
        public DateTime? CreatedOn { get; set; }
    }
}