using System.ComponentModel.DataAnnotations;

namespace SchoolApp.Application.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public Guid? RoleId { get; set; }
        public string? RoleName { get; set; } 
    }
}