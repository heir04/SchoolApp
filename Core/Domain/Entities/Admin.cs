using SchoolApp.Core.Domain.Common;
using SchoolApp.Core.Domain.Identity;

namespace SchoolApp.Core.Domain.Entities
{
    public class Admin : AuditableEntity
    {
        public string? FirstName {get;set;}
        public string? LastName {get;set;}
        public string? Email { get;set;}
        public User? User { get; set; }
        public Guid UserId { get; set; }
    }
}
