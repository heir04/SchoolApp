using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class StudentScoreDto
    {
        public Guid StudentId { get; set; }
        public string? StudentName { get;  set; }
        public double ContinuousAssessment { get; set; }
        public double ExamScore { get; set; }
    }
}