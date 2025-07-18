using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationContext _context;
        private bool _disposed = false;
        
        public IAdminRepository Admin { get; }
        public IAssignmentRepository Assignment { get;  }
        public ILevelRepository Level { get; }
        public IRoleRepository Role { get; }
        public IResultRepository Result { get; }
        public IStudentRepository Student { get; }
        public ISubjectRepository Subject { get; }
        public ISessionRepository Session { get; }
        public ITeacherRepository Teacher { get; }
        public ITermRepository Term { get; }
        public IUserRepository User { get; }

        public UnitOfWork(
            ApplicationContext context,
            IAdminRepository adminRepository,
            IAssignmentRepository assignmentRepository,
            ILevelRepository levelRepository,
            IRoleRepository roleRepository,
            IResultRepository resultRepository,
            IStudentRepository studentRepository,
            ISubjectRepository subjectRepository,
            ISessionRepository sessionRepository,
            ITeacherRepository teacherRepository,
            ITermRepository termRepository,
            IUserRepository userRepository
        )
        {
            _context = context;
            Admin = adminRepository;
            Assignment = assignmentRepository;
            Level = levelRepository;
            Role = roleRepository;
            Result = resultRepository;
            Student = studentRepository;
            Subject = subjectRepository;
            Session = sessionRepository;
            Teacher = teacherRepository;
            Term = termRepository;
            User = userRepository;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
                _disposed = true;
            }
        }
    }
}