using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Application.Models.Dto
{
    public class SubjectScoreDto
    {
        public string? SubjectName { get; set; }
        public double ContinuousAssessment{ get; set; }
        public double ExamScore{ get; set; }
        public double TotalScore { get; set; }
    }
}