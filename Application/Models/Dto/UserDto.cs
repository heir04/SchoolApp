using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Application.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; } = string.Empty;
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; } 
    }
}