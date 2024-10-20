using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class AdminRepository : BaseRepository<Admin>, IAdminRepository
    {
        public AdminRepository(ApplicationContext context)
        {
            _context = context;
        }
    }
}
