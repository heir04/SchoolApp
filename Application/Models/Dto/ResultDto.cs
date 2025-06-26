using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class ResultDto
    {
        public Guid Id { get; set; }
        public string? StudentName { get; set; }
        public string? Level { get; set; }
        public double ContinuousAssessment { get; set; }
        public double ExamScore { get; set; }
        public string? TermName { get; set; }
        public string? SessionName { get; set; }
        public double TotalScore { get; set; }
        public string? Remark { get; set; }
        public ICollection<SubjectScoreDto> SubjectScores { get; set; } = new HashSet<SubjectScoreDto>();
        public IList<Result>? Result { get; set; }
    }
}