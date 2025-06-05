namespace SchoolApp.Application.Abstraction.IRepositories
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
        ITermRepository Term { get; }
        IUserRepository User { get; }
        Task<int> SaveChangesAsync();
    }
}