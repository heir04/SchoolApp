using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class TermDto
    {
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public Guid SessionId { get; set; }
    }
}