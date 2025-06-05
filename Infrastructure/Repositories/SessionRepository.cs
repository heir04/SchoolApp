using Microsoft.EntityFrameworkCore;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class SessionRepository : BaseRepository<Session>, ISessionRepository
    {
        public SessionRepository(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<Session> GetCurrentSession()
        {
            var session = await _context.Sessions
            .Where(s => s.CurrentSession == true)
            .Include(s => s.Terms)
            .FirstOrDefaultAsync();

            return session;
        }
        
    }
}