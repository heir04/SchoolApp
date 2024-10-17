using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class SubjectRepository : BaseRepository<Subject>, ISubjectRepository
    {
        public SubjectRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
}