using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.Enums;

namespace SchoolApp.Application.Models.Dto
{
    public class ResultDto
    {
        public Guid Id { get; set; }
        public string? StudentName { get; set; }
        public string? Level { get; set; }
        public Guid LevelId { get; set; }
        public Guid SubjectId { get; set; }
        public double ContinuousAssessment { get; set; }
        public double ExamScore { get; set; }
        public Terms Terms { get; set; }
        public double TotalScore { get; set; }
        public string? Grade { get; set; }
        public string? Remark { get; set; }
        public ICollection<SubjectScoreDto> SubjectScores { get; set; } = new HashSet<SubjectScoreDto>();
        public IList<Result> Result { get; set; }
    }
}