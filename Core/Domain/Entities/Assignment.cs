using System.ComponentModel.DataAnnotations;
using SchoolApp.Core.Domain.Common;

namespace SchoolApp.Core.Domain.Entities
{
    public class Assignment : AuditableEntity
    {
        public string? Content { get; set; }
        public Guid LevelId { get; set; }
        public Level? Level { get; set; }
        public Guid SessionId { get; set; }
        public Session? Session { get; set; }
        public Term? Term { get; set; }
        public Guid TermId { get; set; }
        public Guid TeacherId { get; set; }
        public Teacher? Teacher { get; set; }
        public Guid SubjectId { get; set; }
        public Subject? Subject { get; set; }
        public DateOnly DueDate { get; set; }
        public string? Instructions { get; set; }
    }
}