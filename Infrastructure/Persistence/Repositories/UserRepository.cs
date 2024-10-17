using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) 
        {
            _context = context;
        }
    }
}
