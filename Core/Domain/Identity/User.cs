using SchoolApp.Core.Domain.Common;
using SchoolApp.Core.Domain.Entities;

namespace SchoolApp.Core.Domain.Identity
{
    public class User : AuditableEntity
    {
        public string? UserName {get;set;}
        public string? Password { get; set; }
        public string? HashSalt { get; set; }
        public string? PasswordHash { get; set; }
        public string? Email { get; set; }
        public Admin? Admin { get; set; }
        public Student? Student { get; set; }
        public Teacher? Teacher { get; set; }
        // public Guid RoleId { get; set; }
        // public Role? Role { get; set; }
        public List<UserRole> UserRoles { get; set; } = new List<UserRole>();
    }
}