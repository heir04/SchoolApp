using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
    {
        public TeacherRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
}