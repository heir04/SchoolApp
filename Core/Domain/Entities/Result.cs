using SchoolApp.Core.Domain.Common;
using SchoolApp.Core.Domain.Enums;

namespace SchoolApp.Core.Domain.Entities
{
    public class Result: AuditableEntity
    {
        public Guid StudentId { get; set; }
        public Student? Student { get; set; }
        public Guid LevelId { get; set; } 
        public Level? Level { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        public Terms Terms{ get; set; }
        public double TermTotalScore => SubjectScores.Sum(s =>
        s.TotalScore);
        // public string Grade{ get; set; }
        public string? Remark {get;set;}
        public ICollection<SubjectScore> SubjectScores { get; set; } = new HashSet<SubjectScore>();
    }
}