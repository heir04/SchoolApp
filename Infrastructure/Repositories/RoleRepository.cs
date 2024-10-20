using Microsoft.EntityFrameworkCore;
using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Identity;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
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