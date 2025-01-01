namespace SchoolApp.Application.Models.Dto
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string? Email { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public Guid? RoleId { get; set; } = null!;
        public string RoleName { get; set; } = null!;
    }
}