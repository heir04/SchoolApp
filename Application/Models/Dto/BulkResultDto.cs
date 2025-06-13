
namespace SchoolApp.Application.Models.Dto
{
    public class BulkResultDto
    {
        public Guid LevelId { get; set; }
        public Guid SubjectId { get; set; }
        public List<StudentScoreDto> StudentScores { get; set; } = [];
        
    }
}