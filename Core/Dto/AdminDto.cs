namespace SchoolApp.Core.Dto
{
    public class AdminDto
    {
        public Guid? Id {get;set;}
        public string? Email { get;set;}
        public string? Password { get;set;}
        public string? FirstName {get;set;}
        public string? LastName {get;set;}
    }
}