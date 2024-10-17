using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class StudentRepository : BaseRepository<Student>, IStudentRepository
    {
        public StudentRepository(ApplicationContext context) 
        {
            _context = context;
        }
    }
}