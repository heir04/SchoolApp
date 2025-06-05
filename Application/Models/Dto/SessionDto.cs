namespace SchoolApp.Application.Models.Dto
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string? SessionName { get; set; } 
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public bool CurrentSession { get; set; }
    }
}