using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class LevelDto
    {
        public Guid? Id { get; set; }
        public string? LevelName { get; set; }
        public IList<Student>? Students { get; set; }
    }
}
