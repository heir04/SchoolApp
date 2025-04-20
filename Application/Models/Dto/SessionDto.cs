namespace SchoolApp.Application.Models.Dto
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string? SessionName { get; set; } 
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CurrentSession { get; set; }
    }
}