using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class ResultRemarkCountDto
    {
        public int TotalResult { get; set; }
        public int WithRemark { get; set; }
        public int WithoutRemark { get; set; }
    }
}