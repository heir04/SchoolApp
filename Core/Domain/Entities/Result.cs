using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Result: AuditableEntity
    {
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public Guid LevelId { get; set; }
        public Level? Level { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        public double ContinuousAssessment{ get; set; }
        public double ExamScore{ get; set; }
        public Terms Terms{ get; set; }
        public double TotalScore{ get; set; }
        // public string Grade{ get; set; }
        public string? Remark {get;set;}
    }
}