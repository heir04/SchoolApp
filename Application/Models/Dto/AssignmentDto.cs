using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class AssignmentDto
    {
        public string? Content { get; set; }
        public Guid LevelId { get; set; }
        public string? Level { get; set; }
        public Guid SessionId { get; set; }
        public string? Session { get; set; }
        public Guid TermId { get; set; }
        public string? Term { get; set; }
        public Guid TeacherId { get; set; }
        public string? Teacher { get; set; }
        public Guid SubjectId { get; set; }
        public string? Subject { get; set; }
        public Guid Id { get; set; }
        public DateOnly DueDate { get; set; }
        public string? Instructions { get; set; }
    }
}