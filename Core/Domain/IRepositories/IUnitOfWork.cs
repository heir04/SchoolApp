namespace SchoolApp.Core.Domain.IRepositories
{
    public interface IUnitOfWork : IDisposable
    {
        IAdminRepository Admin { get; }
        ILevelRepository Level { get; }
        IRoleRepository Role { get; }
        IResultRepository Result { get; }
        IStudentRepository Student { get; }
        ISubjectRepository Subject { get; }
        ISessionRepository Session { get; }
        ITeacherRepository Teacher { get; }
        IUserRepository User { get; }
        Task<int> SaveChangesAsync();
    }
}