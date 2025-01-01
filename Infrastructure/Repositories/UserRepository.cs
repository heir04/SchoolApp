using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

        public async Task<User> GetUser(Expression<Func<User, bool>> expression)
        {
            var user =  _context.Users
                .Where(expression)
                .Include(x => x.Teacher)
                .Include(x => x.Student)
                .SingleOrDefaultAsync(expression);
            return await user;
        }
    }
}
