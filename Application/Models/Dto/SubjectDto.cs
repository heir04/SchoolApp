using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class SubjectDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? StudentId { get; set; }
        public int LevelId { get; set; }
        public int UserId { get; set; }
        public IList<StudentSubject>? StudentSubjects { get; set; }
    }
}