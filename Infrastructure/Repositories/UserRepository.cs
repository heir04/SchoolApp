using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationContext context) 
        {
            _context = context;
        }
    }
}
