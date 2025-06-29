using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class LevelDto
    {
        public Guid? Id { get; set; }
        public string? LevelName { get; set; }
        public required string Category { get; set; }
        // public Guid LevelTeacherId { get; set; }
        // public Teacher? LevelTeacher { get; set; }
        // public IList<Student>? Students { get; set; }
    }
}
