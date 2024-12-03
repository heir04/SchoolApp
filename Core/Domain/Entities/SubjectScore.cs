using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class SubjectScore : AuditableEntity
    {
        public Guid SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public double ContinuousAssessment{ get; set; }
        public double ExamScore{ get; set; }
        public double TotalScore { get; set; }
    }
}