using SchoolApp.Application.Abstraction.IRepositories;
using SchoolApp.Core.Domain.Entities;
using SchoolApp.Infrastructure.Context;

namespace SchoolApp.Infrastructure.Repositories
{
    public class LevelRepository : BaseRepository<Level>, ILevelRepository
    {
        public LevelRepository(ApplicationContext context) 
        {
            _context = context;
        }
    }
}
