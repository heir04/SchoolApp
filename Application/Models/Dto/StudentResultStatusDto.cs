using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class StudentResultStatusDto
    {
        public string? StudentName { get; set; }
        public string? Message { get; set; }
        public string? Status { get; set; }
    }
}