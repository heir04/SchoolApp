namespace SchoolApp.Core.Domain.Common
{
   public interface ISoftDelete
    {
        DateTime? IsDeleteOn { get; set; }
        Guid IsDeleteBy { get; set; }
        bool IsDeleted { get; set; }
    }
}