using SchoolApp.Core.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Application.Models.Dto
{
    public class LevelDto
    {
        public Guid? Id { get; set; }
        public string? LevelName { get; set; }
        [Required]
        public string Category { get; set; } = string.Empty;
        // public Guid LevelTeacherId { get; set; }
        // public Teacher? LevelTeacher { get; set; }
        // public IList<Student>? Students { get; set; }
    }
}
