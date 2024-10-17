using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
}