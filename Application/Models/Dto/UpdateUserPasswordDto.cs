using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class UpdateUserPasswordDto
    {
        public required string CurrentPassword { get; set; } 
        public required string NewPassword { get; set; }
    }
}