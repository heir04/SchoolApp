using SchoolApp.Core.Domain.Entities;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
}