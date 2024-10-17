using Microsoft.EntityFrameworkCore;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Core.Domain.IRepositories;
using SchoolApp.Infrastructure.Persistence.Context;

namespace SchoolApp.Infrastructure.Persistence.Repositories
{
    public class RoleRepository : BaseRepository<Role>, IRoleRepository
    {
        public RoleRepository(ApplicationContext context) 
        {
            _context = context;
        }
        public async Task<IList<Role>> GetRolesByUserId(Guid id)
        {
            var roles = await _context.UserRoles.Include(r => r.Role).Where(x => x.UserId == id).Select(r => new Role
            {
                Name = r.Role.Name
            }).ToListAsync();
            return roles;
        }
    }
}