using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SchoolApp.Application.Models.Dto
{
    public class UpdateUserPasswordDto
    {
        public string CurrentPassword { get; set; } 
        public string NewPassword { get; set; }
    }
}