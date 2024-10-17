namespace SchoolApp.Core.Dto
{
    public class SessionDto
    {
        public Guid Id { get; set; }
        public string SessionName { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public bool CurrentSession { get; set; }
    }
}